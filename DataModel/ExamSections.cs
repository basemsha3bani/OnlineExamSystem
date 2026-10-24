using System.Collections.Generic;

namespace  DataModel
{
    public class ExamSectionsDataModel
    {
        public int Id { get; set; }
        public int NumberOfQuestions { get; set; }
        public List<int> ChapterNumbers { get; set; } = new List<int>();
        public int ExamId { get; set; }
        public string SectionName { get; set; }
        public ExamDataModel exam { get; set; } = new ExamDataModel();
        public List<ExamSectionRulesDataModel> ExamSectionRules { get; set; } = new List<ExamSectionRulesDataModel>();

    }
}
