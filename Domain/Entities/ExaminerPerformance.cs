using Domain.Aggregates;
using System;

namespace Domain.Entities
{
    public class ExaminerPerformance
    {
        public int UserId { get; private set; }

        public int TotalAttempts { get; private set; }
        public int EvaluatedCount { get; private set; }
        public int InProgressCount { get; private set; }

        public double AverageScore { get; private set; }
        public double HighestScore { get; private set; }
        public double LowestScore { get; private set; }

        public double PassRate { get; private set; } // evaluated >= passing

        public DateTime LastCalculatedAt { get; private set; }
        public int LastCalculatedAttemptId { get; private set; } // for idempotency

        public virtual User User { get; private set; }

        private ExaminerPerformance() { }
        public static ExaminerPerformance Define(int UserId = default, int TotalAttempts = default, int EvaluatedCount = default, int InProgressCount = default, double AverageScore = default, double HighestScore = default, double LowestScore = default, double PassRate = default, DateTime LastCalculatedAt = default, int LastCalculatedAttemptId = default, User User = default)
        {
            var value = new ExaminerPerformance();
            value.ChangeUserId(UserId);
            value.ChangeTotalAttempts(TotalAttempts);
            value.ChangeEvaluatedCount(EvaluatedCount);
            value.ChangeInProgressCount(InProgressCount);
            value.ChangeAverageScore(AverageScore);
            value.ChangeHighestScore(HighestScore);
            value.ChangeLowestScore(LowestScore);
            value.ChangePassRate(PassRate);
            value.ChangeLastCalculatedAt(LastCalculatedAt);
            value.ChangeLastCalculatedAttemptId(LastCalculatedAttemptId);
            value.ChangeUser(User);
            return value;
        }
        public void ChangeUserId(int value) => UserId = EntityRules.NonNegative(value, "UserId");
        public void ChangeTotalAttempts(int value) => TotalAttempts = value;
        public void ChangeEvaluatedCount(int value) => EvaluatedCount = value;
        public void ChangeInProgressCount(int value) => InProgressCount = value;
        public void ChangeAverageScore(double value) => AverageScore = value;
        public void ChangeHighestScore(double value) => HighestScore = value;
        public void ChangeLowestScore(double value) => LowestScore = value;
        public void ChangePassRate(double value) => PassRate = value;
        public void ChangeLastCalculatedAt(DateTime value) => LastCalculatedAt = value;
        public void ChangeLastCalculatedAttemptId(int value) => LastCalculatedAttemptId = EntityRules.NonNegative(value, "LastCalculatedAttemptId");
        public void ChangeUser(User value) => User = value;

        public void Recalculate(int userId, int triggerAttemptId, IEnumerable<ExamAttempt> source)
        {
            EntityRules.Positive(userId, "User");
            var attempts = source.Where(a => a.UserId == userId).ToList();
            var scored = attempts.Where(a => a.Status == "Scored" && a.Score.HasValue).ToList();
            UserId = userId;
            TotalAttempts = attempts.Count; EvaluatedCount = scored.Count;
            InProgressCount = attempts.Count(a => a.Status == "InProgress");
            AverageScore = scored.Count == 0 ? 0 : (double)scored.Average(a => a.Score.Value) * 100;
            HighestScore = scored.Count == 0 ? 0 : (double)scored.Max(a => a.Score.Value);
            LowestScore = scored.Count == 0 ? 0 : (double)scored.Min(a => a.Score.Value);
            PassRate = scored.Count == 0 ? 0 : 100d * scored.Count(a => a.Score >= .5m) / scored.Count;
            LastCalculatedAt = DateTime.UtcNow; LastCalculatedAttemptId = triggerAttemptId;
        }

    }
}
