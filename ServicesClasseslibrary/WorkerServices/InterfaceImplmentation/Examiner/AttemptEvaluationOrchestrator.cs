using DataModel;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServicesClasseslibrary.Interface.Analytics;
using ServicesClasseslibrary.Interface.Examiner;
using ServicesClasseslibrary.WorkerServices.ExamEvaluation;
using ServicesClasseslibrary.WorkerServices.Interface.Examiner;
using ServicesClasseslibrary.WorkerServices.Messaging;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace ServicesClasseslibrary.WorkerServices.InterfaceImplementation.Examiner
{
    public class AttemptEvaluationOrchestrator : IEvaluationPublisher, IEvaluationNotifier
    {
        private readonly IServiceScopeFactory scopes;
        private readonly ILogger<AttemptEvaluationOrchestrator> logger;
        private readonly IAnalyticsQueue analytics;
        private readonly IEvaluationResultQueue results;
        private readonly IHubContext<EvaluationHub> hub;
        private readonly RabbitMqConsumer consumer;
        public AttemptEvaluationOrchestrator(ILogger<AttemptEvaluationOrchestrator> logger, IAnalyticsQueue analytics,
            IEvaluationResultQueue results, IServiceScopeFactory scopes, IHubContext<EvaluationHub> hub, RabbitMqConsumer consumer)
        { this.logger = logger; this.analytics = analytics; this.results = results; this.scopes = scopes; this.hub = hub; this.consumer = consumer; }
        public async Task EvaluateExamAttemptsAsync()
        {
            using var scope = scopes.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IExaminerAttemptService>();
            var evaluated = await service.EvaluatePending((id, ex) => logger.LogError(ex, "Evaluation failed for attempt {AttemptId}", id));
            foreach (var attempt in evaluated.DistinctBy(a => a.Item1))
            {
                analytics.Enqueue(new AnalyticsJob { ExaminerId = attempt.Item2, TriggerAttemptId = attempt.Item1 });
                await results.Enqueue(attempt);
            }
        }
        public Task SendOneBatchAsync(CancellationToken ct) => consumer.RunAsync(
            new[] { RabbitMqTopology.PassedQueue, RabbitMqTopology.FailedQueue }, async body =>
            {
                var result = JsonConvert.DeserializeObject<Tuple<int, int, decimal>>(Encoding.UTF8.GetString(body.Span))
                    ?? throw new JsonSerializationException("Empty evaluation result.");
                await hub.Clients.Group($"candidate_{result.Item2}").SendAsync("EvaluationCompleted", result.Item1, ct);
            }, ct);
    }
}
