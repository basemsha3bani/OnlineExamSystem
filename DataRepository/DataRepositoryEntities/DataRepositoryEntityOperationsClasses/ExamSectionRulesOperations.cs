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
        public ExamSectionRulesDataModel GetById(int id) => mapper.Map<ExamSectionRulesDataModel>(gateway.GetById(r => r.Id == id, r => r.difficultyLevel));
        public void Add(ExamSectionRulesDataModel model)
        {
            var entity = mapper.Map<ExamSectionRules>(model);
            entity.Id = 0;
            gateway.Add(entity); gateway.SaveChanges();
        }
        public void Edit(ExamSectionRulesDataModel model)
        {
            var entity = gateway.GetById(r => r.Id == model.Id) ?? throw new InvalidOperationException("Rule not found.");
            entity.DifficultyLevelId = model.DifficultyLevelId;
            entity.NoOfQuestions = model.NoOfQuestions;
            gateway.SaveChanges();
        }
    }
}
