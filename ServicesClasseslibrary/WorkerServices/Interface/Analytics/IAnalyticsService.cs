using Domain.DTOs;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.WorkerServices.Interface.Analytics
{
    interface IAnalyticsService
    {
        Task RecalculateAsync(AnalyticsJobDTO job);
    }

}
