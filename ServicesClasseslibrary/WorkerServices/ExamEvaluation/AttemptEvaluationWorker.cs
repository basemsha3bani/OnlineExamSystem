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
using ServicesClasseslibrary.InterfaceImplmentation.Examiner;
using ServicesClasseslibrary.WorkerServices.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace ServicesClasseslibrary.WorkerServices.ExamEvaluation
{
    public class AttemptEvaluationWorker : BackgroundService
    {
        
        private readonly IServiceScopeFactory scopes;
        public AttemptEvaluationWorker( IServiceScopeFactory scopes)
            
        { this.scopes = scopes; }
        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                using var scope = scopes.CreateScope();
                var orchestrator = scope.ServiceProvider.GetRequiredService<IEvaluationPublisher>();
                await orchestrator.EvaluateExamAttemptsAsync();
                await Task.Delay(5000, ct);
            }
        }
    }

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
