using DataModel;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServicesClasseslibrary.Interface.Analytics;
using ServicesClasseslibrary.Interface.Examiner;
using ServicesClasseslibrary.WorkerServices.ExamEvaluation;
using ServicesClasseslibrary.WorkerServices.Interface.Examiner;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.WorkerServices.InterfaceImplementation.Examiner
{

    public class EvaluationResultQueue : IEvaluationResultQueue
    {
        private readonly ConcurrentQueue<Tuple<int, int,decimal>> _queue = new();
        public async Task Enqueue(Tuple<int, int, decimal> item)
        {
            await Task.Run(() => _queue.Enqueue(item));
        }
        public bool TryDequeue(Tuple<int, int,decimal> item)
        {
            _queue.TryDequeue(out item);

            return true;
        }
        public ConcurrentQueue<Tuple<int, int, decimal>> QueueItems()
        {
            return _queue;
        }
    }
          
        
    

       
       
      

        
    }