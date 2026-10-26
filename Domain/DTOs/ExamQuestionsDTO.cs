using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs
{
    public class ExamQuestionsDTO
    {
        [System.Text.Json.Serialization.JsonInclude]
        public int Id { get; private set; }

        [System.Text.Json.Serialization.JsonInclude]
        public int ExamId { get; private set; }
    
        [System.Text.Json.Serialization.JsonConstructor]

        private ExamQuestionsDTO() { }
        public static ExamQuestionsDTO Define(int Id = default, int ExamId = default)
        {
            var value = new ExamQuestionsDTO();
            value.Id = Id;
            value.ExamId = ExamId;
            return value;
        }
        public void ChangeId( int value) => Id = value;
        public void ChangeExamId( int value) => ExamId = value;
}
}
