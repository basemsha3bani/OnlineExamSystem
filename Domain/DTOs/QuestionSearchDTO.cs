using System;
using System.Collections.Generic;
namespace Domain.DTOs
{
    public sealed class QuestionSearchDTO
    {
        public int? StudySubjectId { get; }
        public int? ChapterNumber { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        private QuestionSearchDTO() { }
        public static QuestionSearchDTO Define(int? studySubjectId = null, int? chapterNumber = null, int pageNumber = 1, int pageSize = 20)
            => new QuestionSearchDTO(studySubjectId, chapterNumber, pageNumber, pageSize);
        [System.Text.Json.Serialization.JsonConstructor]
        private QuestionSearchDTO(int? studySubjectId, int? chapterNumber, int pageNumber, int pageSize)
        {
            StudySubjectId = studySubjectId > 0 ? studySubjectId : null;
            ChapterNumber = chapterNumber > 0 ? chapterNumber : null;
            PageNumber = Math.Max(1, pageNumber);
            PageSize = pageSize == 10 || pageSize == 20 || pageSize == 50 ? pageSize : 20;
        }
        public int PageFor(int totalCount) => Math.Min(PageNumber, Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize)));
    }
    public sealed class QuestionPageDTO
    {
        [System.Text.Json.Serialization.JsonInclude]
        public QuestionSearchDTO Search { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public List<QuestionsDTO> Items { get; private set; } = new List<QuestionsDTO>();
        [System.Text.Json.Serialization.JsonInclude]
        public int TotalCount { get; private set; }
        public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)Search.PageSize));
    
        [System.Text.Json.Serialization.JsonConstructor]

        private QuestionPageDTO() { }
        public static QuestionPageDTO Define(QuestionSearchDTO Search = default, List<QuestionsDTO> Items = default, int TotalCount = default)
        {
            var value = new QuestionPageDTO();
            value.Search = Search;
            value.Items = Items ?? new List<QuestionsDTO>();
            value.TotalCount = TotalCount;
            return value;
        }
        public void ChangeSearch( QuestionSearchDTO value) => Search = value;
        public void ChangeItems( List<QuestionsDTO> value) => Items = value;
        public void ChangeTotalCount( int value) => TotalCount = value;
}
}
