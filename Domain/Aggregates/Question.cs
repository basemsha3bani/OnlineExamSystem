using Domain.Exceptions;
namespace Domain.Aggregates;
// IDs map to the application's configurable difficulty catalogue.
public readonly record struct Difficulty
{
    public int Id { get; }
    public Difficulty(int id)
    {
        if (id < 1) throw new DomainException("Difficulty must be positive.");
        Id = id;
    }
    public static Difficulty Easy => new(1);
    public static Difficulty Medium => new(2);
    public static Difficulty Hard => new(3);
}
public sealed class Question
{
    public int Id { get; }
    public int SubjectId { get; }
    public int ChapterNumber { get; }
    public Difficulty Difficulty { get; }
    public Question(int id, StudySubject subject, int chapterNumber, Difficulty difficulty)
    {
        ArgumentNullException.ThrowIfNull(subject);
        subject.ValidateChapter(chapterNumber);
        if (difficulty.Id < 1) throw new DomainException("Difficulty is required.");
        Id = id; SubjectId = subject.Id; ChapterNumber = chapterNumber; Difficulty = difficulty;
    }
}
