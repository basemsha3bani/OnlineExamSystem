using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.WorkerServices.Interface.Examiner
{
    public interface IEvaluationResultQueue
    {
        Task Enqueue(Tuple<int, int, decimal> item);
        bool TryDequeue(Tuple<int, int, decimal>  item);

        ConcurrentQueue<Tuple<int, int, decimal>> QueueItems();
    }
}
