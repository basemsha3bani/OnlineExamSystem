using Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class ExamTypes
    {
        public int Id { get; private set; }
        public String ExamTypeName { get; private set; }
        public int DifficultyLevelId { get; private set; }

        public int NumberOfQuestions { get; private set; }

        public virtual DifficultyLevels DifficultyLevel { get; private set; }

        private ExamTypes() { }
        public static ExamTypes Define(int Id = default, String ExamTypeName = default, int DifficultyLevelId = default, int NumberOfQuestions = default, DifficultyLevels DifficultyLevel = default)
        {
            var value = new ExamTypes();
            value.ChangeId(Id);
            value.ChangeExamTypeName(ExamTypeName);
            value.ChangeDifficultyLevelId(DifficultyLevelId);
            value.ChangeNumberOfQuestions(NumberOfQuestions);
            value.ChangeDifficultyLevel(DifficultyLevel);
            return value;
        }
        public void ChangeId(int value) => Id = EntityRules.NonNegative(value, "Id");
        public void ChangeExamTypeName(String value) => ExamTypeName = EntityRules.Required(value, "ExamTypeName");
        public void ChangeDifficultyLevelId(int value) => DifficultyLevelId = EntityRules.Positive(value, "DifficultyLevelId");
        public void ChangeNumberOfQuestions(int value) => NumberOfQuestions = EntityRules.Positive(value, "NumberOfQuestions");
        public void ChangeDifficultyLevel(DifficultyLevels value) => DifficultyLevel = value;
    }
}
