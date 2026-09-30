using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServicesClasseslibrary.Examiner;
using Microsoft.Extensions.DependencyInjection;

namespace OnlineExamSystem.Services
{
    public class AttemptEvaluationWorker : BackgroundService
    {
        private readonly ILogger<AttemptEvaluationWorker> logger;
        private readonly IServiceScopeFactory scopes;
        public AttemptEvaluationWorker(ILogger<AttemptEvaluationWorker> logger, IServiceScopeFactory scopes) { this.logger = logger; this.scopes = scopes; }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Yield before database work so application startup is not blocked.
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                try
                {
                    using var scope = scopes.CreateScope();
                    scope.ServiceProvider.GetRequiredService<ExaminerAttemptService>().EvaluatePending((id, ex) => logger.LogError(ex, "Evaluation failed for attempt {AttemptId}; it will be retried.", id));
                }
                catch (Exception ex) { logger.LogError(ex, "Attempt evaluation failed; pending attempts will be retried."); }
            }
        }
    }
}
