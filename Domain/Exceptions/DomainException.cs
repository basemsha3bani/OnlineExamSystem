namespace Domain.Exceptions;
public class DomainException(string message) : InvalidOperationException(message);
public sealed class InvalidChapterException(int chapter, int maximum)
    : DomainException($"Chapter {chapter} must be between 1 and {maximum}.");
public sealed class NotEnoughQuestionsException(int section, int difficulty, int required, int available)
    : DomainException($"Section {section} requires {required} questions of difficulty {difficulty}, but only {available} are available.");
