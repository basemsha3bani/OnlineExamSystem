using DataModel;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog.Core;
using ServicesClasseslibrary.Implmentation.Analytics;
using ServicesClasseslibrary.Interface.Analytics;
using ServicesClasseslibrary.Interface.Examiner;
using ServicesClasseslibrary.WorkerServices.ExamEvaluation;
using ServicesClasseslibrary.WorkerServices.Interface.Examiner;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ServicesClasseslibrary.WorkerServices.InterfaceImplementation.Examiner
{
    public class AttemptEvaluationOrchestrator : IEvaluationPublisher, IEvaluationNotifier
    {
        private readonly IServiceScopeFactory _scopes;
        private readonly ILogger<AttemptEvaluationOrchestrator> _logger;
        private readonly IAnalyticsQueue _analyticsQueue;
        private readonly IEvaluationResultQueue _evaluationResultQueue;
        private readonly IHubContext<EvaluationHub> _hubContext;
        private readonly IChannel _channel;
        private AsyncEventingBasicConsumer consumer; 
        public AttemptEvaluationOrchestrator(
            IConnection connection,
            IChannel channel,
            ILogger<AttemptEvaluationOrchestrator> logger,
            IAnalyticsQueue analyticsQueue,
            IEvaluationResultQueue evaluationResultQueue,
            IServiceScopeFactory scopes,
            IHubContext<EvaluationHub> hubContext)
        {
            _logger = logger; _analyticsQueue = analyticsQueue;
            _evaluationResultQueue = evaluationResultQueue;
            _scopes = scopes; _hubContext = hubContext;
            _channel = connection.CreateChannelAsync().GetAwaiter().GetResult();

            _channel.QueueDeclareAsync(queue: "exam.results.passed.queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclareAsync(queue: "exam.results.failed.queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            _channel.QueueBindAsync("exam.results.passed.queue", "exam.results.exchange", "result.passed");
            _channel.QueueBindAsync("exam.results.failed.queue", "exam.results.exchange", "result.failed");
            consumer = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicConsumeAsync(queue: "exam.results.passed.queue", autoAck: false, consumer: consumer);
            _channel.BasicConsumeAsync(queue: "exam.results.failed.queue", autoAck: false, consumer: consumer);

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
                   await _evaluationResultQueue.Enqueue(attempt);
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
            
            consumer.ReceivedAsync += async (s, ea) =>
            {
                try
                {
                    var routingKey = ea.RoutingKey; // result.passed or result.failed
                    var json = System.Text.Encoding.UTF8.GetString(ea.Body.Span);
                    var evaluatedExam = JsonConvert.DeserializeObject<Tuple<int, int,decimal>>(json);
                    string resultMessage = "EvaluationCompleted"; 
                   
                        await _hubContext.Clients.Group($"candidate_{evaluatedExam.Item2}")
                       .SendAsync($"{resultMessage}", evaluatedExam.Item1, ct);
                 
                    
                        await _channel.BasicAckAsync(ea.DeliveryTag, false, ct);
                }
                catch
                {
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true, ct);
                }
            };
            await _channel.BasicQosAsync(0, 20, false, ct);
            await _channel.BasicConsumeAsync("exam-results", false, consumer, ct);
           
        }
    }
  


}

   