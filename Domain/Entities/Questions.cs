using Domain.Exceptions;
using Domain.DTOs;
using Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Questions
    {
        public int Id { get; private set; }
        public int ChapterNumber { get; private set; } = 1;

        public String QuestionText { get; private set; }

        public virtual ICollection<QuestionAnswers> QuestionAnswers { get; private set; }
        public int DifficultyLevelId { get; private set; }
        public int? StudySubjectId { get; private set; }
        public virtual DifficultyLevels DifficultyLevel { get; private set; }

        private Questions() { }
        public static Questions Define(int Id = default, int ChapterNumber = 1, String QuestionText = default, ICollection<QuestionAnswers> QuestionAnswers = default, int DifficultyLevelId = default, int? StudySubjectId = default, DifficultyLevels DifficultyLevel = default)
        {
            var value = new Questions();
            value.ChangeId(Id);
            value.ChangeChapterNumber(ChapterNumber);
            value.ChangeQuestionText(QuestionText);
            value.ChangeQuestionAnswers(QuestionAnswers ?? new List<QuestionAnswers>());
            value.ChangeDifficultyLevelId(DifficultyLevelId);
            value.ChangeStudySubjectId(StudySubjectId);
            value.ChangeDifficultyLevel(DifficultyLevel);
            return value;
        }
        public void ChangeId(int value) => Id = EntityRules.NonNegative(value, "Id");
        public void ChangeChapterNumber(int value) => ChapterNumber = EntityRules.Positive(value, "ChapterNumber");
        public void ChangeQuestionText(String value) => QuestionText = EntityRules.Required(value, "QuestionText");
        public void ChangeQuestionAnswers(ICollection<QuestionAnswers> value) => QuestionAnswers = value;
        public void ChangeDifficultyLevelId(int value) => DifficultyLevelId = EntityRules.Positive(value, "DifficultyLevelId");
        public void ChangeStudySubjectId(int? value) => StudySubjectId = value;
        public void ChangeDifficultyLevel(DifficultyLevels value) => DifficultyLevel = value;
        public void ValidateForSubject(StudySubject subject)
        {
            EntityRules.Exists(subject, "Subject");
            if (StudySubjectId != subject.Id) throw new DomainException("Question belongs to another subject.");
            subject.ValidateChapter(ChapterNumber);
            EntityRules.Positive(DifficultyLevelId, "Difficulty");
        }

        public static void CheckDefinition(QuestionsDTO model, StudySubjectDTO subject)
        {
            EntityRules.Exists(subject, "Subject");
            var definition = global::Domain.Aggregates.StudySubject.Define(Id: subject.Id, SubjectName: subject.SubjectName, NumberOfChapters: subject.NumberOfChapters);
            definition.ValidateChapter(model.ChapterNumber);
            EntityRules.Required(model.QuestionText, "Question text");
            EntityRules.Positive(model.DifficultyLevelId, "Difficulty");
            CheckAnswers(model.QuestionAnswersDTO);
        }
        public static void CheckAnswers(IEnumerable<QuestionAnswersDTO> answers)
        {
            if (answers == null) throw new DomainException("Each question must have exactly one correct answer.");
            var items = answers.ToList();
            if (items.Count == 0 || items.Any(a => a == null) || items.Count(a => a.IsCorrect) != 1)
            throw new DomainException("Each question must have exactly one correct answer.");
            foreach (var answer in items) EntityRules.Required(answer.AnswerText, "Answer text");
        }
        public void CheckAnswerIdentities(IEnumerable<QuestionAnswersDTO> answers)
        {
            var items = answers.ToList();
            if (items.Where(a => a.Id != 0).GroupBy(a => a.Id).Any(g => g.Count() > 1)
            || items.Any(a => a.Id != 0 && QuestionAnswers.All(e => e.Id != a.Id)))
            throw new DomainException("Invalid answer identity.");
        }

    }
}
