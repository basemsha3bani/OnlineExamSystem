using System;
using System.Collections.Generic;
namespace DataModel
{
    public sealed class QuestionSearch
    {
        public int? StudySubjectId { get; }
        public int? ChapterNumber { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public QuestionSearch(int? studySubjectId = null, int? chapterNumber = null, int pageNumber = 1, int pageSize = 20)
        {
            StudySubjectId = studySubjectId > 0 ? studySubjectId : null;
            ChapterNumber = chapterNumber > 0 ? chapterNumber : null;
            PageNumber = Math.Max(1, pageNumber);
            PageSize = pageSize == 10 || pageSize == 20 || pageSize == 50 ? pageSize : 20;
        }
        public int PageFor(int totalCount) => Math.Min(PageNumber, Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize)));
    }
    public sealed class QuestionPage
    {
        public QuestionSearch Search { get; set; }
        public List<QuestionsDataModel> Items { get; set; } = new List<QuestionsDataModel>();
        public int TotalCount { get; set; }
        public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)Search.PageSize));
    }
}
