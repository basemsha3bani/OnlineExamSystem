using Microsoft.Extensions.DependencyInjection;
using OnlineExamSystem.Application.Implmentation.Analytics;
using ServicesClasseslibrary.WorkerServices.Interface.Analytics;


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
