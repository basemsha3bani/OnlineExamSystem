using AutoMapper;
using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
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
        public void Add(ExamSectionsDataModel model)
        {
            var section = mapper.Map<ExamSections>(model);
            section.Id = 0;
            section.examSectionRules = mapper.Map<List<ExamSectionRules>>(model.ExamSectionRules.Where(r => r.NoOfQuestions != 0).ToList());
            foreach (var rule in section.examSectionRules) { rule.Id = 0; rule.SectionId = 0; }
            gateway.Add(section); gateway.SaveChanges();
            model.Id = section.Id;
        }
        public void Edit(ExamSectionsDataModel model)
        {
            var section = gateway.GetById(s => s.Id == model.Id) ?? throw new System.InvalidOperationException("Section not found.");
            section.SectionName = model.SectionName;
            section.NumberOfQuestions = model.NumberOfQuestions;
            section.ChapterNumbers = model.ChapterNumbers.ToList();
            gateway.SaveChanges();
        }
        public List<ExamSectionsDataModel> List(int examId) => mapper.Map<List<ExamSectionsDataModel>>(
            gateway.Query.AsNoTracking().Where(s => s.ExamId == examId).Include(s => s.Exam).Include(s => s.examSectionRules).ThenInclude(r => r.difficultyLevel).ToList());
        public ExamSectionsDataModel GeById(int id) => mapper.Map<ExamSectionsDataModel>(
            gateway.GetById(s => s.Id == id, s => s.examSectionRules, s => s.Exam));
    }
}
