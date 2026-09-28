namespace DataModel
{
    public class ExamSectionRulesDataModel
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        
        public int DifficultyLevelId { get; set; }
        public string DifficultyLevel { get; set; }

        public string subjectName { get; set; }
        public int NoOfQuestions { get; set; }
      }
}
