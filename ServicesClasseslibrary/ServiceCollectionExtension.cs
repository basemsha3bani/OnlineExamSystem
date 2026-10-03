//using DataRepository.ModelMappers;
//using DataRepository.ModelMappers.Interface;
using DataRepository;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;

using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using Microsoft.Extensions.DependencyInjection;
using OnlineExamSystem.Services;
using Serilog;
using ServicesClasseslibrary.Examiner;
using ServicesClasseslibrary.Implmentation.DataModel;
using ServicesClasseslibrary.Interface;
using ServicesClasseslibrary.Interface.DataModel;
using ServicesClasseslibrary.Logging.Sevices;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using static DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses.ExamsOperations;


namespace ServicesClasseslibrary
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddServiceClasses(this IServiceCollection services)
        {
            services.AddServicesOnWhichServiceClassLibaryDepend();
            services.AddScoped<IDifficultyLevelsService, DifficultyLevelsService>();
            services.AddScoped<IQuestionsService, QuestionsService>();
            services.AddScoped<IStudySubjectsService, StudySubjectsService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IExamSectionService, ExamSectionService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
        private static IServiceCollection AddServicesOnWhichServiceClassLibaryDepend(this IServiceCollection services)
        {
            services.AddDataModelServices();
            services.AddLoggingService();
            services.AddExaminerServices();
            services.AddSingleton<AutoMapper.MapperConfiguration>(provider =>
            {
                var config = new AutoMapper.MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<DataRepository.ModelMapper.RepositoryMappingProfile>();
                    cfg.LicenseKey = Environment.GetEnvironmentVariable("AUTOMAPPER_LICENSE_KEY");
                }, provider.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>());
                config.AssertConfigurationIsValid();
                return config;
            });
            services.AddScoped<AutoMapper.IMapper>(provider => provider.GetRequiredService<AutoMapper.MapperConfiguration>().CreateMapper());






            return services;
        }
    }
      
}
