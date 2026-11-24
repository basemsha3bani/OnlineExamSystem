using Domain.DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OnlineExamSystem.Application.Interface.Events;
using OnlineExamSystem.Application.Interface.DataRepository;
using OnlineExamSystem.Application.Interface.Examiner;
using ServicesClasseslibrary.WorkerServices.ExamEvaluation;
using ServicesClasseslibrary.WorkerServices.Interface.Analytics;
using ServicesClasseslibrary.WorkerServices.Interface.Examiner;
using ServicesClasseslibrary.WorkerServices.Messaging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
namespace ServicesClasseslibrary.WorkerServices.InterfaceImplementation.Examiner
{
    public class AttemptEvaluationOrchestrator : IExamAttemptEvaluator, IEvaluationPublisher, IEvaluationNotifier
    {
        private readonly IServiceScopeFactory scopes;
        private readonly ILogger<AttemptEvaluationOrchestrator> logger;
        private readonly IAnalyticsQueue analytics;
        private readonly IEvaluationResultQueue results;
        private readonly IHubContext<EvaluationHub> hub;
        private readonly RabbitMqConsumer consumer;
        IEventBus _eventbus;
       

        public AttemptEvaluationOrchestrator(ILogger<AttemptEvaluationOrchestrator> logger, IAnalyticsQueue analytics,
            IEvaluationResultQueue results, IServiceScopeFactory scopes, IHubContext<EvaluationHub> hub, RabbitMqConsumer consumer,ActivitySource activitySource, IEventBus eventBus)
        { 
            this.logger = logger; this.analytics = analytics; this.results = results; this.scopes = scopes; this.hub = hub; this.consumer = consumer; this._eventbus = eventBus;
       
        }
        
        public async Task EvaluateExamAttemptsAsync()
        {
            
           
            using var scope = scopes.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IExaminerAttemptBackgroundService>();

            var evaluated = await service.EvaluatePending((id, ex) =>
                logger.LogError(ex, "Evaluation failed for attempt {AttemptId}", id));

         
        }

        public async Task PublishEvaluatedExams()
        {
            ///Use ExaminerAttemptService To Read from OutBox Table 
            using var scope = scopes.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IExaminerAttemptBackgroundService>();
            var evaluated = await service.GetEvaluatedExamAttempts();
            foreach (var attempt in evaluated.DistinctBy(a => a.Id))
            {
                ///convert payload into examgraded event
                ExamGradedEvent examGradedEvent =  JsonConvert.DeserializeObject<ExamGradedEvent>(attempt.Payload);



                if (examGradedEvent != null)
                {
                   await _eventbus.PublishAsync(examGradedEvent);
                }


            }


        }

        public Task SendOneBatchAsync(CancellationToken ct) => consumer.RunAsync(
            new[] { RabbitMqTopology.PassedQueue, RabbitMqTopology.FailedQueue }, async body =>
            {
                var result = JsonConvert.DeserializeObject<Tuple<int, int, decimal>>(Encoding.UTF8.GetString(body.Span))
                    ?? throw new JsonSerializationException("Empty evaluation result.");
                await hub.Clients.Group($"candidate_{result.Item2}").SendAsync("EvaluationCompleted", result.Item1, ct);
                using var scope = scopes.CreateScope();
                await scope.ServiceProvider.GetRequiredService<IExamAttemptOprations>()
                    .MarkOutboxMessageProcessedAsync(result.Item1, ct);
                results.TryDequeue(result);
            }, ct);
    }
}
