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
    public class DifficultyLevelsOperations : IDifficultyLevelsOperations
    {
        private readonly ContextGateway<DifficultyLevels> gateway;
        private readonly IMapper mapper;
        public DifficultyLevelsOperations(ContextGateway<DifficultyLevels> gateway, IMapper mapper)
        { this.gateway = gateway; this.mapper = mapper; }
        public void Add(DifficultyLevelsDTO model)
        {
            var entity = mapper.Map<DifficultyLevels>(model);
            entity.ChangeId(0);
            gateway.Add(entity); gateway.SaveChanges();
        }
        public void Edit(DifficultyLevelsDTO model)
        {
            var entity = gateway.GetById(e => e.Id == model.Id) ?? throw new InvalidOperationException("Difficulty level not found.");
            mapper.Map(model, entity); gateway.SaveChanges();
        }
        public void Delete(int id)
        {
            var entity = gateway.GetById(e => e.Id == id);
            if (entity == null) return;
            gateway.Delete(entity); gateway.SaveChanges();
        }
        public DifficultyLevelsDTO GetById(int id) => mapper.Map<DifficultyLevelsDTO>(gateway.GetById(e => e.Id == id));
        public List<DifficultyLevelsDTO> list() => mapper.Map<List<DifficultyLevelsDTO>>(gateway.List());
    }
}
