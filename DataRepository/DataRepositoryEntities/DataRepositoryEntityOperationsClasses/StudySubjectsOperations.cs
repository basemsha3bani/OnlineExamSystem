using AutoMapper;
using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using System;
using System.Collections.Generic;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class StudySubjectsOperations : IStudySubjectsOperations
    {
        private readonly ContextGateway<StudySubject> gateway;
        private readonly IMapper mapper;
        public StudySubjectsOperations(ContextGateway<StudySubject> gateway, IMapper mapper)
        { this.gateway = gateway; this.mapper = mapper; }
        public void Add(StudySubjectDataModel model)
        {
            var entity = mapper.Map<StudySubject>(model);
            entity.Id = 0;
            gateway.Add(entity); gateway.SaveChanges();
        }
        public void Edit(StudySubjectDataModel model)
        {
            var entity = gateway.GetById(e => e.Id == model.Id) ?? throw new InvalidOperationException("Subject not found.");
            mapper.Map(model, entity); gateway.SaveChanges();
        }
        public void Delete(int id)
        {
            var entity = gateway.GetById(e => e.Id == id);
            if (entity == null) return;
            gateway.Delete(entity); gateway.SaveChanges();
        }
        public StudySubjectDataModel GetById(int id) => mapper.Map<StudySubjectDataModel>(gateway.GetById(e => e.Id == id));
        public List<StudySubjectDataModel> list() => mapper.Map<List<StudySubjectDataModel>>(gateway.List());
    }
}
