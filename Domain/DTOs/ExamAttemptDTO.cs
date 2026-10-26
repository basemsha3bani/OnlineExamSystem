using System;

namespace Domain.DTOs
{
    public class ExamAttemptDTO
    {
        [System.Text.Json.Serialization.JsonInclude]
        public int Id { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public int ExamId { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public int UserId { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public string ExamTitle { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public string SnapshotJson { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public string Status { get; private set; } = "InProgress";
        [System.Text.Json.Serialization.JsonInclude]
        public DateTime StartedAt { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public DateTime? SubmittedAt { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public DateTime? EvaluatedAt { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public int TotalQuestions { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public int? CorrectQuestions { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public decimal? Score { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public byte[] Version { get; private set; }
    
        [System.Text.Json.Serialization.JsonConstructor]

        private ExamAttemptDTO() { }
        public static ExamAttemptDTO Define(int Id = default, int ExamId = default, int UserId = default, string ExamTitle = default, string SnapshotJson = default, string Status = "InProgress", DateTime StartedAt = default, DateTime? SubmittedAt = default, DateTime? EvaluatedAt = default, int TotalQuestions = default, int? CorrectQuestions = default, decimal? Score = default, byte[] Version = default)
        {
            var value = new ExamAttemptDTO();
            value.Id = Id;
            value.ExamId = ExamId;
            value.UserId = UserId;
            value.ExamTitle = ExamTitle;
            value.SnapshotJson = SnapshotJson;
            value.Status = Status;
            value.StartedAt = StartedAt;
            value.SubmittedAt = SubmittedAt;
            value.EvaluatedAt = EvaluatedAt;
            value.TotalQuestions = TotalQuestions;
            value.CorrectQuestions = CorrectQuestions;
            value.Score = Score;
            value.Version = Version;
            return value;
        }
        public void ChangeId( int value) => Id = value;
        public void ChangeExamId( int value) => ExamId = value;
        public void ChangeUserId( int value) => UserId = value;
        public void ChangeExamTitle( string value) => ExamTitle = value;
        public void ChangeSnapshotJson( string value) => SnapshotJson = value;
        public void ChangeStatus( string value) => Status = value;
        public void ChangeStartedAt( DateTime value) => StartedAt = value;
        public void ChangeSubmittedAt( DateTime? value) => SubmittedAt = value;
        public void ChangeEvaluatedAt( DateTime? value) => EvaluatedAt = value;
        public void ChangeTotalQuestions( int value) => TotalQuestions = value;
        public void ChangeCorrectQuestions( int? value) => CorrectQuestions = value;
        public void ChangeScore( decimal? value) => Score = value;
        public void ChangeVersion( byte[] value) => Version = value;
}
}
