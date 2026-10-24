using Domain.Exceptions;
namespace Domain.Aggregates;
public sealed class StudySubject
{
    public int Id { get; }
    public string Name { get; }
    public int NumberOfChapters { get; }
    public StudySubject(int id, string name, int numberOfChapters)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Subject name is required.");
        if (numberOfChapters < 1) throw new DomainException("A subject requires at least one chapter.");
        Id = id; Name = name; NumberOfChapters = numberOfChapters;
    }
    public void ValidateChapter(int chapter)
    {
        if (chapter < 1 || chapter > NumberOfChapters) throw new InvalidChapterException(chapter, NumberOfChapters);
    }
}
