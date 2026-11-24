using Domain.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnlineExamSystem.Application.Interface.Events;
using ServicesClasseslibrary.WorkerServices.Interface.Analytics;
using ServicesClasseslibrary.WorkerServices.Interface.Examiner;
using ServicesClasseslibrary.WorkerServices.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace ServicesClasseslibrary.WorkerServices.ExamEvaluation
{
    public class AttemptEvaluationCompletionChecker : BackgroundWorkerLoop
    {
        private readonly IServiceScopeFactory scopes;
        private readonly ILogger<AttemptEvaluationCompletionChecker> logger;
        private readonly BrokerWorkerOptions options;
        private readonly IEvaluationResultQueue results;
        IAnalyticsQueue analyticsQueue;
        public AttemptEvaluationCompletionChecker(IServiceScopeFactory scopes, ILogger<AttemptEvaluationCompletionChecker> logger, BrokerWorkerOptions options, IEvaluationResultQueue results, IAnalyticsQueue analytics) : base(logger, options.EvaluationInterval, options.RetryDelay)
        {
            this.scopes = scopes; this.logger = logger; this.options = options; this.results = results; this.analyticsQueue = analytics;
        }

        private bool subscribed;



        protected override async Task RunAsync(CancellationToken ct)
        {
            using var scope = scopes.CreateScope();
            if (!subscribed)
            {
                var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
                await eventBus.SubscribeAsync<ExamGradedEvent>(evt =>
                {
                    results.Enqueue(new System.Tuple<int, int, decimal>(evt.AttemptId, evt.ExaminerId, evt.score));
                    analyticsQueue.Enqueue(AnalyticsJobDTO.Define(evt.AttemptId, evt.ExaminerId));

                    subscribed = true;
                    return Task.CompletedTask;
                });
            }
            await scope.ServiceProvider.GetRequiredService<IEvaluationPublisher>().PublishEvaluatedExams();
            //await this.RunNotificationsAsync(ct);
        }
    }
        public class AttemptEvaluationsCompletionNotifier:BackgroundWorkerLoop
        {
            private readonly IServiceScopeFactory scopes;   
        public AttemptEvaluationsCompletionNotifier(ILogger<AttemptEvaluationsCompletionNotifier> logger, BrokerWorkerOptions options, IServiceScopeFactory scopes) : base(logger, options.EvaluationInterval,options.RetryDelay)
            {
                this.scopes = scopes;
        }

            protected override async Task RunAsync(CancellationToken ct)
            {
                await this.RunNotificationsAsync(ct);
            }

            // The notifier owns a long-lived consumer seon, so it must not block publishing.
            private async Task RunNotificationsAsync(CancellationToken ct)
        {
            
                try
                {
                    using var scope = scopes.CreateScope();
                    await scope.ServiceProvider.GetRequiredService<IEvaluationNotifier>().SendOneBatchAsync(ct);
                   
                }
                //catch (System.OperationCanceledException) when (ct.IsCancellationRequested) { }
                catch (System.Exception ex) { await HandleExceptionAsync(ex, ct); }
           
        }
    }
}