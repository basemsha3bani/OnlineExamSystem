using AutoMapper;
using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class ExamSectionRulesOperations : IExamSectionRuleOperations
    {
        private readonly ContextGateway<ExamSectionRules> gateway;
        private readonly IMapper mapper;
        public ExamSectionRulesOperations(ContextGateway<ExamSectionRules> gateway, IMapper mapper)
        { this.gateway = gateway; this.mapper = mapper; }
        public List<ExamSectionRulesDataModel> GetBySectionId(int sectionId) => mapper.Map<List<ExamSectionRulesDataModel>>(
            gateway.List(r => r.SectionId == sectionId, r => r.difficultyLevel, r => r.section.Exam.StudySubject));
        public void Add(ExamSectionRulesDataModel model)
        {
            if (model.NoOfQuestions <= 0) throw new InvalidOperationException("Question count must be positive.");
            using var transaction = gateway.BeginTransaction();
            if (gateway.List(r => r.SectionId == model.SectionId).Sum(r => r.NoOfQuestions) + model.NoOfQuestions > 5)
                throw new InvalidOperationException("Max 5 questions per section");
            var entity = mapper.Map<ExamSectionRules>(model);
            entity.Id = 0;
            gateway.Add(entity);
            gateway.SaveChanges();
            transaction.Commit();
        }
    }
}
