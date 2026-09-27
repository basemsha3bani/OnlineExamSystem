
using System.Collections.Generic;

namespace DataModel
{
    public class ExamDataModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int StudySubjectId { get; set; }

        public string StudySubjectName { get; set; }
        public decimal TotalMarks { get; set; }

        public List<ExamSectionsDataModel> Sections { get; set; } = new List<ExamSectionsDataModel>();
    }
}
