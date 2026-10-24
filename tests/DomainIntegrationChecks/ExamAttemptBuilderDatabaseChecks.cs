using AutoMapper;
using DataModel;
using DataRepository.DataRepositoryEntities;
using DataRepository.GateWay;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using ServicesClasseslibrary.Implmentation.Builder;
using System.Text.Json;
static class ExamAttemptBuilderDatabaseChecks
{
    public static void Run(IMapper mapper)
    {
        var original = Directory.GetCurrentDirectory();
        var folder = Path.Combine(Path.GetTempPath(), "ExamAttemptBuilderChecks_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        var database = "ExamAttemptBuilderChecks_" + Guid.NewGuid().ToString("N");
        File.WriteAllText(Path.Combine(folder, "appsettings.json"), JsonSerializer.Serialize(new {
            ConnectionStrings = new { DbCoreConnectionString = @"Server=(localdb)\MSSQLLocalDB;Database=" + database + ";Integrated Security=true;Connect Timeout=15" } }));
        Directory.SetCurrentDirectory(folder);
        var factory = new Factory();
        bool created = false;
        try
        {
            int subjectId, examId, easyId, hardId;
            using (var db = factory.CreateDbContext())
            {
                var connection = new SqlConnectionStringBuilder(db.Database.GetConnectionString());
                if (connection.InitialCatalog != database || connection.DataSource != @"(localdb)\MSSQLLocalDB")
                    throw new Exception("Refusing to run regression checks against a non-test database.");
                db.Database.EnsureCreated(); created = true;
                var subject = new StudySubject { SubjectName = "Builder regression", NumberOfChapters = 2 };
                var otherSubject = new StudySubject { SubjectName = "Other subject", NumberOfChapters = 2 };
                var easy = new DifficultyLevels { DifficultyLevelName = "Easy" };
                var medium = new DifficultyLevels { DifficultyLevelName = "Medium" };
                var hard = new DifficultyLevels { DifficultyLevelName = "Hard" };
                db.AddRange(subject, otherSubject, easy, medium, hard); db.SaveChanges();
                subjectId = subject.Id; easyId = easy.Id; hardId = hard.Id;
                var exam = new Exams { Title = "Builder regression", StudySubjectId = subjectId };
                db.Add(exam); db.SaveChanges(); examId = exam.Id;
                Questions Question(int difficulty, int chapter = 1, int? owner = null) => new() {
                    QuestionText = "Regression question", StudySubjectId = owner ?? subjectId, DifficultyLevelId = difficulty, ChapterNumber = chapter,
                    QuestionAnswers = Enumerable.Range(1, 3).Select(i => new QuestionAnswers { AnswerText = "Answer " + i, IsCorrect = i == 1 }).ToList() };
                db.AddRange(Enumerable.Range(1, 4).Select(_ => Question(easyId)));
                db.AddRange(Enumerable.Range(1, 2).Select(_ => Question(hardId)));
                // Irrelevant difficulty/chapter must not be materialized into domain questions.
                db.Add(Question(medium.Id, 99));
                db.Add(Question(easyId, 2));
                db.Add(Question(easyId, 1, otherSubject.Id));
                db.SaveChanges();
                var oldRows = db.Questions.AsNoTracking().Include(q => q.QuestionAnswers)
                    .Where(q => q.StudySubjectId == subjectId).OrderBy(q => Guid.NewGuid()).ToList();
                Console.WriteLine($"Original randomized Include returned {oldRows.Count} rows for {oldRows.Select(q => q.Id).Distinct().Count()} question IDs.");
            }
            var builder = new ExamAttemptQuestionBuilder(factory, mapper);
            ExamSectionsDataModel Section(int id) => new() { Id = id, ExamId = examId, NumberOfQuestions = 3, ChapterNumbers = new() { 1 }, ExamSectionRules = new() {
                new() { DifficultyLevelId = easyId, NoOfQuestions = 2 }, new() { DifficultyLevelId = hardId, NoOfQuestions = 1 } } };
            for (int run = 0; run < 10; run++)
            {
                var result = builder.BuildExamAttemptQuestions(examId, new() { Section(1), Section(2) });
                var questions = result.Values.SelectMany(q => q).ToList();
                if (questions.Count != 6 || questions.Select(q => q.Id).Distinct().Count() != 6) throw new Exception("Duplicate question IDs in attempt");
                foreach (var part in result.Values)
                    if (part.Count(q => q.DifficultyLevelId == easyId) != 2 || part.Count(q => q.DifficultyLevelId == hardId) != 1
                        || part.Any(q => q.StudySubjectId != subjectId || q.ChapterNumber != 1 || q.QuestionAnswersDataModel.Count != 3))
                        throw new Exception("Incorrect difficulty/chapter/answer materialization");
            }
            var first = builder.BuildExamAttemptQuestions(subjectId, new ExamSectionRulesDataModel { DifficultyLevelId = easyId, NoOfQuestions = 2 });
            var second = builder.BuildExamAttemptQuestions(subjectId, new ExamSectionRulesDataModel { DifficultyLevelId = easyId, NoOfQuestions = 2 }, first.Select(q => q.Id));
            if (second.Count != 2 || second.Any(q => q.DifficultyLevelId != easyId || first.Any(p => p.Id == q.Id))) throw new Exception("Single-rule difficulty/exclusion filter failed");
            var demanding = Section(1); demanding.NumberOfQuestions = 4; demanding.ExamSectionRules[0].NoOfQuestions = 3;
            try { builder.BuildExamAttemptQuestions(examId, new() { demanding, Section(2) }); throw new Exception("Expected shortage"); }
            catch (NotEnoughQuestionsException) { }
            Console.WriteLine("Passed SQL Server builder regression: required difficulties/chapters, unique question IDs, complete answers, exclusions, and shortages.");
        }
        finally
        {
            try { if (created) { using var cleanup = factory.CreateDbContext(); cleanup.Database.EnsureDeleted(); } }
            finally { Directory.SetCurrentDirectory(original); Directory.Delete(folder, true); }
        }
    }
    sealed class Factory : IDbContextFactory<DbConext>
    {
        public DbConext CreateDbContext() => new(new DbContextOptionsBuilder<DbConext>().Options, new DataBaseConnectionConfiguration());
    }
}
