using Domain.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OnlineExamSystem.Application.Interface.Events;
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
        private readonly IEvaluationResultQueue results;
        public AttemptEvaluationCompletionChecker(IServiceScopeFactory scopes, ILogger<AttemptEvaluationCompletionChecker> logger, BrokerWorkerOptions options, IEvaluationResultQueue results)
        { this.scopes = scopes; this.logger = logger; this.options = options; this.results = results; }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {

            var subscribed = false;
            var publishing = BackgroundWorkerLoop.RunAsync(async token =>
            {
                using var scope = scopes.CreateScope();
                if (!subscribed)
                {
                    var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
                    await eventBus.SubscribeAsync<ExamGradedEvent>(evt =>
                        results.Enqueue(new System.Tuple<int, int, decimal>(evt.AttemptId, evt.ExaminerId, evt.score)));
                    subscribed = true;
                }
                await scope.ServiceProvider.GetRequiredService<IEvaluationPublisher>().PublishEvaluatedExams();
            }, logger, options.EvaluationInterval, options.RetryDelay, ct);

            // The notifier owns a long-lived consumer session, so it must not block publishing.
            var notifications = BackgroundWorkerLoop.RunAsync(async token =>
            {
                using var scope = scopes.CreateScope();
                await scope.ServiceProvider.GetRequiredService<IEvaluationNotifier>().SendOneBatchAsync(token);
            }, logger, options.EvaluationInterval, options.RetryDelay, ct);

            await Task.WhenAll(publishing, notifications);



        }
    }
}
