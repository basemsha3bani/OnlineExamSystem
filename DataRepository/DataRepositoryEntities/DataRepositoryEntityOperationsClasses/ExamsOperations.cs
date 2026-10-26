using Domain.Entities;
using Domain.Aggregates;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Domain.DTOs;
using ServicesClasseslibrary.Interface.DataRepository;
using DataRepository.GateWay;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class ExamsOperations : IExamOprations
    {
        private readonly ContextGateway<Exam> gateway;
        private readonly IMapper mapper;
        public ExamsOperations(ContextGateway<Exam> gateway, IMapper mapper)
        { this.gateway = gateway; this.mapper = mapper; }
        public List<ExamDTO> list() => mapper.Map<List<ExamDTO>>(gateway.List(null, e => e.StudySubject));
        public ExamDTO GetById(int id) => mapper.Map<ExamDTO>(gateway.Query.AsNoTracking().Include(e => e.StudySubject).Include(e => e.Sections).ThenInclude(s => s.examSectionRules).ThenInclude(r => r.difficultyLevel).SingleOrDefault(e => e.Id == id));
        public void Add(ExamDTO model)
        {
            var exam = mapper.Map<Exam>(model);
            exam.ChangeId(0);
            exam.ChangeSections(model.Sections.Select(CreateSection).ToList());
            foreach (var section in exam.Sections)
            {
                section.ChangeId(0); section.ChangeExamId(0);
                foreach (var rule in section.examSectionRules) { rule.ChangeId(0); rule.ChangeSectionId(0); }
            }
            gateway.Add(exam);
            gateway.SaveChanges();
            model.ChangeId(exam.Id);
        }
        private List<ExamSectionRules> MapRules(ExamSectionsDTO model)
        {
            var result = mapper.Map<List<ExamSectionRules>>(model.ExamSectionRules.Where(r => r.NoOfQuestions != 0).ToList());
            foreach (var rule in result) { rule.ChangeId(0); rule.ChangeSectionId(0); }
            return result;
        }
        private ExamSections CreateSection(ExamSectionsDTO model)
        {
            var section = mapper.Map<ExamSections>(model);
            section.ChangeId(0);
            section.ChangeexamSectionRules(MapRules(model));
            return section;
        }
        public void Edit(ExamDTO model)
        {
            var exam = gateway.Query.Include(e => e.Sections).ThenInclude(s => s.examSectionRules).SingleOrDefault(e => e.Id == model.Id)
                ?? throw new InvalidOperationException("Exam not found.");
            exam.CheckSectionIdentities(model.Sections);
            mapper.Map(model, exam);
            foreach (var incoming in model.Sections)
            {
                var section = incoming.Id == 0 ? null : exam.Sections.Single(s => s.Id == incoming.Id);
                if (section == null)
                {
                    section = CreateSection(incoming);
                    section.ChangeExamId(exam.Id);
                    exam.Sections.Add(section);
                }
                else
                {
                    var trackedRules = section.examSectionRules.ToList();
                    var requestedRules = MapRules(incoming);
                    mapper.Map(incoming, section);
                    section.ChangeexamSectionRules(trackedRules);
                    foreach (var old in trackedRules.ToList())
                        if (requestedRules.All(r => r.DifficultyLevelId != old.DifficultyLevelId)) section.examSectionRules.Remove(old);
                    foreach (var requested in requestedRules)
                    {
                        var rule = trackedRules.SingleOrDefault(r => r.DifficultyLevelId == requested.DifficultyLevelId);
                        if (rule == null) { requested.ChangeId(0); requested.ChangeSectionId(section.Id); section.examSectionRules.Add(requested); }
                        else rule.ChangeNoOfQuestions(requested.NoOfQuestions);
                    }
                    section.ChangeExamId(exam.Id);
                }
            }
            gateway.SaveChanges();
        }
    }
}
