namespace DataModel
{
    public record AnalyticsJob
    {
        public int ExaminerId { get; set; }
        public int TriggerAttemptId { get; set; }
    }
}
