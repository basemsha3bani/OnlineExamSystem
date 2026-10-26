using Domain.Abstractions;
using Domain.Aggregates;
using Domain.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Services;

EntityChecks.Run();
var roots = typeof(Exam).Assembly.GetTypes().Where(t => t.IsClass && typeof(IAggregateRoot).IsAssignableFrom(t)).Select(t => t.Name).OrderBy(n => n).ToArray();
if (!roots.SequenceEqual(new[] { "Exam", "ExamAttempt", "StudySubject" })) throw new Exception("The aggregate roots must be StudySubject, Exam, and ExamAttempt.");
var subject = StudySubject.Define(Id: 1, SubjectName: "Math", NumberOfChapters: 10);
const int Easy = 1, Medium = 2, Hard = 3;
void Throws<T>(Action action) where T : Exception
{
    try { action(); } catch (T) { return; }
    throw new Exception($"Expected {typeof(T).Name}");
}
Questions Question(int id, int chapter, int difficulty) => Questions.Define(Id: id, QuestionText: "Question", StudySubjectId: subject.Id, ChapterNumber: chapter, DifficultyLevelId: difficulty);
ExamSections Section(int id, int chapter, int easy = 3, int hard = 2) => ExamSections.Define(Id: id, SectionName: "Section", NumberOfQuestions: easy + hard,
    ChapterNumbers: new() { chapter }, examSectionRules: new[] { (Difficulty: Easy, Count: easy), (Difficulty: Hard, Count: hard) }
        .Where(r => r.Count > 0).Select(r => ExamSectionRules.Define(DifficultyLevelId: r.Difficulty, NoOfQuestions: r.Count)).ToList());
Exam Definition(params ExamSections[] sections) => Exam.Define(Id: 1, Title: "Exam", StudySubjectId: subject.Id, StudySubject: subject, Sections: sections.ToList());
var service = new ExamDomainService();
Throws<InvalidChapterException>(() => Question(1, 11, Easy).ValidateForSubject(subject));
Throws<InvalidChapterException>(() => Definition(Section(1, 0)).ValidateForAttempt());
var insufficient = Enumerable.Range(1, 2).Select(i => Question(i, 1, Easy)).ToList();
Throws<NotEnoughQuestionsException>(() => service.BuildExamAttemptQuestions(Definition(Section(1, 1, 5, 0)), insufficient));
var pool = Enumerable.Range(1, 10).Select(i => Question(i, i <= 5 ? 1 : 2, (i - 1) % 5 < 3 ? Easy : Hard)).ToList();
var selected = service.BuildExamAttemptQuestions(Definition(Section(1, 1), Section(2, 2)), pool);
if (selected.Count != 10 || selected.Select(q => q.Id).Distinct().Count() != 10) throw new Exception("Wrong selection count");
for (int i = 0; i < 2; i++)
{
    var part = selected.Skip(i * 5).Take(5).ToList();
    if (part.Any(q => q.ChapterNumber != i + 1) || part.Count(q => q.DifficultyLevelId == Easy) != 3 || part.Count(q => q.DifficultyLevelId == Hard) != 2)
        throw new Exception("Wrong section selection");
}
var incomplete = Section(1, 1, 4, 0); incomplete.ChangeNumberOfQuestions(5);
Throws<DomainException>(() => incomplete.ValidateForAttempt(subject));
Throws<NotEnoughQuestionsException>(() => service.BuildExamAttemptQuestions(Definition(Section(1, 1, 2, 0), Section(2, 1, 2, 0)), insufficient));
var broad = Section(1, 1, 1, 0); broad.ChangeChapterNumbers(new() { 1, 2 });
var overlap = service.BuildExamAttemptQuestions(Definition(broad, Section(2, 1, 1, 0)), new() { Question(1, 1, Easy), Question(2, 2, Easy) });
if (overlap[0].ChapterNumber != 2 || overlap[1].ChapterNumber != 1) throw new Exception("Overlapping chapter matching failed");
var mediumHardSection = ExamSections.Define(Id: 1, SectionName: "Medium/Hard", NumberOfQuestions: 5, ChapterNumbers: new() { 1 }, examSectionRules: new() {
    ExamSectionRules.Define(DifficultyLevelId: Medium, NoOfQuestions: 3), ExamSectionRules.Define(DifficultyLevelId: Hard, NoOfQuestions: 2) });
var mediumHard = service.BuildExamAttemptQuestions(Definition(mediumHardSection), Enumerable.Range(1, 5).Select(i => Question(i, 1, i <= 3 ? Medium : Hard)).ToList());
if (mediumHard.Count(q => q.DifficultyLevelId == Medium) != 3 || mediumHard.Count(q => q.DifficultyLevelId == Hard) != 2) throw new Exception("Medium/Hard distribution was not respected");
Exam.Define(Title: "New exam");
ExamSections.Define(SectionName: "New section");
var rules = new[] { ExamSectionRulesDTO.Define(DifficultyLevelId: Medium, NoOfQuestions: 3), ExamSectionRulesDTO.Define(DifficultyLevelId: Hard, NoOfQuestions: 2) };
ExamSectionRules.CheckAllocation(5, rules);
Throws<DomainException>(() => ExamSectionRules.CheckAllocation(4, rules));
Throws<DomainException>(() => ExamSectionRules.CheckAllocation(5, new[] { rules[0], rules[0] }));
Console.WriteLine("Passed aggregate-root classification, chapter validation, unique question selection, shortages, overlapping chapters, difficulty distribution, and allocation checks.");
