using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataRepository.DataRepositoryEntities
{ 
    public class Exams:IRepository
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [ForeignKey("StudySubject")]
        public int StudySubjectId { get; set; }

        public double TotalMarks { get; set; }

        public StudySubject StudySubject{ get; set; }

        public List<ExamSections> Sections { get; set; }
    }
}
