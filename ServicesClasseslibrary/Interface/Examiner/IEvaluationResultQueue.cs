using System;
using System.Collections.Concurrent;

namespace ServicesClasseslibrary.Interface.Examiner
{
    public interface IEvaluationResultQueue
    {
        void Enqueue(Tuple<int,int> item);
        bool TryDequeue(Tuple<int, int>  item);

        ConcurrentQueue<Tuple<int, int>> QueueItems();
    }
}
