using Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class ExamTypesDetails
    {
        public int Id { get; private set; }
        public int DifficultyLevelId { get; private set; }
        public int ExamTypeId { get; private set; }

        public int NumberOfQuestions { get; private set; }

        public virtual DifficultyLevels DifficultyLevel { get; private set; }

        private ExamTypesDetails() { }
        public static ExamTypesDetails Define(int Id = default, int DifficultyLevelId = default, int ExamTypeId = default, int NumberOfQuestions = default, DifficultyLevels DifficultyLevel = default)
        {
            var value = new ExamTypesDetails();
            value.ChangeId(Id);
            value.ChangeDifficultyLevelId(DifficultyLevelId);
            value.ChangeExamTypeId(ExamTypeId);
            value.ChangeNumberOfQuestions(NumberOfQuestions);
            value.ChangeDifficultyLevel(DifficultyLevel);
            return value;
        }
        public void ChangeId(int value) => Id = EntityRules.NonNegative(value, "Id");
        public void ChangeDifficultyLevelId(int value) => DifficultyLevelId = EntityRules.Positive(value, "DifficultyLevelId");
        public void ChangeExamTypeId(int value) => ExamTypeId = EntityRules.NonNegative(value, "ExamTypeId");
        public void ChangeNumberOfQuestions(int value) => NumberOfQuestions = EntityRules.Positive(value, "NumberOfQuestions");
        public void ChangeDifficultyLevel(DifficultyLevels value) => DifficultyLevel = value;
    }
}
