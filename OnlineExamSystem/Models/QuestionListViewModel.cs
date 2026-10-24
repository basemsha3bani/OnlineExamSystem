using DataModel;
using System.Collections.Generic;
namespace OnlineExamSystem.Models
{
    public class QuestionListViewModel
    {
        public QuestionPage Results { get; set; }
        public List<StudySubjectDataModel> Subjects { get; set; }
        public IEnumerable<int> Chapters { get; set; }
        public int? StudySubjectId => Results.Search.StudySubjectId;
        public int? ChapterNumber => Results.Search.ChapterNumber;
        public int PageSize => Results.Search.PageSize;
    }
}
