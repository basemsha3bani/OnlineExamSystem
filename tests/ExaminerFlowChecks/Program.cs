using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using DataRepository.GateWay;
using DataRepository.DataRepositoryEntities;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
using Microsoft.EntityFrameworkCore;
using ServicesClasseslibrary.Examiner;
using ServicesClasseslibrary.Implmentation.Builder;

// Uses a uniquely named, disposable LocalDB database; never uses the application's database.
class Program
{
    static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        Console.WriteLine("PASS: " + message);
    }
    static void Main()
    {
        var originalDirectory = Directory.GetCurrentDirectory();
        var directory = Path.Combine(Path.GetTempPath(), "ExaminerFlowChecks_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var database = "ExaminerFlowChecks_" + Guid.NewGuid().ToString("N");
        File.WriteAllText(Path.Combine(directory, "appsettings.json"), JsonSerializer.Serialize(new {
            ConnectionStrings = new { DbCoreConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=" + database + ";Integrated Security=true;Connect Timeout=15" } }));
        Directory.SetCurrentDirectory(directory);
        try
        {
            using (var db = new DbConext())
            {
                db.Database.EnsureCreated();
                var subject = new StudySubject { SubjectName = "Flow test" };
                var level = new DifficultyLevels { DifficultyLevelName = "Easy" };
                db.Add(subject); db.Add(level); db.SaveChanges();
                var exam = new Exams { Title = "Flow test", StudySubjectId = subject.Id, TotalMarks = 999 };
                db.Add(exam); db.SaveChanges();
                for (int i = 0; i < 2; i++)
                {
                    var section = new ExamSections { ExamId = exam.Id, SectionName = "Section " + i, Percentage = 90 };
                    db.Add(section); db.SaveChanges();
                    db.Add(new ExamSectionRules { SectionId = section.Id, DifficultyLevelId = level.Id, NoOfQuestions = 1 });
                    db.Add(new Questions { QuestionText = "Question " + i, StudySubjectId = subject.Id, DifficultyLevelId = level.Id,
                        QuestionAnswers = new List<QuestionAnswers> { new QuestionAnswers { AnswerText = "Yes", IsCorrect = true }, new QuestionAnswers { AnswerText = "No" } } });
                }
                db.SaveChanges();
            }
            var mapping = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<DataRepository.ModelMapper.RepositoryMappingProfile>(), Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
            mapping.AssertConfigurationIsValid();
            var factory = new TestContextFactory();
            var service = new ExaminerAttemptService(new ExamAttemptQuestionBuilder(factory, mapping.CreateMapper()), factory);
            int examId;
            using (var db = new DbConext()) examId = db.Exams.Single().Id;
            var id = service.Start(examId, 42);
            using (var firstContext = new DbConext())
            using (var secondContext = new DbConext())
            {
                var firstCopy = firstContext.ExaminerAttempts.Single(a => a.Id == id);
                var secondCopy = secondContext.ExaminerAttempts.Single(a => a.Id == id);
                firstCopy.ExamTitle = "Concurrent update";
                firstContext.SaveChanges();
                secondCopy.ExamTitle = "Stale update";
                bool conflict = false;
                try { secondContext.SaveChanges(); } catch (DbUpdateConcurrencyException) { conflict = true; }
                Check(conflict, "Rowversion rejects concurrent stale writes");
            }
            var attempt = service.Get(id, 42);
            var snapshot = ExaminerAttemptService.Read(attempt);
            Check(snapshot.Sections.Count == 2 && snapshot.Questions.Select(q => q.Id).Distinct().Count() == 2, "Builder preserves sections and selects unique questions");
            Check(service.Get(id, 43) == null && service.List(43).Count == 0, "Other users cannot access the attempt");
            var first = snapshot.Sections[0].Questions.Single();
            var correct = first.Options.Single(o => o.IsCorrect).Id;
            service.Save(id, 42, 0, new Dictionary<int, int?> { [first.Id] = correct }, false);
            Check(ExaminerAttemptService.Read(service.Get(id, 42)).Sections[0].Questions[0].SelectedOptionId == correct, "Answers survive navigation and reload");
            bool rejected = false;
            try { service.Save(id, 42, 0, new Dictionary<int, int?> { [first.Id] = -999 }, false); }
            catch (InvalidOperationException) { rejected = true; }
            Check(rejected, "Foreign answer options are rejected");
            using (var db = new DbConext())
            {
                foreach (var option in db.QuestionAnswers) option.IsCorrect = !option.IsCorrect;
                db.SaveChanges();
            }
            service.Save(id, 42, 1, new Dictionary<int, int?>(), true);
            Check(service.Get(id, 42).Status == "Submitted" && service.Get(id, 42).Score == null, "Submission persists without waiting for evaluation");
            service.Save(id, 42, 0, new Dictionary<int, int?>(), true);
            Check(ExaminerAttemptService.Read(service.Get(id, 42)).Sections[0].Questions[0].SelectedOptionId == correct, "Duplicate submission cannot overwrite answers");
            service.EvaluatePending();
            attempt = service.Get(id, 42);
            Check(attempt.Status == "Scored" && attempt.CorrectQuestions == 1 && attempt.Score == 0.5m, "Background scoring uses original answer key, counts blanks, and ignores marks and weights");
            var evaluatedAt = attempt.EvaluatedAt;
            service.EvaluatePending();
            Check(service.Get(id, 42).EvaluatedAt == evaluatedAt, "Worker retries do not duplicate completed results");
            int retryId;
            using (var db = new DbConext())
            {
                db.ExaminerAttempts.Add(new ExaminerAttempt { UserId = 99, ExamId = examId, ExamTitle = "Broken snapshot",
                    SnapshotJson = "broken", Status = "Submitted", StartedAt = DateTime.UtcNow, SubmittedAt = DateTime.UtcNow.AddMinutes(-1), TotalQuestions = 2 });
                var retry = new ExaminerAttempt { UserId = 99, ExamId = examId, ExamTitle = "Valid queued attempt",
                    SnapshotJson = attempt.SnapshotJson, Status = "Submitted", StartedAt = DateTime.UtcNow, SubmittedAt = DateTime.UtcNow, TotalQuestions = 2 };
                db.ExaminerAttempts.Add(retry);
                db.SaveChanges();
                retryId = retry.Id;
            }
            int failures = 0;
            service.EvaluatePending((failedId, error) => failures++);
            Check(failures == 1 && service.Get(retryId, 99).Status == "Scored", "A failed evaluation does not block other queued attempts");
            using (var db = new DbConext())
            {
                db.ExamSectionRules.First().NoOfQuestions = 10;
                db.SaveChanges();
            }
            rejected = false;
            try { service.Start(examId, 42); } catch (InvalidOperationException) { rejected = true; }
            Check(rejected && service.List(42).Count == 1, "Insufficient questions do not create a partial attempt");
            MaintenanceChecks.Run(Path.Combine(originalDirectory, "OnlineExamSystem"), new DataRepository.AppConfiguration().ConnectionString, examId);
            Console.WriteLine("All examiner and maintenance checks passed.");
        }
        finally
        {
            using (var db = new DbConext()) db.Database.EnsureDeleted();
            Directory.SetCurrentDirectory(originalDirectory);
            File.Delete(Path.Combine(directory, "appsettings.json"));
            Directory.Delete(directory);
        }
    }
}

class TestContextFactory : IDbContextFactory<DbConext> { public DbConext CreateDbContext() => new DbConext(); }


