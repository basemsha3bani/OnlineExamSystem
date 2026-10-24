using RabbitMQ.Client;
using System.Threading;
using System.Threading.Tasks;
namespace ServicesClasseslibrary.WorkerServices.Messaging
{
    public static class RabbitMqTopology
    {
        public const string ResultsExchange = "exam.results.exchange";
        public const string PassedQueue = "exam.results.passed.queue";
        public const string FailedQueue = "exam.results.failed.queue";
        public const string LogsQueue = "exam-logs";
        public static async Task DeclareAsync(IChannel channel, CancellationToken ct)
        {
            await channel.ExchangeDeclareAsync(ResultsExchange, ExchangeType.Topic, durable: true, autoDelete: false, cancellationToken: ct);
            foreach (var queue in new[] { PassedQueue, FailedQueue, LogsQueue })
                await channel.QueueDeclareAsync(queue, durable: true, exclusive: false, autoDelete: false, cancellationToken: ct);
            await channel.QueueBindAsync(PassedQueue, ResultsExchange, "result.passed", cancellationToken: ct);
            await channel.QueueBindAsync(FailedQueue, ResultsExchange, "result.failed", cancellationToken: ct);
        }
    }
}
