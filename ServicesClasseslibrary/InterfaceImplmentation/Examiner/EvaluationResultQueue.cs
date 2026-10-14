using DataModel;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServicesClasseslibrary.Interface.Analytics;
using ServicesClasseslibrary.Interface.Examiner;
using ServicesClasseslibrary.WorkerServices.ExamEvaluation;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.InterfaceImplmentation.Examiner
{

    public class EvaluationResultQueue : IEvaluationResultQueue
    {
        private readonly ConcurrentQueue<Tuple<int, int>> _queue = new();
        public async Task Enqueue(Tuple<int, int> item)
        {
            await Task.Run(() => _queue.Enqueue(item));
        }
        public bool TryDequeue(Tuple<int, int> item)
        {
            _queue.TryDequeue(out item);

            return true;
        }
        public ConcurrentQueue<Tuple<int, int>> QueueItems()
        {
            return _queue;
        }
    }
          
        
    

       
       
      

        
    }