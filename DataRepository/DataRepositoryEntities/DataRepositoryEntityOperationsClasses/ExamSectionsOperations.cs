using AutoMapper;
using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using System.Collections.Generic;

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
            gateway.Add(section); gateway.SaveChanges();
        }
        public List<ExamSectionsDataModel> List(int examId) => mapper.Map<List<ExamSectionsDataModel>>(
            gateway.List(s => s.ExamId == examId, s => s.examSectionRules, s => s.Exam));
        public ExamSectionsDataModel GeById(int id) => mapper.Map<ExamSectionsDataModel>(
            gateway.GetById(s => s.Id == id, s => s.examSectionRules, s => s.Exam));
    }
}
