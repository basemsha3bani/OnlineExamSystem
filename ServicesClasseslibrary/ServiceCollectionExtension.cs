//using DataRepository.ModelMappers;
//using DataRepository.ModelMappers.Interface;
using DataRepository;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses.DataRepository.DataRepositoryEntities;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using static DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses.ExamsOperations;


namespace ServicesClasseslibrary
{
    public static  class ServiceCollectionExtension
    {
        public static IServiceCollection AddServicesOnWhichServiceClassLibaryDepend(this IServiceCollection services)
        {
            services.AddScoped(typeof(ContextGateway<>));
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

            services.AddScoped<IDifficultyLevelsOperations, DifficultyLevelsOperations>();
            services.AddScoped<IQuestionsOperations, QuestionsOperations>();
            services.AddScoped<IStudySubjectsOperations, StudySubjectsOperations>();
            services.AddScoped<IExamOprations, ExamsOperations>();
            services.AddScoped<IExamSectionsOperations, ExamSectionsOperations>();
            services.AddScoped<IUserOperations, UserOperations>();
            services.AddScoped<IExamSectionRuleOperations, ExamSectionRulesOperations>();




            return services;
        }
       
    }
}
