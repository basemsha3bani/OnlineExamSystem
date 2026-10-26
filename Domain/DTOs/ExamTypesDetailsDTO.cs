using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs
{
    public class ExamTypesDetailsDTO
    {
        [System.Text.Json.Serialization.JsonInclude]
        public int Id { get; private set; }
       
        [System.Text.Json.Serialization.JsonInclude]
        public int DifficultyLevelId { get; private set; }

        [System.Text.Json.Serialization.JsonInclude]
        public int NumberOfQuestions { get; private set; }



    
        [System.Text.Json.Serialization.JsonConstructor]

        private ExamTypesDetailsDTO() { }
        public static ExamTypesDetailsDTO Define(int Id = default, int DifficultyLevelId = default, int NumberOfQuestions = default)
        {
            var value = new ExamTypesDetailsDTO();
            value.Id = Id;
            value.DifficultyLevelId = DifficultyLevelId;
            value.NumberOfQuestions = NumberOfQuestions;
            return value;
        }
        public void ChangeId( int value) => Id = value;
        public void ChangeDifficultyLevelId( int value) => DifficultyLevelId = value;
        public void ChangeNumberOfQuestions( int value) => NumberOfQuestions = value;
}
}
