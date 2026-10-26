using Domain.Entities;
using Domain.Aggregates;
using AutoMapper;
using Domain.DTOs;
using ServicesClasseslibrary.Interface.DataRepository;
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
        public List<ExamSectionRulesDTO> GetBySectionId(int sectionId) => mapper.Map<List<ExamSectionRulesDTO>>(
            gateway.List(r => r.SectionId == sectionId, r => r.difficultyLevel, r => r.section.Exam.StudySubject));
        public ExamSectionRulesDTO GetById(int id) => mapper.Map<ExamSectionRulesDTO>(gateway.GetById(r => r.Id == id, r => r.difficultyLevel));
        public void Add(ExamSectionRulesDTO model)
        {
            var entity = mapper.Map<ExamSectionRules>(model);
            entity.ChangeId(0);
            gateway.Add(entity); gateway.SaveChanges();
        }
        public void Edit(ExamSectionRulesDTO model)
        {
            var entity = gateway.GetById(r => r.Id == model.Id) ?? throw new InvalidOperationException("Rule not found.");
            entity.ChangeDifficultyLevelId(model.DifficultyLevelId);
            entity.ChangeNoOfQuestions(model.NoOfQuestions);
            gateway.SaveChanges();
        }
    }
}
