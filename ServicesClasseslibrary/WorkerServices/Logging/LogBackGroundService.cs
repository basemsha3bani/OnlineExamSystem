using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Abstractions;
using Newtonsoft.Json;
using ServicesClasseslibrary.WorkerServices.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace ServicesClasseslibrary.WorkerServices.Logging
{
    public class LogBackGroundService : BackgroundService
    {
        private readonly RabbitMqConsumer consumer;
        private readonly ILogger<LogBackGroundService> logger;
        private readonly BrokerWorkerOptions options;
        public LogBackGroundService(RabbitMqConsumer consumer, ILogger<LogBackGroundService> logger, BrokerWorkerOptions options)
        { this.consumer = consumer; this.logger = logger; this.options = options; }
        protected override Task ExecuteAsync(CancellationToken ct) => BackgroundWorkerLoop.RunAsync(
            token => consumer.RunAsync(new[] { RabbitMqTopology.LogsQueue }, body =>
            {
                var entry = JsonConvert.DeserializeObject<LogEntry>(Encoding.UTF8.GetString(body.Span))
                    ?? throw new JsonSerializationException("Empty log entry.");
                logger.Log((LogLevel)entry.EventLogLevel, "{Message}", entry.Message);
                return Task.CompletedTask;
            }, token), logger, options.EvaluationInterval, options.RetryDelay, ct);
    }
}
