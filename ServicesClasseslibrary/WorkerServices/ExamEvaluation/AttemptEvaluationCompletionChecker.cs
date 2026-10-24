using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServicesClasseslibrary.WorkerServices.Interface.Examiner;
using ServicesClasseslibrary.WorkerServices.Messaging;
using System.Threading;
using System.Threading.Tasks;
namespace ServicesClasseslibrary.WorkerServices.ExamEvaluation
{
    public class AttemptEvaluationCompletionChecker : BackgroundService
    {
        private readonly IServiceScopeFactory scopes;
        private readonly ILogger<AttemptEvaluationCompletionChecker> logger;
        private readonly BrokerWorkerOptions options;
        public AttemptEvaluationCompletionChecker(IServiceScopeFactory scopes, ILogger<AttemptEvaluationCompletionChecker> logger, BrokerWorkerOptions options)
        { this.scopes = scopes; this.logger = logger; this.options = options; }
        protected override Task ExecuteAsync(CancellationToken ct) => BackgroundWorkerLoop.RunAsync(async token =>
        {
            using var scope = scopes.CreateScope();
            await scope.ServiceProvider.GetRequiredService<IEvaluationNotifier>().SendOneBatchAsync(token);
        }, logger, options.EvaluationInterval, options.RetryDelay, ct);
    }
}
