using Domain.Entities;
using Domain.Aggregates;
using Domain.Exceptions;
namespace Domain.Services;
public sealed class ExamDomainService
{
    public List<Questions> BuildExamAttemptQuestions(Exam exam, List<Questions> allQuestionsForSubject)
    {
        ArgumentNullException.ThrowIfNull(exam); ArgumentNullException.ThrowIfNull(allQuestionsForSubject);
        exam.ValidateForAttempt();
        var pool = allQuestionsForSubject.Where(q => q.StudySubjectId == exam.StudySubjectId).DistinctBy(q => q.Id).ToList();
        foreach (var question in pool) question.ValidateForSubject(exam.StudySubject);
        var slots = exam.Sections.SelectMany(s => s.examSectionRules.SelectMany(d =>
            Enumerable.Range(0, d.NoOfQuestions).Select(_ => (Section: s, Difficulty: d.DifficultyLevelId)))).ToList();
        var candidates = slots.Select(s => pool.Where(q => q.DifficultyLevelId == s.Difficulty &&
            s.Section.ChapterNumbers.Contains(q.ChapterNumber)).ToList()).ToList();
        // Augmenting paths avoid false shortages when chapter filters overlap.
        var owners = new Dictionary<int, int>();
        bool Assign(int slot, HashSet<int> visited)
        {
            foreach (var q in candidates[slot])
                if (visited.Add(q.Id) && (!owners.TryGetValue(q.Id, out var previous) || Assign(previous, visited)))
                { owners[q.Id] = slot; return true; }
            return false;
        }
        foreach (var slot in Enumerable.Range(0, slots.Count).OrderBy(i => candidates[i].Count))
            if (!Assign(slot, new HashSet<int>()))
            {
                var requirement = slots[slot];
                throw new NotEnoughQuestionsException(requirement.Section.Id, requirement.Difficulty,
                    requirement.Section.examSectionRules.Single(d => d.DifficultyLevelId == requirement.Difficulty).NoOfQuestions,
                    owners.Values.Count(i => ReferenceEquals(slots[i].Section, requirement.Section) && slots[i].Difficulty == requirement.Difficulty));
            }
        var byId = pool.ToDictionary(q => q.Id);
        return owners.OrderBy(p => p.Value).Select(p => byId[p.Key]).ToList();
    }
}
