using Domain.Exceptions;
using Domain.DTOs;
using Domain.Aggregates;
namespace Domain.Entities
{
    public class ExamSectionRules
    {
        public int Id { get; private set; }
        public int SectionId { get; private set; }
        public int DifficultyLevelId { get; private set; }
        public int NoOfQuestions { get; private set; }
        public ExamSections section { get; private set; }
        public DifficultyLevels difficultyLevel { get; private set; }

        private ExamSectionRules() { }
        public static ExamSectionRules Define(int Id = default, int SectionId = default, int DifficultyLevelId = default, int NoOfQuestions = default, ExamSections section = default, DifficultyLevels difficultyLevel = default)
        {
            var value = new ExamSectionRules();
            value.ChangeId(Id);
            value.ChangeSectionId(SectionId);
            value.ChangeDifficultyLevelId(DifficultyLevelId);
            value.ChangeNoOfQuestions(NoOfQuestions);
            value.Changesection(section);
            value.ChangedifficultyLevel(difficultyLevel);
            return value;
        }
        public void ChangeId(int value) => Id = EntityRules.NonNegative(value, "Id");
        public void ChangeSectionId(int value) => SectionId = EntityRules.NonNegative(value, "SectionId");
        public void ChangeDifficultyLevelId(int value) => DifficultyLevelId = EntityRules.Positive(value, "DifficultyLevelId");
        public void ChangeNoOfQuestions(int value) => NoOfQuestions = EntityRules.Positive(value, "NoOfQuestions");
        public void ChangeAllocation(int difficultyLevelId, int numberOfQuestions)
        {
            EntityRules.Positive(difficultyLevelId, "Difficulty");
            EntityRules.Positive(numberOfQuestions, "Question count");
            DifficultyLevelId = difficultyLevelId;
            NoOfQuestions = numberOfQuestions;
        }
        public void Changesection(ExamSections value) => section = value;
        public void ChangedifficultyLevel(DifficultyLevels value) => difficultyLevel = value;

        public static void CheckAllocation(int total, IEnumerable<ExamSectionRulesDTO> rules)
        => ValidateAllocation(total, rules.Select(r => (r.DifficultyLevelId, r.NoOfQuestions)));

        public static void CheckAllocation(int total, IEnumerable<ExamSectionRules> rules)
        => ValidateAllocation(total, rules.Select(r => (r.DifficultyLevelId, r.NoOfQuestions)));

        private static void ValidateAllocation(int total, IEnumerable<(int Difficulty, int Count)> rules)
        {
            var items = rules.ToList();
            if (items.Any(r => r.Count < 1 || r.Difficulty < 1)) throw new DomainException("Difficulty and count must be positive.");
            if (items.Select(r => r.Difficulty).Distinct().Count() != items.Count) throw new DomainException("This difficulty is already configured.");
            if (items.Sum(r => (long)r.Count) > total) throw new DomainException("Difficulty counts exceed the section question count.");
        }
        public static void CheckDefinition(ExamSectionRulesDTO model, ExamSectionsDTO section, IEnumerable<ExamSectionRulesDTO> existing)
        {
            EntityRules.Exists(section, "Section");
            CheckAllocation(section.NumberOfQuestions, existing.Where(r => r.Id != model.Id).Append(model));
        }
        public static int ParentOf(ExamSectionRulesDTO current) => EntityRules.Exists(current, "Rule").SectionId;

    }
}
