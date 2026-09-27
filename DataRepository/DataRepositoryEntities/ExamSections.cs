using DataRepository.ModelMapper.Interface;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class ExamSections:IRepository
    {
        public int Id { get; set; }
        [ForeignKey("Exam")]
        public int ExamId { get; set; }
        public string SectionName { get; set; }
        public decimal Percentage { get; set; }

        public Exams Exam { get; set; }

      
    }

}
