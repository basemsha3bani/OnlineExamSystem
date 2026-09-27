namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class ExamSectionRules
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public int DifficultyLevelId { get; set; }
        public int NoOfQuestions { get; set; }
        public ExamSectionRules()
        {
        }
    }
}
