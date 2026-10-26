using Domain.Entities;
using Domain.Aggregates;
using AutoMapper;
using Domain.DTOs;
using ServicesClasseslibrary.Interface.DataRepository;
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
        public void Add(StudySubjectDTO model)
        {
            var entity = mapper.Map<StudySubject>(model);
            entity.ChangeId(0);
            gateway.Add(entity); gateway.SaveChanges();
        }
        public void Edit(StudySubjectDTO model)
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
        public StudySubjectDTO GetById(int id) => mapper.Map<StudySubjectDTO>(gateway.GetById(e => e.Id == id));
        public List<StudySubjectDTO> list() => mapper.Map<List<StudySubjectDTO>>(gateway.List());
    }
}
