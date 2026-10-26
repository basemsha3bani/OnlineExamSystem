using Domain.Exceptions;
using Domain.DTOs;
using Domain.Aggregates;

using System.Collections.Generic;

namespace Domain.Entities
{
    public class ExamSections
    {
        public int Id { get; private set; }
        public int NumberOfQuestions { get; private set; }
        public List<int> ChapterNumbers { get; private set; } = new List<int>();
        public int ExamId { get; private set; }
        public string SectionName { get; private set; }

        public virtual Exam Exam { get; private set; }

        public List<ExamSectionRules> examSectionRules { get; private set; } = new();

        private ExamSections() { }
        public static ExamSections Define(int Id = default, int NumberOfQuestions = default, List<int> ChapterNumbers = default, int ExamId = default, string SectionName = default, Exam Exam = default, List<ExamSectionRules> examSectionRules = default)
        {
            var value = new ExamSections();
            value.ChangeId(Id);
            value.ChangeNumberOfQuestions(NumberOfQuestions);
            value.ChapterNumbers = ChapterNumbers ?? new List<int>();
            value.ChangeExamId(ExamId);
            value.ChangeSectionName(SectionName);
            value.ChangeExam(Exam);
            value.ChangeexamSectionRules(examSectionRules ?? new());
            return value;
        }
        public void ChangeId(int value) => Id = EntityRules.NonNegative(value, "Id");
        public void ChangeNumberOfQuestions(int value) => NumberOfQuestions = EntityRules.NonNegative(value, "NumberOfQuestions");
        public void ChangeChapterNumbers(List<int> value) => ChapterNumbers = value;
        public void ChangeExamId(int value) => ExamId = EntityRules.NonNegative(value, "ExamId");
        public void ChangeSectionName(string value) => SectionName = EntityRules.Required(value, "SectionName");
        public void ChangeExam(Exam value) => Exam = value;
        public void ChangeexamSectionRules(List<ExamSectionRules> value) => examSectionRules = value;
        public void ValidateForAttempt(StudySubject subject)
        {
            EntityRules.Exists(subject, "Subject");
            if (NumberOfQuestions < 1 || ChapterNumbers == null || ChapterNumbers.Count == 0
            || examSectionRules == null || examSectionRules.Count == 0)
            throw new DomainException("Sections require chapters and positive difficulty counts.");
            if (ChapterNumbers.Distinct().Count() != ChapterNumbers.Count)
            throw new DomainException("Section chapters must be unique.");
            foreach (var chapter in ChapterNumbers) subject.ValidateChapter(chapter);
            ExamSectionRules.CheckAllocation(NumberOfQuestions, examSectionRules);
            if (examSectionRules.Sum(r => (long)r.NoOfQuestions) != NumberOfQuestions)
            throw new DomainException("Difficulty counts must equal the section question count.");
        }

        public static int ParentOf(ExamSectionsDTO current) => EntityRules.Exists(current, "Section").ExamId;
        public static int SubjectOf(ExamDTO exam) => EntityRules.Exists(exam, "Exam").StudySubjectId;
        public static void CheckDraft(ExamSectionsDTO model, ExamDTO exam)
        {
            EntityRules.Exists(exam, "Exam");
            EntityRules.Required(model.SectionName, "Section name");
        }
        public static void CheckConfiguration(ExamSectionsDTO model, ExamSectionsDTO current, StudySubjectDTO subject)
        {
            EntityRules.Exists(current, "Section"); EntityRules.Exists(subject, "Subject");
            EntityRules.Required(model.SectionName, "Section name");
            EntityRules.Positive(model.NumberOfQuestions, "Question count");
            if (model.ChapterNumbers == null || model.ChapterNumbers.Count == 0 || model.ChapterNumbers.Distinct().Count() != model.ChapterNumbers.Count)
            throw new DomainException("A section requires unique chapters.");
            var definition = global::Domain.Aggregates.StudySubject.Define(Id: subject.Id, SubjectName: subject.SubjectName, NumberOfChapters: subject.NumberOfChapters);
            foreach (var chapter in model.ChapterNumbers) definition.ValidateChapter(chapter);
            ExamSectionRules.CheckAllocation(model.NumberOfQuestions, current.ExamSectionRules);
        }

    }

}
