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
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item));
            var props = new BasicProperties { Persistent = true };
            await _channel.BasicPublishAsync("", "exam-results", false, props, body);
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
