using Domain.DTOs;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.Interface.Analytics
{
    public interface IAnalyticsService
    {
        Task RecalculateAsync(AnalyticsJobDTO job);
    }
}
