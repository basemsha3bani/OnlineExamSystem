using DataRepository;
using DevExpress.Utils.Url;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ServicesClasseslibrary.Interface.Builder;
using ServicesClasseslibrary.Interface.Examiner;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace ServicesClasseslibrary.InterfaceImplmentation.Examiner
{
    public sealed class RabbitMqEvaluationResultQueue : IEvaluationResultQueue
    {
        private readonly IConnection _conn;
        private readonly IChannel _channel;
        private readonly ConcurrentQueue<Tuple<int, int>> _buffer = new(); // local prefetch buffer
      
        

        public RabbitMqEvaluationResultQueue(IConnection connection)
        {
            _channel = connection.CreateChannelAsync().GetAwaiter().GetResult();
            // DECLARE HERE - creates queue if not exists
            _channel.QueueDeclareAsync(
                queue: "exam-results",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            ).GetAwaiter().GetResult();
        }

        public async Task Enqueue(Tuple<int, int> item)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item));

            // New client 7.x properties
            var props = new BasicProperties
            {
                Persistent = true,
                DeliveryMode = DeliveryModes.Persistent
            };

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: "exam-results",
                mandatory: false,
                basicProperties: props,
                body: body
            );

            Console.WriteLine($"Sent to exam-results: {item.Item1}");
        }



        public bool TryDequeue(Tuple<int, int> item)
        {
            _buffer.TryDequeue(out item);
            return true;
        }

        public ConcurrentQueue<Tuple<int, int>> QueueItems()
        {
           
            return _buffer;
        }
    }
}
