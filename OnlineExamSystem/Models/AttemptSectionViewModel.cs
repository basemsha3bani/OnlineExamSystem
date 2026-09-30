using System.Collections.Generic;

namespace OnlineExamSystem.Models
{
    public class AttemptSectionViewModel
    {
        public int AttemptId { get; set; }
        public string ExamTitle { get; set; }
        public int SectionIndex { get; set; }
        public int SectionCount { get; set; }
        public string SectionName { get; set; }
        public List<AttemptQuestionViewModel> Questions { get; set; }
    }
    public class AttemptQuestionViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int? SelectedOptionId { get; set; }
        public Dictionary<int, string> Options { get; set; }
    }
}
