using Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class ExamQuestions
    {
        public int Id { get; private set; }
        public int ExamId { get; private set; }

        public virtual Exam Exam{get; private set;}
        public int QuestionId { get; private set; }

        public virtual Questions Question { get; private set; }

        private ExamQuestions() { }
        public static ExamQuestions Define(int Id = default, int ExamId = default, Exam Exam = default, int QuestionId = default, Questions Question = default)
        {
            var value = new ExamQuestions();
            value.ChangeId(Id);
            value.ChangeExamId(ExamId);
            value.ChangeExam(Exam);
            value.ChangeQuestionId(QuestionId);
            value.ChangeQuestion(Question);
            return value;
        }
        public void ChangeId(int value) => Id = EntityRules.NonNegative(value, "Id");
        public void ChangeExamId(int value) => ExamId = EntityRules.NonNegative(value, "ExamId");
        public void ChangeExam(Exam value) => Exam = value;
        public void ChangeQuestionId(int value) => QuestionId = EntityRules.NonNegative(value, "QuestionId");
        public void ChangeQuestion(Questions value) => Question = value;
    }
}
