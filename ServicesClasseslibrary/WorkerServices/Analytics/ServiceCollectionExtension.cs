using Microsoft.Extensions.DependencyInjection;
using ServicesClasseslibrary.Implmentation.Analytics;
using ServicesClasseslibrary.Interface.Analytics;

namespace ServicesClasseslibrary.WorkerServices.Analytics
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddAnalyticsService(this IServiceCollection services)
        {
            services.AddSingleton<IAnalyticsQueue, AnalyticsQueue>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            services.AddHostedService<AnalyticsWorker>();
            return services;
        }
    }
}
