using DataModel;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using ServicesClasseslibrary.Implmentation.Analytics;
using ServicesClasseslibrary.Interface.Analytics;
using ServicesClasseslibrary.Interface.Examiner;
using ServicesClasseslibrary.WorkerServices.ExamEvaluation;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.InterfaceImplmentation.Examiner
{
    public class AttemptEvaluationOrchestrator : IEvaluationPublisher, IEvaluationNotifier
    {
        private readonly IServiceScopeFactory _scopes;
        private readonly ILogger<AttemptEvaluationOrchestrator> _logger;
        private readonly IAnalyticsQueue _analyticsQueue;
        private readonly IEvaluationResultQueue _evaluationResultQueue;
        private readonly IHubContext<EvaluationHub> _hubContext;

        public AttemptEvaluationOrchestrator(
            ILogger<AttemptEvaluationOrchestrator> logger,
            IAnalyticsQueue analyticsQueue,
            IEvaluationResultQueue evaluationResultQueue,
            IServiceScopeFactory scopes,
            IHubContext<EvaluationHub> hubContext)
        {
            _logger = logger; _analyticsQueue = analyticsQueue;
            _evaluationResultQueue = evaluationResultQueue;
            _scopes = scopes; _hubContext = hubContext;
        }

        public async Task EvaluateExamAttemptsAsync()
        {
            try
            {
                using var scope = _scopes.CreateScope();
                var evaluationService = scope.ServiceProvider.GetRequiredService<IExaminerAttemptService>();

                var evaluatedAttempts = await evaluationService.EvaluatePending(
                    (id, ex) => _logger.LogError(ex, "Evaluation failed for attempt {AttemptId}", id));

                // Senior fix: DistinctBy instead of GroupBy
                foreach (var attempt in evaluatedAttempts.DistinctBy(x => x.Item1))
                {
                    _evaluationResultQueue.Enqueue(attempt);
                    _analyticsQueue.Enqueue(new AnalyticsJob
                    {
                        ExaminerId = attempt.Item2,
                        TriggerAttemptId = attempt.Item1
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EvaluateExamAttempts failed");
            }
        }

        public async Task SendOneBatchAsync(CancellationToken ct)
        {
            foreach (var attempt in _evaluationResultQueue.QueueItems())
            {
                try
                {
                    await _hubContext.Clients.Group($"candidate_{attempt.Item2}")
                        .SendAsync("EvaluationCompleted", attempt.Item1, ct);
                    _evaluationResultQueue.TryDequeue(attempt);
                    _logger.LogInformation("Notified candidate for attempt {Id}", attempt.Item1);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Notify failed for {Id}, re-queuing", attempt.Item1);
                    _evaluationResultQueue.Enqueue(attempt); // retry
                    await Task.Delay(1000, ct);
                }
            }
        }
    }


}

   