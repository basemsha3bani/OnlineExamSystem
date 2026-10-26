using Domain.Entities;
using Domain.Aggregates;
using AutoMapper;
using Domain.DTOs;
using ServicesClasseslibrary.Interface.DataRepository;
using DataRepository.GateWay;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class ExamAttemptOprations : IExamAttemptOprations
    {

        private readonly ContextGateway<ExamAttempt> gateway;
        private readonly IMapper mapper;
        public ExamAttemptOprations(ContextGateway<ExamAttempt> gateway, IMapper mapper)
        { this.gateway = gateway; this.mapper = mapper; }
       
       
        public int Add(ExamAttemptDTO model)
        {
            ExamAttempt entity = mapper.Map<ExamAttempt>(model);

            gateway.Add(entity);
            gateway.SaveChanges();
            return entity.Id;
        }
        public async Task Edit(ExamAttemptDTO model)
        {
            var entity = gateway.GetById(e => e.Id == model.Id) ?? throw new InvalidOperationException("Difficulty level not found.");
            mapper.Map(model, entity); await gateway.SaveChangesAsync();
        }
        public void Delete(int id)
        {
            var entity = gateway.GetById(e => e.Id == id);
            if (entity == null) return;
            gateway.Delete(entity); gateway.SaveChanges();
        }
        public ExamAttemptDTO GetById(int id) => mapper.Map<ExamAttemptDTO>(gateway.GetById(e => e.Id == id));
        public List<ExamAttemptDTO> listByUser(int userId) => mapper.Map<List<ExamAttemptDTO>>(gateway.List(l=>l.UserId==userId));

        public ExamAttemptDTO CheckIfAttemptSubmitted(int id)
        {
            ExamAttempt attempt = gateway.GetById(e => e.Id == id);
            return attempt.Status == "Submitted" ? mapper.Map<ExamAttemptDTO>(attempt) : null;
        }

        

        public List<ExamAttemptDTO> listSubmitted()
        {
           return mapper.Map<List<ExamAttemptDTO>>(gateway.List(l=> l.Status == "Submitted"));
        }
    }
}
