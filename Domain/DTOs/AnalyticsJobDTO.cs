namespace Domain.DTOs
{
    public record AnalyticsJobDTO
    {
        [System.Text.Json.Serialization.JsonInclude]
        public int ExaminerId { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public int TriggerAttemptId { get; private set; }
    
        [System.Text.Json.Serialization.JsonConstructor]

        private AnalyticsJobDTO() { }
        public static AnalyticsJobDTO Define(int ExaminerId = default, int TriggerAttemptId = default)
        {
            var value = new AnalyticsJobDTO();
            value.ExaminerId = ExaminerId;
            value.TriggerAttemptId = TriggerAttemptId;
            return value;
        }
        public void ChangeExaminerId( int value) => ExaminerId = value;
        public void ChangeTriggerAttemptId( int value) => TriggerAttemptId = value;
}
}
