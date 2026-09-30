using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using AutoMapper;
using DataModel;
using DataRepository.GateWay;
using DataRepository.DataRepositoryEntities;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

static class MaintenanceChecks
{
    static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        Console.WriteLine("PASS: " + message);
    }
    public static void Run(string contentRoot, string connectionString, int examId)
    {
        using var host = new WebHostBuilder().UseKestrel().UseUrls("http://127.0.0.1:0")
            .UseContentRoot(contentRoot).UseEnvironment("Development")
            .UseSetting(WebHostDefaults.ApplicationKey, typeof(OnlineExamSystem.Startup).Assembly.GetName().Name)
            .ConfigureAppConfiguration((context, config) => config.AddInMemoryCollection(new Dictionary<string, string> {
                ["ConnectionStrings:DbCoreConnectionString"] = connectionString,
                ["Logging:LogLevel:Default"] = "Warning"
            }))
            .UseStartup<OnlineExamSystem.Startup>().Build();
        host.Start();
        try
        {
            using (var scope = host.Services.CreateScope())
            using (var another = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var db = services.GetRequiredService<DbConext>();
                Check(ReferenceEquals(db, services.GetRequiredService<DbConext>()) &&
                    !ReferenceEquals(db, another.ServiceProvider.GetRequiredService<DbConext>()), "Context is shared within a scope and isolated across scopes");
                services.GetRequiredService<MapperConfiguration>().AssertConfigurationIsValid();
                Check(true, "All AutoMapper profiles validate");
                var levels = services.GetRequiredService<IDifficultyLevelsOperations>();
                levels.Add(new DifficultyLevelsDataModel { DifficultyLevelName = "Mapping check" });
                var level = levels.list().Single(l => l.DifficultyLevelName == "Mapping check");
                level.DifficultyLevelName = "Updated level";
                levels.Edit(level);
                Check(levels.GetById(level.Id).DifficultyLevelName == "Updated level", "Difficulty-level mapping supports create, read and edit");
                var subjects = services.GetRequiredService<IStudySubjectsOperations>();
                subjects.Add(new StudySubjectDataModel { SubjectName = "Mapping subject" });
                var subject = subjects.list().Single(s => s.SubjectName == "Mapping subject");
                var questions = services.GetRequiredService<IQuestionsOperations>();
                questions.Add(new QuestionsDataModel { QuestionText = "Mapping question", DifficultyLevelId = level.Id,
                    StudySubjectId = subject.Id, QuestionAnswersDataModel = new List<QuestionAnswersDataModel> {
                        new QuestionAnswersDataModel { AnswerText = "Correct", IsCorrect = true },
                        new QuestionAnswersDataModel { AnswerText = "Incorrect" } } });
                var question = questions.list().Single(q => q.QuestionText == "Mapping question");
                var answerId = question.QuestionAnswersDataModel[0].Id;
                question.QuestionText = "Edited question";
                question.QuestionAnswersDataModel[0].AnswerText = "Edited answer";
                questions.Edit(question);
                Check(questions.GetById(question.Id).QuestionAnswersDataModel.Any(a => a.Id == answerId && a.AnswerText == "Edited answer"), "Question edits preserve tracked answer identities");
                var exams = services.GetRequiredService<IExamOprations>();
                exams.Add(new ExamDataModel { Title = "Mapping exam", StudySubjectId = subject.Id,
                    Sections = new List<ExamSectionsDataModel> { new ExamSectionsDataModel { SectionName = "Original", Percentage = 50 } } });
                var exam = exams.GetById(exams.list().Single(e => e.Title == "Mapping exam").Id);
                var sectionId = exam.Sections[0].Id;
                exam.Title = "Edited exam";
                exam.Sections[0].SectionName = "Edited section";
                exam.Sections.Add(new ExamSectionsDataModel { SectionName = "New section", Percentage = 50 });
                exams.Edit(exam);
                var saved = exams.GetById(exam.Id);
                Check(saved.Title == "Edited exam" && saved.Sections.Count == 2 && saved.Sections.Any(s => s.Id == sectionId && s.SectionName == "Edited section"), "Exam edits preserve existing sections and add new sections");
                var userRepo = services.GetRequiredService<IUserOperations>();
                userRepo.Add(new LoginModel { Username = "mapping-user", Password = "test-hash", Role = "Examiner" });
                var user = userRepo.Validate(new LoginModel { Username = "mapping-user", Password = "test-hash" });
                Check(user?.Role == "Examiner" && user.Password == null, "User mapping preserves login behavior without returning password hashes");
            }
            var address = host.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>().Addresses.Single();
            using var client = new HttpClient(new HttpClientHandler { CookieContainer = new CookieContainer(), AllowAutoRedirect = false }) { BaseAddress = new Uri(address) };
            var details = client.GetStringAsync("/Exams/Details/" + examId).GetAwaiter().GetResult();
            Check(details.Contains("exam-details.js") && details.Contains("/ExamSections/Create?examId="), "Exam details renders the correct section route and click-handler script");
            var form = client.GetStringAsync("/ExamSections/Create?examId=" + examId).GetAwaiter().GetResult();
            var tokenInput = Regex.Match(form, "<input[^>]*name=\"__RequestVerificationToken\"[^>]*>").Value;
            var token = WebUtility.HtmlDecode(Regex.Match(tokenInput, "value=\"([^\"]+)\"").Groups[1].Value);
            Check(form.Contains("createSectionForm") && !string.IsNullOrEmpty(token), "Add Section route returns a form with an antiforgery token");
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            var fields = new Dictionary<string, string> { ["ExamId"] = examId.ToString(), ["SectionName"] = "", ["Percentage"] = "20", ["__RequestVerificationToken"] = token };
            var invalid = client.PostAsync("/ExamSections/Create", new FormUrlEncodedContent(fields)).GetAwaiter().GetResult();
            Check((int)invalid.StatusCode == 422 && invalid.Content.ReadAsStringAsync().GetAwaiter().GetResult().Contains("Section name is required"), "AJAX validation returns an editable form with errors");
            fields["SectionName"] = "AJAX section";
            var response = client.PostAsync("/ExamSections/Create", new FormUrlEncodedContent(fields)).GetAwaiter().GetResult();
            Check(response.IsSuccessStatusCode && response.Content.ReadAsStringAsync().GetAwaiter().GetResult().Contains("AJAX section"), "AJAX section submission persists and returns the refreshed list");
            fields.Remove("__RequestVerificationToken");
            var blocked = client.PostAsync("/ExamSections/Create", new FormUrlEncodedContent(fields)).GetAwaiter().GetResult();
            Check(blocked.StatusCode == HttpStatusCode.BadRequest, "Section submission rejects missing antiforgery tokens");
        }
        finally { host.StopAsync().GetAwaiter().GetResult(); }
    }
}
