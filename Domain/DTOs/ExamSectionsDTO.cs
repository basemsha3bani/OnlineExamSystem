using System.Collections.Generic;

namespace Domain.DTOs
{
    public class ExamSectionsDTO
    {
        [System.Text.Json.Serialization.JsonInclude]
        public int Id { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public int NumberOfQuestions { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public List<int> ChapterNumbers { get; private set; } = new List<int>();
        [System.Text.Json.Serialization.JsonInclude]
        public int ExamId { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public string SectionName { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public ExamDTO exam { get; private set; } = ExamDTO.Define();
        [System.Text.Json.Serialization.JsonInclude]
        public List<ExamSectionRulesDTO> ExamSectionRules { get; private set; } = new List<ExamSectionRulesDTO>();

    
        [System.Text.Json.Serialization.JsonConstructor]

        private ExamSectionsDTO() { }
        public static ExamSectionsDTO Define(int Id = default, int NumberOfQuestions = default, List<int> ChapterNumbers = default, int ExamId = default, string SectionName = default, ExamDTO exam = default, List<ExamSectionRulesDTO> ExamSectionRules = default)
        {
            var value = new ExamSectionsDTO();
            value.Id = Id;
            value.NumberOfQuestions = NumberOfQuestions;
            value.ChapterNumbers = ChapterNumbers ?? new List<int>();
            value.ExamId = ExamId;
            value.SectionName = SectionName;
            value.exam = exam ?? ExamDTO.Define();
            value.ExamSectionRules = ExamSectionRules ?? new List<ExamSectionRulesDTO>();
            return value;
        }
        public void ChangeId( int value) => Id = value;
        public void ChangeNumberOfQuestions( int value) => NumberOfQuestions = value;
        public void ChangeChapterNumbers( List<int> value) => ChapterNumbers = value;
        public void ChangeExamId( int value) => ExamId = value;
        public void ChangeSectionName( string value) => SectionName = value;
        public void Changeexam( ExamDTO value) => exam = value;
        public void ChangeExamSectionRules( List<ExamSectionRulesDTO> value) => ExamSectionRules = value;
}
}
