using AutoMapper;
using Microsoft.EntityFrameworkCore;
using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class ExamsOperations : IExamOprations
    {
        private readonly ContextGateway<Exams> gateway;
        private readonly IMapper mapper;
        public ExamsOperations(ContextGateway<Exams> gateway, IMapper mapper)
        { this.gateway = gateway; this.mapper = mapper; }
        public List<ExamDataModel> list() => mapper.Map<List<ExamDataModel>>(gateway.List(null, e => e.StudySubject));
        public ExamDataModel GetById(int id) => mapper.Map<ExamDataModel>(gateway.Query.AsNoTracking().Include(e => e.StudySubject).Include(e => e.Sections).ThenInclude(s => s.examSectionRules).ThenInclude(r => r.difficultyLevel).SingleOrDefault(e => e.Id == id));
        public void Add(ExamDataModel model)
        {
            var exam = mapper.Map<Exams>(model);
            exam.Id = 0;
            exam.Sections = model.Sections.Select(CreateSection).ToList();
            foreach (var section in exam.Sections)
            {
                section.Id = 0; section.ExamId = 0;
                foreach (var rule in section.examSectionRules) { rule.Id = 0; rule.SectionId = 0; }
            }
            gateway.Add(exam);
            gateway.SaveChanges();
            model.Id = exam.Id;
        }
        private List<ExamSectionRules> MapRules(ExamSectionsDataModel model)
        {
            var result = mapper.Map<List<ExamSectionRules>>(model.ExamSectionRules.Where(r => r.NoOfQuestions != 0).ToList());
            foreach (var rule in result) { rule.Id = 0; rule.SectionId = 0; }
            return result;
        }
        private ExamSections CreateSection(ExamSectionsDataModel model)
        {
            var section = mapper.Map<ExamSections>(model);
            section.Id = 0;
            section.examSectionRules = MapRules(model);
            return section;
        }
        public void Edit(ExamDataModel model)
        {
            var exam = gateway.Query.Include(e => e.Sections).ThenInclude(s => s.examSectionRules).SingleOrDefault(e => e.Id == model.Id)
                ?? throw new InvalidOperationException("Exam not found.");
            if (model.Sections.Any(s => s.Id != 0 && exam.Sections.All(e => e.Id != s.Id)))
                throw new InvalidOperationException("Invalid section identity.");
            mapper.Map(model, exam);
            foreach (var incoming in model.Sections)
            {
                var section = incoming.Id == 0 ? null : exam.Sections.Single(s => s.Id == incoming.Id);
                if (section == null)
                {
                    section = CreateSection(incoming);
                    section.ExamId = exam.Id;
                    exam.Sections.Add(section);
                }
                else
                {
                    var trackedRules = section.examSectionRules.ToList();
                    var requestedRules = MapRules(incoming);
                    mapper.Map(incoming, section);
                    section.examSectionRules = trackedRules;
                    foreach (var old in trackedRules.ToList())
                        if (requestedRules.All(r => r.DifficultyLevelId != old.DifficultyLevelId)) section.examSectionRules.Remove(old);
                    foreach (var requested in requestedRules)
                    {
                        var rule = trackedRules.SingleOrDefault(r => r.DifficultyLevelId == requested.DifficultyLevelId);
                        if (rule == null) { requested.Id = 0; requested.SectionId = section.Id; section.examSectionRules.Add(requested); }
                        else rule.NoOfQuestions = requested.NoOfQuestions;
                    }
                    section.ExamId = exam.Id;
                }
            }
            gateway.SaveChanges();
        }
    }
}
