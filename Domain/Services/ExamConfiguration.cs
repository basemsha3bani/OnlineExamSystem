using Domain.Aggregates;
using Domain.Exceptions;
namespace Domain.Services;
public static class ExamConfiguration
{
    public static void ValidateExamDraft(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new DomainException("Exam title is required.");
    }
    public static void ValidateSectionName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Section name is required.");
    }
    public static void ValidateAllocation(int total, IEnumerable<SectionDifficulty> requirements)
    {
        var rules = requirements.ToList();
        if (rules.Any(r => r.Count < 1 || r.Difficulty.Id < 1)) throw new DomainException("Difficulty and count must be positive.");
        if (rules.Select(r => r.Difficulty).Distinct().Count() != rules.Count) throw new DomainException("This difficulty is already configured.");
        if (rules.Sum(r => (long)r.Count) > total) throw new DomainException("Difficulty counts exceed the section question count.");
    }
    public static void ValidateDifficultyDraft(int total, IEnumerable<SectionDifficulty> existing, SectionDifficulty incoming)
    {
        if (incoming.Count < 1 || incoming.Difficulty.Id < 1) throw new DomainException("Difficulty and count must be positive.");
        var requirements = existing.ToList();
        if (requirements.Any(d => d.Difficulty == incoming.Difficulty)) throw new DomainException("This difficulty is already configured.");
        if (requirements.Sum(d => (long)d.Count) + incoming.Count > total) throw new DomainException("Difficulty counts exceed the section question count.");
    }
    // Sections may be saved as drafts before individual difficulty rules are added.
    // The complete aggregate enforces the exact total before an attempt is built.
    public static void ValidateSectionDraft(StudySubject subject, string name, int count, IEnumerable<int> chapters)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Section name is required.");
        if (count < 1 || chapters == null || !chapters.Any()) throw new DomainException("A section requires a positive question count and chapters.");
        foreach (var chapter in chapters) subject.ValidateChapter(chapter);
    }
}
