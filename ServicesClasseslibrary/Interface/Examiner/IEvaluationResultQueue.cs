using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.Interface.Examiner
{
    public interface IEvaluationResultQueue
    {
        Task Enqueue(Tuple<int,int> item);
        bool TryDequeue(Tuple<int, int>  item);

        ConcurrentQueue<Tuple<int, int>> QueueItems();
    }
}
