using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;

using System.Collections.Generic;
using System.Linq;

namespace ServicesClasseslibrary
{
    public class ExamService : IExamService
    {
        private readonly IExamOprations exams;
        private readonly IExamSectionRuleOperations rules;
        private readonly IExamSectionsOperations sections;
        private readonly IStudySubjectsOperations subjects;
        public ExamService(IExamOprations exams, IExamSectionRuleOperations rules, IExamSectionsOperations sections, IStudySubjectsOperations subjects)
        { this.exams = exams; this.rules = rules; this.sections = sections; this.subjects = subjects; }
        public List<ExamDataModel> List() => exams.list();
        public ExamDataModel GetById(int id) => exams.GetById(id);
        public void Add(ExamDataModel exam)
        {
            Subject(exam);
            Domain.Services.ExamConfiguration.ValidateExamDraft(exam.Title);
            exam.Sections = new List<ExamSectionsDataModel>();
            exams.Add(exam);
        }
        public void Edit(ExamDataModel exam)
        {
            var subject = Subject(exam);
            Domain.Services.ExamConfiguration.ValidateExamDraft(exam.Title);
            foreach (var section in sections.List(exam.Id))
                foreach (var chapter in section.ChapterNumbers) subject.ValidateChapter(chapter);
            exams.Edit(exam);
        }
        private Domain.Aggregates.StudySubject Subject(ExamDataModel exam)
        {
            var row = subjects.GetById(exam.StudySubjectId) ?? throw new Domain.Exceptions.DomainException("Subject not found.");
            return new Domain.Aggregates.StudySubject(row.Id, row.SubjectName, row.NumberOfChapters);
        }
        public List<ExamSectionRulesDataModel> GetSectionRules(int sectionId) => rules.GetBySectionId(sectionId);
        public ExamSectionRulesDataModel GetSectionRule(int id) => rules.GetById(id);
        public void AddSectionRule(ExamSectionRulesDataModel model)
        {
            model.Id = 0;
            ValidateRule(model);
            rules.Add(model);
        }
        public void EditSectionRule(ExamSectionRulesDataModel model)
        {
            var current = rules.GetById(model.Id) ?? throw new Domain.Exceptions.DomainException("Rule not found.");
            model.SectionId = current.SectionId;
            ValidateRule(model);
            rules.Edit(model);
        }
        private void ValidateRule(ExamSectionRulesDataModel model)
        {
            var section = sections.GeById(model.SectionId) ?? throw new Domain.Exceptions.DomainException("Section not found.");
            Domain.Services.ExamConfiguration.ValidateDifficultyDraft(section.NumberOfQuestions,
                rules.GetBySectionId(model.SectionId).Where(r => r.Id != model.Id).Select(r => new Domain.Aggregates.SectionDifficulty(new Domain.Aggregates.Difficulty(r.DifficultyLevelId), r.NoOfQuestions)),
                new Domain.Aggregates.SectionDifficulty(new Domain.Aggregates.Difficulty(model.DifficultyLevelId), model.NoOfQuestions));
        }
    }
}
