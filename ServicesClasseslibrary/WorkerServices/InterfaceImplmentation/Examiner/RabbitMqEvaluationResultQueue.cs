using DataRepository;
using DevExpress.Utils.Filtering;
using DevExpress.Utils.Url;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ServicesClasseslibrary.Interface.Builder;
using ServicesClasseslibrary.Interface.Examiner;
using ServicesClasseslibrary.WorkerServices.Interface.Examiner;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace ServicesClasseslibrary.WorkerServices.InterfaceImplementation.Examiner
{
    public sealed class RabbitMqEvaluationResultQueue : IEvaluationResultQueue
    {
        private readonly IConnection _conn;
        private readonly IChannel _channel;
        private readonly ConcurrentQueue<Tuple<int, int,decimal>> _buffer = new(); // local prefetch buffer
      
        

        public RabbitMqEvaluationResultQueue(IChannel channel)
        {
           
            _channel= channel;

        }

        public async Task Enqueue(Tuple<int, int, decimal> item)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item));
            decimal passedThreshold = 0.5m; // Define the passing threshold
            var routingKey = item.Item3 >= passedThreshold ? "result.passed" : "result.failed";
            // New client 7.x properties
            var props = new BasicProperties
            {
                Persistent = true,
                DeliveryMode = DeliveryModes.Persistent
            };

            await _channel.BasicPublishAsync(
               exchange: "exam.results.exchange",
               
                routingKey: routingKey,
                mandatory: false,
                basicProperties: props,
                body: body
            );

            Console.WriteLine($"Sent to exam-results: {item.Item1}");
        }



        public bool TryDequeue(Tuple<int, int, decimal> item)
        {
            _buffer.TryDequeue(out item);
            return true;
        }

        public ConcurrentQueue<Tuple<int, int, decimal>> QueueItems()
        {
           
            return _buffer;
        }
    }
}
