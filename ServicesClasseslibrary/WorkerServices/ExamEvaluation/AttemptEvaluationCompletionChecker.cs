using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServicesClasseslibrary.Interface.Examiner;
using ServicesClasseslibrary.WorkerServices.Interface.Examiner;
using System.Threading;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.WorkerServices.ExamEvaluation
{
    public class AttemptEvaluationCompletionChecker : BackgroundService
    {

        private readonly IServiceScopeFactory scopes;


        public AttemptEvaluationCompletionChecker(IServiceScopeFactory scopes)
        {
            { this.scopes = scopes; }
        }
        
        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                using var scope = scopes.CreateScope();
                var orchestrator = scope.ServiceProvider.GetRequiredService<IEvaluationNotifier>();
                await orchestrator.SendOneBatchAsync(ct);
                
            }
        }
    }

}
