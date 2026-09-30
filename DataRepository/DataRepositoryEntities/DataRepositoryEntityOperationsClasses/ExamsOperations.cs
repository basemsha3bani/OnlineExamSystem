using AutoMapper;
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
        public ExamDataModel GetById(int id) => mapper.Map<ExamDataModel>(gateway.GetById(e => e.Id == id, e => e.Sections, e => e.StudySubject));
        public void Add(ExamDataModel model)
        {
            var exam = mapper.Map<Exams>(model);
            exam.Id = 0;
            exam.Sections = mapper.Map<List<ExamSections>>(model.Sections);
            foreach (var section in exam.Sections) { section.Id = 0; section.ExamId = 0; }
            gateway.Add(exam);
            gateway.SaveChanges();
        }
        public void Edit(ExamDataModel model)
        {
            var exam = gateway.GetById(e => e.Id == model.Id, e => e.Sections)
                ?? throw new InvalidOperationException("Exam not found.");
            if (model.Sections.Any(s => s.Id != 0 && exam.Sections.All(e => e.Id != s.Id)))
                throw new InvalidOperationException("Invalid section identity.");
            mapper.Map(model, exam);
            foreach (var incoming in model.Sections)
            {
                var section = incoming.Id == 0 ? null : exam.Sections.Single(s => s.Id == incoming.Id);
                if (section == null)
                {
                    section = mapper.Map<ExamSections>(incoming);
                    section.ExamId = exam.Id;
                    exam.Sections.Add(section);
                }
                else
                {
                    mapper.Map(incoming, section);
                    section.ExamId = exam.Id;
                }
            }
            gateway.SaveChanges();
        }
    }
}
