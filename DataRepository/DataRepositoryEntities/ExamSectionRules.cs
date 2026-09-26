namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class ExamSectionRules
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public int DifficultyLevelId { get; set; }
        public int NoOfQuestions { get; set; }

        public ExamSectionRules(int Id_, int SectionId_, int DifficultyLevelId_, int NoOfQuestions_)
        {
            this.Id = Id_;
            this.SectionId = SectionId_;
            this.DifficultyLevelId = DifficultyLevelId_;
            this.NoOfQuestions = NoOfQuestions_;
        }
    }
}
