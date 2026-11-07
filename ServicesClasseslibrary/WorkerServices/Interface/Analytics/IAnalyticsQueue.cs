using Domain.DTOs;

namespace ServicesClasseslibrary.WorkerServices.Interface.Analytics
{
   public interface IAnalyticsQueue
    {
        void Enqueue(AnalyticsJobDTO job);
        bool TryDequeue(out AnalyticsJobDTO job);
    }

}
