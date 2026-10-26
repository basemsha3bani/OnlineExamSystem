using Domain.Entities;
using Domain.Aggregates;
using AutoMapper;
using Domain.DTOs;
using ServicesClasseslibrary.Interface.DataRepository;
using DataRepository.GateWay;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class ExamSectionsOperations : IExamSectionsOperations
    {
        private readonly ContextGateway<ExamSections> gateway;
        private readonly IMapper mapper;
        public ExamSectionsOperations(ContextGateway<ExamSections> gateway, IMapper mapper)
        { this.gateway = gateway; this.mapper = mapper; }
        public void Add(ExamSectionsDTO model)
        {
            var section = mapper.Map<ExamSections>(model);
            section.ChangeId(0);
            section.ChangeexamSectionRules(mapper.Map<List<ExamSectionRules>>(model.ExamSectionRules.Where(r => r.NoOfQuestions != 0).ToList()));
            foreach (var rule in section.examSectionRules) { rule.ChangeId(0); rule.ChangeSectionId(0); }
            gateway.Add(section); gateway.SaveChanges();
            model.ChangeId(section.Id);
        }
        public void Edit(ExamSectionsDTO model)
        {
            var section = gateway.GetById(s => s.Id == model.Id) ?? throw new System.InvalidOperationException("Section not found.");
            section.ChangeSectionName(model.SectionName);
            section.ChangeNumberOfQuestions(model.NumberOfQuestions);
            section.ChangeChapterNumbers(model.ChapterNumbers.ToList());
            gateway.SaveChanges();
        }
        public List<ExamSectionsDTO> List(int examId) => mapper.Map<List<ExamSectionsDTO>>(
            gateway.Query.AsNoTracking().Where(s => s.ExamId == examId).Include(s => s.Exam).Include(s => s.examSectionRules).ThenInclude(r => r.difficultyLevel).ToList());
        public ExamSectionsDTO GeById(int id) => mapper.Map<ExamSectionsDTO>(
            gateway.GetById(s => s.Id == id, s => s.examSectionRules, s => s.Exam));
    }
}
