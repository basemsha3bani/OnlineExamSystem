using Domain.Entities;
using Domain.Aggregates;
using Domain.DTOs;
using Domain.Exceptions;
using System.Reflection;
using System.Text.Json;

static class EntityChecks
{
    public static void Run()
    {
        foreach (var type in typeof(DifficultyLevels).Assembly.GetTypes().Where(t => t.Namespace == "Domain.Entities" || t.Namespace == "Domain.Aggregates" || t.Namespace == "Domain.DTOs"))
        {
            foreach (var property in type.GetProperties())
            {
                if (property.SetMethod?.IsPublic == true) throw new Exception($"Public setter: {type.Name}.{property.Name}");
                if (property.GetCustomAttributes().Any(a => a.GetType().Namespace?.StartsWith("System.ComponentModel.DataAnnotations") == true))
                    throw new Exception($"DataAnnotation: {type.Name}.{property.Name}");
            }
        }
        Reject(() => DifficultyLevels.Define(DifficultyLevelName: " "));
        var difficulty = DifficultyLevels.Define(DifficultyLevelName: "Easy");
        Reject(() => difficulty.ChangeDifficultyLevelName(""));
        if (difficulty.DifficultyLevelName != "Easy") throw new Exception("Failed rename changed difficulty");
        Reject(() => QuestionAnswers.Define(AnswerText: ""));
        Reject(() => ExamSectionRules.Define(DifficultyLevelId: 1, NoOfQuestions: 0));
        Reject(() => Questions.CheckAnswers(new[] { QuestionAnswersDTO.Define(AnswerText: "A", IsCorrect: true), QuestionAnswersDTO.Define(AnswerText: "B", IsCorrect: true) }));
        var question = QuestionsDTO.Define(Id: 1, QuestionText: "Q", QuestionAnswersDTO: new() {
            QuestionAnswersDTO.Define(Id: 10, AnswerText: "A", IsCorrect: true), QuestionAnswersDTO.Define(Id: 11, AnswerText: "B") });
        var section = ExamSectionsDTO.Define(Id: 1, SectionName: "Section", exam: ExamDTO.Define(Title: "Exam"));
        var attempt = ExamAttempt.Start(1, 1, new[] { section }, new() { [1] = new() { question } });
        Reject(() => attempt.SaveAnswers( 0, new() { [1] = 99 }, false));
        attempt.SaveAnswers( 0, new() { [1] = 10 }, true);
        if (attempt.SaveAnswers( 0, new() { [1] = 11 }, true)) throw new Exception("Duplicate submission changed answers");
        var copy = JsonSerializer.Deserialize<ExamAttemptDTO>(JsonSerializer.Serialize(attempt.ToDTO()))!;
        var restored = ExamAttempt.Restore(copy);
        restored.Evaluate();
        if (restored.Score != 1m || restored.Status != "Scored") throw new Exception("Snapshot round trip lost answers");
        var cached = JsonSerializer.Deserialize<DifficultyLevelsDTO>(JsonSerializer.Serialize(DifficultyLevelsDTO.Define(Id: 5, DifficultyLevelName: "Hard")))!;
        if (cached.Id != 5 || cached.DifficultyLevelName != "Hard") throw new Exception("Cache round trip failed");
        Console.WriteLine("Passed private setters, annotation removal, entity invariants, invalid answer rejection, submission idempotency, scoring, and JSON round trips.");
    }
    static void Reject(Action action)
    {
        try { action(); } catch (DomainException) { return; }
        throw new Exception("Expected DomainException");
    }
}
