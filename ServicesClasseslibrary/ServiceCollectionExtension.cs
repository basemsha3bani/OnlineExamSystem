using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
//using DataRepository.ModelMappers;
//using DataRepository.ModelMappers.Interface;
using DataRepository;
using DataRepository.GateWay;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;


namespace ServicesClasseslibrary
{
    public static  class ServiceCollectionExtension
    {
        public static IServiceCollection AddServicesOnWhichServiceClassLibaryDepend(this IServiceCollection services)
        {

            services.AddScoped<IDifficultyLevelsOperations, DifficultyLevelsOperations>();
            services.AddScoped<IQuestionsOperations, QuestionsOperations>();
         
            //services.AddScoped<ISystemSettingsModelMapper, SystemSettingsModelMapper>();
            //services.AddScoped<IRegistrarsModelMapper, RegistrarsModelMapper>();
            //services.AddScoped<IVaccinationTypesModelMapper, VaccinationTypesModelMapper>();
            //services.AddScoped<IVaccinationAppointmentModelMapper, VaccinationAppointmentModelMapper>();
            //services.AddScoped<RecordListInterface<RegistrarsRepository>, RepositoryGateWay<RegistrarsRepository>>();
            //services.AddScoped<RecordListInterface<VaccinationTypesRepository>, RepositoryGateWay<VaccinationTypesRepository>>();
            //services.AddScoped<RecordListInterface<VaccinationReservationRepository>, RepositoryGateWay<VaccinationReservationRepository>>();
            //services.AddScoped<RecordListInterface<SystemSettingsRepository>, RepositoryGateWay<SystemSettingsRepository>>();

            return services;
        }
       
    }
}
