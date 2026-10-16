using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Abstractions;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ServicesClasseslibrary.Interface.Logging;
using System;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.WorkerServices.Logging
{
    public class LogBackGroundService : BackgroundService
    {
        
        private readonly IChannel _channel;
        
        ILogger<LogginqQueueProcessor> _logger;
        public LogBackGroundService(IChannel channel, ILogger<LogginqQueueProcessor> logger)
        {
            _channel = channel;
            _logger= logger;

        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
           await _channel.QueueDeclareAsync(
        queue: "exam-logs",
        durable: true,
        exclusive: false,
        autoDelete: false,
        arguments: null
    );
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (s, ea) =>
            {
                try
                {
                    var json = System.Text.Encoding.UTF8.GetString(ea.Body.Span);
                    var logEntry = JsonConvert.DeserializeObject<LogEntry>(json);

                    _logger.Log((LogLevel)logEntry.EventLogLevel, default, logEntry.Message, null, (state, ex) => state ?? string.Empty);
                    await _channel.BasicAckAsync(ea.DeliveryTag, false, ct);
                }
                catch
                {
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true, ct);
                }
            };
            await _channel.BasicQosAsync(0, 20, false, ct);
            await _channel.BasicConsumeAsync("exam-logs", false, consumer, ct);
            await Task.Delay(Timeout.Infinite, ct);
        }
    }

}
