using Microsoft.Extensions.DependencyInjection;

using ServicesClasseslibrary.WorkerServices.Analytics;
using ServicesClasseslibrary.WorkerServices.InterfaceImplementation.Examiner;
using ServicesClasseslibrary.WorkerServices.Logging;

namespace ServicesClasseslibrary;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddBackGroundServiceClasses(this IServiceCollection services)
    {
        
        services.AddAnalyticsService();
        services.AddExaminerServices();
        services.AddLoggingService();
        return services;
    }
}
