using DataModel;
using DataRepository.DataRepositoryEntities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServicesClasseslibrary.Implmentation.Analytics;
using ServicesClasseslibrary.Implmentation.Examiner;
using ServicesClasseslibrary.Interface.Analytics;
using ServicesClasseslibrary.Interface.Examiner;
using ServicesClasseslibrary.WorkerServices.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.WorkerServices.ExamEvaluation
{
    public class AttemptEvaluationWorker : BackgroundService
    {
        private readonly ILogger<AttemptEvaluationWorker> logger;
        private readonly IServiceScopeFactory scopes;
        private IAnalyticsQueue analyticsQueue;
        private readonly IHubContext<EvaluationHub> _hubContext;
        public AttemptEvaluationWorker(ILogger<AttemptEvaluationWorker> logger,IAnalyticsQueue analyticsQueue,  IServiceScopeFactory scopes,
            IHubContext<EvaluationHub> hubContext) { this.logger = logger; this.scopes = scopes;this.analyticsQueue = analyticsQueue;_hubContext=hubContext ;}
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Yield before database work so application startup is not blocked.
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                try
                {
                    using var scope = scopes.CreateScope();
                    var evaluationService = scope.ServiceProvider.GetRequiredService<IExaminerAttemptService>();

                    var evaluatedAttempts=   evaluationService.EvaluatePending((id, ex) => logger.LogError(ex, "Evaluation failed for attempt {AttemptId}; it will be retried.", id));
                    foreach (var examattempt in evaluatedAttempts.Distinct())
                    {
                        await _hubContext.Clients.Group($"candidate_{examattempt.Item2}")
                        .SendAsync("EvaluationCompleted", examattempt.Item1);
                        analyticsQueue.Enqueue(new AnalyticsJob
                        {
                            ExaminerId = examattempt.Item2,
                            TriggerAttemptId = examattempt.Item1 // or last id for idempotency
                        });
                        logger.LogInformation("Enqueued analytics for examiner {Id}", examattempt.Item2);
                    }
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error,ex.Message);
                    //.LogError(ex, "Attempt evaluation failed; pending attempts will be retried."); }
                }
            }
        }
    }
}
