using Newtonsoft.Json;
using ServicesClasseslibrary.WorkerServices.Interface.Examiner;
using ServicesClasseslibrary.WorkerServices.Messaging;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
namespace ServicesClasseslibrary.WorkerServices.InterfaceImplementation.Examiner
{
    public sealed class RabbitMqEvaluationResultQueue : IEvaluationResultQueue
    {
        private readonly IRabbitMqPublisher publisher;
        private readonly ConcurrentQueue<Tuple<int, int, decimal>> buffer = new();
        public RabbitMqEvaluationResultQueue(IRabbitMqPublisher publisher) { this.publisher = publisher; }
        public Task Enqueue(Tuple<int, int, decimal> item)
        {
            publisher.Enqueue(RabbitMqTopology.ResultsExchange, item.Item3 >= 0.5m ? "result.passed" : "result.failed", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item)));
            return Task.CompletedTask;
        }
        public bool TryDequeue(Tuple<int, int, decimal> item) => buffer.TryDequeue(out item);
        public ConcurrentQueue<Tuple<int, int, decimal>> QueueItems() => buffer;
    }
}
