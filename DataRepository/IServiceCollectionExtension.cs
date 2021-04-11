using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using DataRepository.GateWay;
using DataRepository;



namespace ServicesClasseslibrary
{
    public static  class IServiceCollectionExtension
    {
        public static object AddServicesOnWhichModelMapperDepend(this IServiceCollection services)
        {


            services.AddScoped<RecordListInterface<RegistrarsRepository>, RepositoryGateWay<RegistrarsRepository>>();
            services.AddScoped<RecordListInterface<VaccinationTypesRepository>, RepositoryGateWay<VaccinationTypesRepository>>();
            services.AddScoped<RecordListInterface<VaccinationReservationRepository>, RepositoryGateWay<VaccinationReservationRepository>>();
           services.AddScoped<RecordListInterface<SystemSettingsRepository>, RepositoryGateWay<SystemSettingsRepository>>();
         
            return services;
        }
       
    }
}
