using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
namespace ServicesClasseslibrary.WorkerServices.Messaging
{
    public interface IRabbitMqPublisher
    {
        void Enqueue(string exchange, string routingKey, byte[] body);
    }
    public sealed class RabbitMqPublisher : BackgroundService, IRabbitMqPublisher
    {
        private sealed record Message(string Exchange, string RoutingKey, byte[] Body);
        private readonly ConcurrentQueue<Message> pending = new();
        private readonly IRabbitMqConnectionProvider connections;
        private readonly ILogger<RabbitMqPublisher> logger;
        private readonly BrokerWorkerOptions options;
        public int PendingCount => pending.Count;
        public RabbitMqPublisher(IRabbitMqConnectionProvider connections, ILogger<RabbitMqPublisher> logger, BrokerWorkerOptions options)
        { this.connections = connections; this.logger = logger; this.options = options; }
        public void Enqueue(string exchange, string routingKey, byte[] body) => pending.Enqueue(new(exchange, routingKey, body));
        protected override Task ExecuteAsync(CancellationToken ct) => BackgroundWorkerLoop.RunAsync(PublishAsync, logger, options.PublishIdleDelay, options.RetryDelay, ct);
        private async Task PublishAsync(CancellationToken ct)
        {
            await using var channel = await connections.OpenChannelAsync(true, ct);
            await RabbitMqTopology.DeclareAsync(channel, ct);
            logger.LogInformation("RabbitMQ publisher connected");
            while (!ct.IsCancellationRequested)
            {
                if (!channel.IsOpen) throw new System.IO.IOException("RabbitMQ publisher channel closed.");
                if (pending.TryPeek(out var message))
                {
                    var properties = new BasicProperties { Persistent = true };
                    await channel.BasicPublishAsync(message.Exchange, message.RoutingKey, mandatory: true,
                        basicProperties: properties, body: message.Body, cancellationToken: ct);
                    // Keep failed/unconfirmed messages queued for the next connection.
                    pending.TryDequeue(out _);
                }
                else await Task.Delay(options.PublishIdleDelay, ct);
            }
        }
    }
}
