using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs
{
    public class DifficultyLevelsDTO
    {
        [System.Text.Json.Serialization.JsonInclude]
        public int Id { get; private set; }

        [System.Text.Json.Serialization.JsonInclude]
        public String DifficultyLevelName { get; private set; }
    
        [System.Text.Json.Serialization.JsonConstructor]

        private DifficultyLevelsDTO() { }
        public static DifficultyLevelsDTO Define(int Id = default, String DifficultyLevelName = default)
        {
            var value = new DifficultyLevelsDTO();
            value.Id = Id;
            value.DifficultyLevelName = DifficultyLevelName;
            return value;
        }
        public void ChangeId( int value) => Id = value;
        public void ChangeDifficultyLevelName( String value) => DifficultyLevelName = value;
}
}
