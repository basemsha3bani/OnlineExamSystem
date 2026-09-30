using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses.DataRepository.DataRepositoryEntities;
using System.Collections.Generic;

namespace ServicesClasseslibrary
{
    public class ExamService : IExamService
    {
        private readonly IExamOprations exams;
        private readonly IExamSectionRuleOperations rules;
        public ExamService(IExamOprations exams, IExamSectionRuleOperations rules)
        { this.exams = exams; this.rules = rules; }
        public List<ExamDataModel> List() => exams.list();
        public ExamDataModel GetById(int id) => exams.GetById(id);
        public void Add(ExamDataModel exam) => exams.Add(exam);
        public void Edit(ExamDataModel exam) => exams.Edit(exam);
        public List<ExamSectionRulesDataModel> GetSectionRules(int sectionId) => rules.GetBySectionId(sectionId);
        public void AddSectionRule(ExamSectionRulesDataModel model) => rules.Add(model);
    }
}
