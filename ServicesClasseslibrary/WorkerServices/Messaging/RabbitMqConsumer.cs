using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace ServicesClasseslibrary.WorkerServices.Messaging
{
    public sealed class RabbitMqConsumer
    {
        private readonly IRabbitMqConnectionProvider connections;
        private readonly ILogger<RabbitMqConsumer> logger;
        public RabbitMqConsumer(IRabbitMqConnectionProvider connections, ILogger<RabbitMqConsumer> logger)
        { this.connections = connections; this.logger = logger; }
        public async Task RunAsync(string[] queues, Func<ReadOnlyMemory<byte>, Task> handle, CancellationToken ct)
        {
            await using var channel = await connections.OpenChannelAsync(false, ct);
            await RabbitMqTopology.DeclareAsync(channel, ct);
            await channel.BasicQosAsync(0, 20, false, ct);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (_, delivery) =>
            {
                try
                {
                    await handle(delivery.Body);
                    await channel.BasicAckAsync(delivery.DeliveryTag, false, ct);
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested) { }
                catch (Exception ex)
                {
                    logger.LogError(ex, "RabbitMQ delivery failed on {RoutingKey}", delivery.RoutingKey);
                    try
                    {
                        if (channel.IsOpen && !ct.IsCancellationRequested)
                            await channel.BasicNackAsync(delivery.DeliveryTag, false, ex is not Newtonsoft.Json.JsonException, ct);
                    }
                    catch (Exception nackError) { logger.LogWarning(nackError, "RabbitMQ acknowledgement failed; delivery will be retried on reconnect"); }
                }
            };
            foreach (var queue in queues) await channel.BasicConsumeAsync(queue, false, consumer, ct);
            logger.LogInformation("RabbitMQ consumer connected for {Queues}", string.Join(", ", queues));
            while (!ct.IsCancellationRequested)
            {
                if (!channel.IsOpen) throw new System.IO.IOException("RabbitMQ consumer channel closed.");
                await Task.Delay(1000, ct);
            }
        }
    }
}
