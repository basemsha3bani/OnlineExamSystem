namespace Domain.DTOs
{
    public class ExamSectionRulesDTO
    {
        [System.Text.Json.Serialization.JsonInclude]
        public int Id { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public int SectionId { get; private set; }
        
        [System.Text.Json.Serialization.JsonInclude]
        public int DifficultyLevelId { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public string DifficultyLevel { get; private set; }

        [System.Text.Json.Serialization.JsonInclude]
        public string subjectName { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public int NoOfQuestions { get; private set; }
      
        [System.Text.Json.Serialization.JsonConstructor]

        private ExamSectionRulesDTO() { }
        public static ExamSectionRulesDTO Define(int Id = default, int SectionId = default, int DifficultyLevelId = default, string DifficultyLevel = default, string subjectName = default, int NoOfQuestions = default)
        {
            var value = new ExamSectionRulesDTO();
            value.Id = Id;
            value.SectionId = SectionId;
            value.DifficultyLevelId = DifficultyLevelId;
            value.DifficultyLevel = DifficultyLevel;
            value.subjectName = subjectName;
            value.NoOfQuestions = NoOfQuestions;
            return value;
        }
        public void ChangeId( int value) => Id = value;
        public void ChangeSectionId( int value) => SectionId = value;
        public void ChangeDifficultyLevelId( int value) => DifficultyLevelId = value;
        public void ChangeDifficultyLevel( string value) => DifficultyLevel = value;
        public void ChangesubjectName( string value) => subjectName = value;
        public void ChangeNoOfQuestions( int value) => NoOfQuestions = value;
}
}
