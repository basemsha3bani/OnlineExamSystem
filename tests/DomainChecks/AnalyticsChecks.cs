using Domain.Aggregates;
using Domain.Entities;
internal static class AnalyticsChecks
{
    public static void Run()
    {
// Each repeat attempt contributes once, regardless of exam identity or question count.
var performanceAttempts = new[] {
    ExamAttempt.Define(Id: 1, ExamId: 1, UserId: 1, Status: "Scored", Score: .6m),
    ExamAttempt.Define(Id: 2, ExamId: 1, UserId: 1, Status: "Scored", Score: .7m),
    ExamAttempt.Define(Id: 3, ExamId: 2, UserId: 1, Status: "Scored", Score: .8m),
    ExamAttempt.Define(Id: 4, ExamId: 3, UserId: 1, Status: "Scored", Score: .9m),
    ExamAttempt.Define(Id: 5, ExamId: 1, UserId: 2, Status: "Scored", Score: 0m),
    ExamAttempt.Define(Id: 6, ExamId: 1, UserId: 1, Status: "InProgress"),
    ExamAttempt.Define(Id: 7, ExamId: 1, UserId: 1, Status: "Submitted")
};
var subjectByExam = new Dictionary<int, int> { [1] = 1, [2] = 1, [3] = 2 };
var summaries = performanceAttempts.GroupBy(a => subjectByExam[a.ExamId]).Select(group => {
    var result = ExaminerPerformance.Define();
    result.ChangeSubjectId(group.Key);
    result.Recalculate(1, 7, group);
    return result;
}).ToDictionary(p => p.SubjectId);
var math = summaries[1];
if (Math.Abs(math.AverageScore - 70) > .00001 || math.EvaluatedCount != 3 || math.TotalAttempts != 5
    || math.InProgressCount != 1 || math.HighestScore != 80 || math.LowestScore != 60)
    throw new Exception("Subject analytics lost repeat attempts, candidate isolation, or percentage units.");
if (summaries[2].AverageScore != 90 || summaries[2].TotalAttempts != 1)
    throw new Exception("Physics summary must be independent of Math.");
math.Recalculate(1, 7, performanceAttempts.Where(a => subjectByExam[a.ExamId] == 1));
if (math.EvaluatedCount != 3) throw new Exception("Replayed analytics jobs must not double count attempts.");
var empty = ExaminerPerformance.Define();
empty.Recalculate(1, 0, Array.Empty<ExamAttempt>());
if (empty.AverageScore != 0 || empty.PassRate != 0) throw new Exception("Empty analytics must be zero.");
Console.WriteLine("Passed subject analytics example, repeat attempts, candidate isolation, pending attempts, replay, and empty summary checks.");

    }
}
