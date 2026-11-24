using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServicesClasseslibrary.WorkerServices.Interface.Examiner;
using ServicesClasseslibrary.WorkerServices.Messaging;
using System.Threading;
using System.Threading.Tasks;
namespace ServicesClasseslibrary.WorkerServices.ExamEvaluation.JobExecuting
{
    public class AttemptEvaluationWorker : BackgroundWorkerLoop
    {
        private readonly IServiceScopeFactory scopes;
        
        public AttemptEvaluationWorker(IServiceScopeFactory scopes, ILogger<AttemptEvaluationWorker> logger, BrokerWorkerOptions options) : base(logger, options.EvaluationInterval, options.RetryDelay)
        { this.scopes = scopes; }
        protected override async Task RunAsync(CancellationToken ct)
        {
            using var scope = scopes.CreateScope();
            await scope.ServiceProvider.GetRequiredService<IExamAttemptEvaluator>().EvaluateExamAttemptsAsync();
        }
    }
}
