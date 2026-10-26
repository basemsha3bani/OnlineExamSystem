using Domain.Entities;
using Domain.Aggregates;
using Domain.DTOs;
using ServicesClasseslibrary.Interface.DataRepository;

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
        public List<ExamDTO> List() => exams.list();
        public ExamDTO GetById(int id) => exams.GetById(id);
        public void Add(ExamDTO exam)
        {
            Subject(exam);

            exam.ChangeSections(new List<ExamSectionsDTO>());
            exams.Add(exam);
        }
        public void Edit(ExamDTO exam)
        {
            var subject = Subject(exam);


            exams.Edit(exam);
        }
        private Domain.Aggregates.StudySubject Subject(ExamDTO exam)
        {
            return Domain.Aggregates.Exam.CheckDefinition(exam, subjects.GetById(exam.StudySubjectId),
                sections.List(exam.Id).SelectMany(s => s.ChapterNumbers));
        }
        public List<ExamSectionRulesDTO> GetSectionRules(int sectionId) => rules.GetBySectionId(sectionId);
        public ExamSectionRulesDTO GetSectionRule(int id) => rules.GetById(id);
        public void AddSectionRule(ExamSectionRulesDTO model)
        {
            model.ChangeId(0);
            ValidateRule(model);
            rules.Add(model);
        }
        public void EditSectionRule(ExamSectionRulesDTO model)
        {
            var current = rules.GetById(model.Id);
            model.ChangeSectionId(Domain.Entities.ExamSectionRules.ParentOf(current));
            ValidateRule(model);
            rules.Edit(model);
        }
        private void ValidateRule(ExamSectionRulesDTO model)
        {
            Domain.Entities.ExamSectionRules.CheckDefinition(model,
                sections.GeById(model.SectionId), rules.GetBySectionId(model.SectionId));
        }
    }
}
