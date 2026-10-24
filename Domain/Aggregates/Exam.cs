using Domain.Exceptions;
namespace Domain.Aggregates;
public sealed record SectionChapter(int ChapterNumber);
public sealed record SectionDifficulty(Difficulty Difficulty, int Count);
public sealed class ExamSection
{
    public int Id { get; }
    public int NumberOfQuestions { get; }
    public IReadOnlyList<SectionChapter> SectionChapters { get; }
    public IReadOnlyList<SectionDifficulty> SectionDifficulties { get; }
    public ExamSection(int id, int numberOfQuestions, IEnumerable<SectionChapter> chapters, IEnumerable<SectionDifficulty> difficulties)
    {
        ArgumentNullException.ThrowIfNull(chapters); ArgumentNullException.ThrowIfNull(difficulties);
        var c = chapters.ToList(); var d = difficulties.ToList();
        if (numberOfQuestions < 1 || c.Count == 0 || d.Count == 0 || c.Any(x => x is null) || d.Any(x => x is null || x.Count < 1 || x.Difficulty.Id < 1))
            throw new DomainException("Sections require chapters and positive difficulty counts.");
        if (d.Sum(x => (long)x.Count) != numberOfQuestions)
            throw new DomainException("Difficulty counts must equal the section question count.");
        if (c.Distinct().Count() != c.Count || d.Select(x => x.Difficulty).Distinct().Count() != d.Count)
            throw new DomainException("Section chapters and difficulties must be unique.");
        Id = id; NumberOfQuestions = numberOfQuestions;
        SectionChapters = c.AsReadOnly(); SectionDifficulties = d.AsReadOnly();
    }
}
public sealed class Exam
{
    public int Id { get; }
    public int SubjectId => Subject.Id;
    public StudySubject Subject { get; }
    public IReadOnlyList<ExamSection> Sections { get; }
    public Exam(int id, StudySubject subject, IEnumerable<ExamSection> sections)
    {
        ArgumentNullException.ThrowIfNull(subject); ArgumentNullException.ThrowIfNull(sections);
        var items = sections.ToList();
        foreach (var section in items)
        {
            if (section is null) throw new DomainException("Section is required.");
            foreach (var chapter in section.SectionChapters) subject.ValidateChapter(chapter.ChapterNumber);
        }
        Id = id; Subject = subject; Sections = items.AsReadOnly();
    }
}
