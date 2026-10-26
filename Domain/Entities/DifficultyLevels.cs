using Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class DifficultyLevels
    {
        public int Id { get; private set; }

        public String DifficultyLevelName { get; private set; }

        private DifficultyLevels() { }
        public static DifficultyLevels Define(int Id = default, String DifficultyLevelName = default)
        {
            var value = new DifficultyLevels();
            value.ChangeId(Id);
            value.ChangeDifficultyLevelName(DifficultyLevelName);
            return value;
        }
        public void ChangeId(int value) => Id = EntityRules.NonNegative(value, "Id");
        public void ChangeDifficultyLevelName(String value) => Rename(value);
        public void Rename(string name) => DifficultyLevelName = EntityRules.Required(name, "Difficulty name");
    }
}
