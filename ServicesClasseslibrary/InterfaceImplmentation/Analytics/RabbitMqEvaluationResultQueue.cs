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
using System.Threading.Tasks;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace ServicesClasseslibrary.Implmentation.Analytics
{
    public sealed class RabbitMqEvaluationResultQueue : IEvaluationResultQueue, IAsyncDisposable
    {
        private readonly IConnection _conn;
        private readonly IChannel _channel;
        private readonly ConcurrentQueue<Tuple<int, int>> _buffer = new(); // local prefetch buffer
        public RabbitMqEvaluationResultQueue(IChannel channel) => _channel = channel;
        
        

       

       

        public int Count => _buffer.Count;

        public async ValueTask DisposeAsync()
        {
            await _channel.CloseAsync();
            await _conn.CloseAsync();
        }

        public async Task Enqueue(Tuple<int, int> item)
        {
            byte[] body;
            using (var ms = new System.IO.MemoryStream())
            using (var sw = new System.IO.StreamWriter(ms))
            using (var jsonWriter = new JsonTextWriter(sw))
            {
                Newtonsoft.Json.JsonSerializer.CreateDefault().Serialize(jsonWriter, item);
                jsonWriter.Flush();
                sw.Flush();
                body = ms.ToArray();
            }
            var props = new BasicProperties { Persistent = true }; // durable

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: "exam-results",
                mandatory: false,
                basicProperties: props,
                body: body);
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
