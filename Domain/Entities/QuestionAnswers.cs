using Domain.Aggregates;

namespace Domain.Entities
{
    public class QuestionAnswers
    {
        public int Id { get; private set; }

        public int QuestionId { get; private set; }
        public string AnswerText { get; private set; }
        public bool IsCorrect { get; private set; }
        public virtual Questions Question { get; private set; }

        private QuestionAnswers() { }
        public static QuestionAnswers Define(int Id = default, int QuestionId = default, string AnswerText = default, bool IsCorrect = default, Questions Question = default)
        {
            var value = new QuestionAnswers();
            value.ChangeId(Id);
            value.ChangeQuestionId(QuestionId);
            value.ChangeAnswerText(AnswerText);
            value.ChangeIsCorrect(IsCorrect);
            value.ChangeQuestion(Question);
            return value;
        }
        public void ChangeId(int value) => Id = EntityRules.NonNegative(value, "Id");
        public void ChangeQuestionId(int value) => QuestionId = EntityRules.NonNegative(value, "QuestionId");
        public void ChangeAnswerText(string value) => Rename(value);
        public void Rename(string text) => AnswerText = EntityRules.Required(text, "Answer text");
        public void ChangeIsCorrect(bool value) => IsCorrect = value;
        public void ChangeQuestion(Questions value) => Question = value;
        public void MarkCorrect(bool correct) => ChangeIsCorrect(correct);
    }
}
