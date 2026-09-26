using DataRepository.ModelMapper.Interface;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class ExamSections:IRepository
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string SectionName { get; set; }
        public int Percentage { get; set; }

      
    }

}
