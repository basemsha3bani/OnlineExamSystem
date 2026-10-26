using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs
{
   public class ExamTypesDTO
    {
        [System.Text.Json.Serialization.JsonInclude]
        public int Id { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public String ExamTypeName { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public int DifficultyLevelId { get; private set; }

        [System.Text.Json.Serialization.JsonInclude]
        public int NumberOfQuestions { get; private set; }
    
        [System.Text.Json.Serialization.JsonConstructor]

        private ExamTypesDTO() { }
        public static ExamTypesDTO Define(int Id = default, String ExamTypeName = default, int DifficultyLevelId = default, int NumberOfQuestions = default)
        {
            var value = new ExamTypesDTO();
            value.Id = Id;
            value.ExamTypeName = ExamTypeName;
            value.DifficultyLevelId = DifficultyLevelId;
            value.NumberOfQuestions = NumberOfQuestions;
            return value;
        }
        public void ChangeId( int value) => Id = value;
        public void ChangeExamTypeName( String value) => ExamTypeName = value;
        public void ChangeDifficultyLevelId( int value) => DifficultyLevelId = value;
        public void ChangeNumberOfQuestions( int value) => NumberOfQuestions = value;
}
}
