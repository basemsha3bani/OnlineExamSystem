using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
using System.Collections.Generic;

namespace DataModel
{
    public class ExamDataModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int StudySubjectId { get; set; }

        public string StudySubjectName { get; set; }
        public double TotalMarks { get; set; }

        public List<ExamSectionsDataModel> Sections { get; set; }
    }
}
