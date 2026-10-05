using System.Threading.Tasks;

namespace ServicesClasseslibrary.Interface.Analytics
{
    public interface IAnalyticsService
    {
        Task RecalculateAsync(int UserId, int AttemptId);
    }
}
