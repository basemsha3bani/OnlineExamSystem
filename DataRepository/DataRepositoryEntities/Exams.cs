namespace DataRepository.DataRepositoryEntities
{ 
    public class Exams:IRepository
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int StudySubjectId { get; set; }

        public double TotalMarks { get; set; }
    }
}
