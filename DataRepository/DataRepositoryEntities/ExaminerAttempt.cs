using System;
using System.ComponentModel.DataAnnotations;

namespace DataRepository.DataRepositoryEntities
{
    // The snapshot retains the exact sections, questions, options and answer key at start time.
    public class ExaminerAttempt
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int UserId { get; set; }
        public string ExamTitle { get; set; }
        public string SnapshotJson { get; set; }
        public string Status { get; set; } = "InProgress";
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? EvaluatedAt { get; set; }
        public int TotalQuestions { get; set; }
        public int? CorrectQuestions { get; set; }
        public decimal? Score { get; set; }
        [Timestamp]
        public byte[] Version { get; set; }
    }
}
