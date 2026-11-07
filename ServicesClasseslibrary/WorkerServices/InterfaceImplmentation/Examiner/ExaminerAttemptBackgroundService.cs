using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineExamSystem.Application.Interface.DataRepository;
using OnlineExamSystem.Application.Interface.Events;
using OnlineExamSystem.Application.Interface.Examiner;
using ServicesClasseslibrary.WorkerServices.InterfaceImplementation.Examiner;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Domain.DTOs;

namespace ServicesClasseslibrary.WorkerServices.InterfaceImplmentation.Examiner
{
    public class ExaminerAttemptBackgroundService : IExaminerAttemptBackgroundService
    {
        private readonly IExamAttemptOprations examAttemptOprations;
        private ActivitySource _activitySource;
        IEventBus _eventBus;
        private readonly ILogger<ExaminerAttemptBackgroundService> _logger;

        public ExaminerAttemptBackgroundService(IExamAttemptOprations examAttemptOprations,
            ActivitySource activitySource,IEventBus eventBus,
            ILogger<ExaminerAttemptBackgroundService> logger) 
        {
            this.examAttemptOprations = examAttemptOprations;
            _activitySource = activitySource;
            this._eventBus = eventBus;
            this._logger = logger;
        }

        public async Task<List<Tuple<int, int, decimal>>> EvaluatePending(Action<int, Exception> onFailure = null)
    {
        Tuple<int, int, decimal> evaluatedTuple = new Tuple<int, int, decimal>(0, 0, 0);
        var pending = (await examAttemptOprations.listSubmitted()).Select(s => s.Id);
        
        List<Tuple<int, int, decimal>> evaluatedTuples = new List<Tuple<int, int, decimal>>();
            using var activity = _activitySource.StartActivity("EvaluateExamAttempts - Iteration");

            ;

            foreach (int id in pending)
        {
            try
            {
                    var iterationId = Guid.NewGuid();
                    activity?.SetTag("iteration.id", iterationId);   
                    var pendingAttempt = examAttemptOprations.CheckIfAttemptSubmitted(id);
                if (pendingAttempt == null) continue;
                var attempt = ExamAttempt.Restore(pendingAttempt);
                attempt.Evaluate();
                   
    
                    await examAttemptOprations.Evaluate(attempt.ToDTO(), iterationId);
                evaluatedTuple = new Tuple<int, int, decimal>(id, attempt.UserId, attempt.Score.Value);
                evaluatedTuples.Add(evaluatedTuple);
                    // or attempt's original CorrelationId if you store it
                  
                    _logger.LogInformation("Graded Attempt={Attempt} Iteration={Iter}", attempt.Id, iterationId);
                }
            catch (DbUpdateConcurrencyException) { /* Another worker completed this attempt. */ }
            catch (Exception ex)
            {
                if (onFailure == null) throw;
                onFailure(id, ex); // Leave pending for retry without blocking other attempts.
            }

        }
        return evaluatedTuples;
    }

     

        public async Task<List<OutboxMessage>> GetEvaluatedExamAttempts()
        {
          return await  examAttemptOprations.listEvaluated();
        }

      
    }
}
