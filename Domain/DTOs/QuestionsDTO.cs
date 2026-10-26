using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs
{
    public class QuestionsDTO
    {
        [System.Text.Json.Serialization.JsonInclude]
        public int Id { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public int ChapterNumber { get; private set; } = 1;
        [System.Text.Json.Serialization.JsonInclude]
        public String QuestionText{ get; private set; }

        [System.Text.Json.Serialization.JsonInclude]
        public int DifficultyLevelId { get; private set; }

        [System.Text.Json.Serialization.JsonInclude]
        public int? StudySubjectId { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public List<QuestionAnswersDTO> QuestionAnswersDTO { get; private set; }

        [System.Text.Json.Serialization.JsonConstructor]

        private QuestionsDTO() { }
        public static QuestionsDTO Define(int Id = default, int ChapterNumber = 1, String QuestionText = default, int DifficultyLevelId = default, int? StudySubjectId = default, List<QuestionAnswersDTO> QuestionAnswersDTO = default)
        {
            var value = new QuestionsDTO();
            value.Id = Id;
            value.ChapterNumber = ChapterNumber;
            value.QuestionText = QuestionText;
            value.DifficultyLevelId = DifficultyLevelId;
            value.StudySubjectId = StudySubjectId;
            value.QuestionAnswersDTO = QuestionAnswersDTO;
            return value;
        }
        public void SelectCorrectAnswer(int index)
        {
            if (QuestionAnswersDTO == null || index < 0 || index >= QuestionAnswersDTO.Count)
                throw new Domain.Exceptions.DomainException("Select a valid correct answer.");
            for (var i = 0; i < QuestionAnswersDTO.Count; i++) QuestionAnswersDTO[i].ChangeIsCorrect(i == index);
        }
        public void ChangeId( int value) => Id = value;
        public void ChangeChapterNumber( int value) => ChapterNumber = value;
        public void ChangeQuestionText( String value) => QuestionText = value;
        public void ChangeDifficultyLevelId( int value) => DifficultyLevelId = value;
        public void ChangeStudySubjectId( int? value) => StudySubjectId = value;
        public void ChangeQuestionAnswersDTO( List<QuestionAnswersDTO> value) => QuestionAnswersDTO = value;
}
}
