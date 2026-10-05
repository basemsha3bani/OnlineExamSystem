using System;
using System.ComponentModel.DataAnnotations;

namespace DataRepository.DataRepositoryEntities
{
    public class ExaminerPerformance
    {
        [Key]
        public int ExaminerId { get; set; }

        public int TotalAttempts { get; set; }
        public int EvaluatedCount { get; set; }
        public int InProgressCount { get; set; }

        public double AverageScore { get; set; }
        public double HighestScore { get; set; }
        public double LowestScore { get; set; }

        public double PassRate { get; set; } // evaluated >= passing

        public DateTime LastCalculatedAt { get; set; }
        public int LastCalculatedAttemptId { get; set; } // for idempotency
    }
}
