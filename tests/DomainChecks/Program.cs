using Domain.Aggregates;
using Domain.Exceptions;
using Domain.Services;
var subject = new StudySubject(1, "Math", 10);
void Throws<T>(Action action) where T : Exception
{
    try { action(); } catch (T) { return; }
    throw new Exception($"Expected {typeof(T).Name}");
}
ExamSection Section(int id, int chapter, int easy = 3, int hard = 2) => new(id, easy + hard,
    new[] { new SectionChapter(chapter) }, new[] { new SectionDifficulty(Difficulty.Easy, easy), new SectionDifficulty(Difficulty.Hard, hard) }.Where(x => x.Count > 0));
var service = new ExamDomainService();
Throws<InvalidChapterException>(() => new Question(1, subject, 11, Difficulty.Easy));
Throws<InvalidChapterException>(() => new Exam(1, subject, new[] { Section(1, 0) }));
var insufficient = Enumerable.Range(1, 2).Select(i => new Question(i, subject, 1, Difficulty.Easy)).ToList();
Throws<NotEnoughQuestionsException>(() => service.BuildExamAttemptQuestions(new Exam(1, subject, new[] { Section(1, 1, 5, 0) }), insufficient));
var pool = Enumerable.Range(1, 10).Select(i => new Question(i, subject, i <= 5 ? 1 : 2, (i - 1) % 5 < 3 ? Difficulty.Easy : Difficulty.Hard)).ToList();
var exam = new Exam(1, subject, new[] { Section(1, 1), Section(2, 2) });
var selected = service.BuildExamAttemptQuestions(exam, pool);
if (selected.Count != 10 || selected.Select(q => q.Id).Distinct().Count() != 10) throw new Exception("Wrong selection count");
for (int i = 0; i < 2; i++)
{
    var part = selected.Skip(i * 5).Take(5).ToList();
    if (part.Any(q => q.ChapterNumber != i + 1) || part.Count(q => q.Difficulty == Difficulty.Easy) != 3 || part.Count(q => q.Difficulty == Difficulty.Hard) != 2) throw new Exception("Wrong section selection");
}
Throws<DomainException>(() => new ExamSection(1, 5, new[] { new SectionChapter(1) }, new[] { new SectionDifficulty(Difficulty.Easy, 4) }));
Throws<NotEnoughQuestionsException>(() => service.BuildExamAttemptQuestions(new Exam(1, subject, new[] { Section(1, 1, 2, 0), Section(2, 1, 2, 0) }), insufficient));
var overlap = new Exam(1, subject, new[] {
    new ExamSection(1, 1, new[] { new SectionChapter(1), new SectionChapter(2) }, new[] { new SectionDifficulty(Difficulty.Easy, 1) }), Section(2, 1, 1, 0) });
var overlapResult = service.BuildExamAttemptQuestions(overlap, new List<Question> { new(1, subject, 1, Difficulty.Easy), new(2, subject, 2, Difficulty.Easy) });
if (overlapResult[0].ChapterNumber != 2 || overlapResult[1].ChapterNumber != 1) throw new Exception("Overlapping chapter matching failed");
var mediumHardSection = new ExamSection(1, 5, new[] { new SectionChapter(1) }, new[] {
    new SectionDifficulty(Difficulty.Medium, 3), new SectionDifficulty(Difficulty.Hard, 2) });
var mediumHardPool = Enumerable.Range(1, 5).Select(i => new Question(i, subject, 1, i <= 3 ? Difficulty.Medium : Difficulty.Hard)).ToList();
var mediumHard = service.BuildExamAttemptQuestions(new Exam(1, subject, new[] { mediumHardSection }), mediumHardPool);
if (mediumHard.Count(q => q.Difficulty == Difficulty.Medium) != 3 || mediumHard.Count(q => q.Difficulty == Difficulty.Hard) != 2) throw new Exception("Medium/Hard distribution was not respected");
ExamConfiguration.ValidateExamDraft("New exam");
ExamConfiguration.ValidateSectionName("New section");
ExamConfiguration.ValidateDifficultyDraft(5, new[] { new SectionDifficulty(Difficulty.Medium, 3) }, new SectionDifficulty(Difficulty.Hard, 2));
Throws<DomainException>(() => ExamConfiguration.ValidateDifficultyDraft(5, new[] { new SectionDifficulty(Difficulty.Medium, 3) }, new SectionDifficulty(Difficulty.Hard, 3)));
Throws<DomainException>(() => ExamConfiguration.ValidateDifficultyDraft(5, new[] { new SectionDifficulty(Difficulty.Medium, 3) }, new SectionDifficulty(Difficulty.Medium, 2)));
Throws<DomainException>(() => ExamConfiguration.ValidateAllocation(4, new[] { new SectionDifficulty(Difficulty.Medium, 3), new SectionDifficulty(Difficulty.Hard, 2) }));
Console.WriteLine("Passed all 4 acceptance scenarios and 4 additional invariant/distribution checks.");
