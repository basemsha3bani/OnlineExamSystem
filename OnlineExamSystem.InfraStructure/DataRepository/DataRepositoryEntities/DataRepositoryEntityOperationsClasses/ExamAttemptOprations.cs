using AutoMapper;
using DataRepository.GateWay;



using Domain.Aggregates;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OnlineExamSystem.Application.Interface.DataRepository;
using OnlineExamSystem.Application.Interface.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class ExamAttemptOprations : IExamAttemptOprations
    {

        private readonly ContextGateway<ExamAttempt> gateway;
        private readonly DbConext context;

        private readonly IMapper mapper;
        public ExamAttemptOprations(ContextGateway<ExamAttempt> gateway, IMapper mapper, DbConext context)
        { this.gateway = gateway; this.mapper = mapper;this.context= context; }
       
       
        public int Add(ExamAttemptDTO model)
        {
            ExamAttempt entity = mapper.Map<ExamAttempt>(model);

            gateway.Add(entity);
            gateway.SaveChanges();
            return entity.Id;
        }
        public async Task Edit(ExamAttemptDTO attempt)
        {
            var entity = gateway.GetById(e => e.Id == attempt.Id) ?? throw new InvalidOperationException("Difficulty level not found.");
            mapper.Map(attempt, entity);
            await gateway.SaveChangesAsync();   
        }
      

              public async Task Evaluate(ExamAttemptDTO attempt, Guid iterationId)
        {
            var entity = gateway.GetById(e => e.Id == attempt.Id)
                ?? throw new InvalidOperationException("Attempt not found.");

            mapper.Map(attempt, entity);
            context.Update(entity);
            var gradedEvent = new ExamGradedEvent(
                EventId: Guid.NewGuid(),
                AttemptId: attempt.Id,
                ExaminerId: attempt.UserId,
                OccurredAt: DateTime.UtcNow,
                CorrelationId: iterationId,
                score: attempt.Score.GetValueOrDefault());

            context.OutboxMessages.Add(new OutboxMessage
            {
                Id = attempt.Id,
                EventType = nameof(ExamGradedEvent),
                Payload = JsonConvert.SerializeObject(gradedEvent)
            });

            await context.SaveChangesAsync();
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

        

        public async Task<List<ExamAttemptDTO>> listSubmitted()
        {
           return mapper.Map<List<ExamAttemptDTO>>(await gateway.ListAsync(l=> l.Status == "Submitted"));
        }
        public async Task MarkOutboxMessageProcessedAsync(int attemptId, System.Threading.CancellationToken ct)
        {
            var message = await context.OutboxMessages.SingleAsync(m => m.Id == attemptId, ct);
            if (message.ProcessedAt.HasValue) return;

            message.ProcessedAt = DateTime.UtcNow;
            await context.SaveChangesAsync(ct);
        }

        public async Task< List<OutboxMessage>> listEvaluated()
        {
            try
            {
                var x = await (from OutboxMessage in context.OutboxMessages
                               join
                               ExamAttempt in context.ExaminerAttempts
                               on OutboxMessage.Id equals ExamAttempt.Id
                               where OutboxMessage.ProcessedAt == null
                               select OutboxMessage).ToListAsync();

                return x;
            }
            catch(Exception ex)
            {
                return new List<OutboxMessage>();
            }
                  
             
        }

       
    }
}
