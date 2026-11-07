using Microsoft.Extensions.DependencyInjection;

namespace DataRepository.GateWay
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddConnectionStringConfiguration(this IServiceCollection services)
        {
            services.AddSingleton<DataBaseConnectionConfiguration>();
            return services;
        }

    }
}
