using Microsoft.Extensions.DependencyInjection;

using Serilog;




namespace ServicesClasseslibrary.WorkerServices.Logging
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddLoggingService(this IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                                                  .CreateLogger();
          
            services.AddScoped<LogginqQueueProcessor>();
            services.AddHostedService<LogBackGroundService>();
           
            return services;
        }
    }
}
