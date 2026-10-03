using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ServicesClasseslibrary.Interface.Logging;
using ServicesClasseslibrary.Logging.Implementation;

namespace ServicesClasseslibrary.Logging.Sevices
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddLoggingService(this IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
           .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)

           .CreateLogger();
            services.AddSingleton<ILoggingQueue, LoggingQueue>();
            services.AddHostedService<LogBackGroundService>();
            services.AddScoped<LogginqQueueProcessor>();
            return services;
        }
    }
}
