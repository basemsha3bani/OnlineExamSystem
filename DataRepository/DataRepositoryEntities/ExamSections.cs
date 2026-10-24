
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class ExamSections:IRepository
    {
        public int Id { get; set; }
        public int NumberOfQuestions { get; set; }
        public List<int> ChapterNumbers { get; set; } = new List<int>();
        [ForeignKey("Exam")]
        public int ExamId { get; set; }
        public string SectionName { get; set; }

        public virtual Exams Exam { get; set; }

        public List<ExamSectionRules> examSectionRules { get; set; }


    }

}
