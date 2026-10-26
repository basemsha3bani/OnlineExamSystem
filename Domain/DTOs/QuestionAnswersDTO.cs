using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs
{
  public  class QuestionAnswersDTO
    {
        [System.Text.Json.Serialization.JsonInclude]
        public int Id { get; private set; }

        [System.Text.Json.Serialization.JsonInclude]
        public int QuestionId { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public String AnswerText { get; private set; }

        [System.Text.Json.Serialization.JsonInclude]
        public bool IsCorrect { get; private set; }  

        [System.Text.Json.Serialization.JsonInclude]
        public string radioButtonDisplay { get; private set; }
    
        [System.Text.Json.Serialization.JsonConstructor]

        private QuestionAnswersDTO() { }
        public static QuestionAnswersDTO Define(int Id = default, int QuestionId = default, String AnswerText = default, bool IsCorrect = default, string radioButtonDisplay = default)
        {
            var value = new QuestionAnswersDTO();
            value.Id = Id;
            value.QuestionId = QuestionId;
            value.AnswerText = AnswerText;
            value.IsCorrect = IsCorrect;
            value.radioButtonDisplay = radioButtonDisplay;
            return value;
        }
        public void ChangeId( int value) => Id = value;
        public void ChangeQuestionId( int value) => QuestionId = value;
        public void ChangeAnswerText( String value) => AnswerText = value;
        public void ChangeIsCorrect( bool value) => IsCorrect = value;
        public void ChangeradioButtonDisplay( string value) => radioButtonDisplay = value;
}
}
