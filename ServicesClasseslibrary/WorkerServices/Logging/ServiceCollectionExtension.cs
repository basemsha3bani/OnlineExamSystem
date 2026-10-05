using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ServicesClasseslibrary.Implmentation.Logging;
using ServicesClasseslibrary.Interface.Logging;


namespace ServicesClasseslibrary.WorkerServices.Logging
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddLoggingService(this IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
           .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)

           .CreateLogger();
            
            services.AddHostedService<LogBackGroundService>();
            services.AddScoped<ILoggingQueue, LoggingQueue>();
            services.AddScoped<LogginqQueueProcessor>();
            return services;
        }
    }
}
