using AutoMapper;
using DataRepository.Builders;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
using DataRepository.GateWay;
using DataRepository.ModelMapper;
using Domain.Aggregates;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnlineExamSystem.Application.Interface.Builder;
using OnlineExamSystem.Application.Interface.DataRepository;
using OnlineExamSystem.Application.Interface.Events;
using OnlineExamSystem.Application.Interface.Logging;
using OnlineExamSystem.InfraStructure.Events;
using ServicesClasseslibrary.Implmentation.Logging;
using System;

namespace DataRepository;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfraStructure(this IServiceCollection services)
    {
        services.AddScoped<IDifficultyLevelsOperations, DifficultyLevelsOperations>();
        services.AddScoped<IQuestionsOperations, QuestionsOperations>();
        services.AddScoped<IStudySubjectsOperations, StudySubjectsOperations>();
        services.AddScoped<IExamOprations, ExamsOperations>();
        services.AddScoped<IExamSectionsOperations, ExamSectionsOperations>();
        services.AddScoped<IUserOperations, UserOperations>();
        services.AddScoped<IExamAttemptOprations, ExamAttemptOprations>();
        services.AddScoped<IExamSectionRuleOperations, ExamSectionRulesOperations>();
        services.AddScoped<IExaminerPerformanceOperations, ExaminerPerformanceOperations>();
        services.AddScoped(typeof(ContextGateway<>));
        services.AddScoped<IExamAttemptQuestionBuilder, ExamAttemptQuestionBuilder>();
        services.AddConnectionStringConfiguration();
        services.AddSingleton<MapperConfiguration>(provider =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<RepositoryMappingProfile>();
                cfg.LicenseKey = Environment.GetEnvironmentVariable("AUTOMAPPER_LICENSE_KEY");
            }, provider.GetRequiredService<ILoggerFactory>());
            config.AssertConfigurationIsValid();
            return config;
        });
        services.AddSingleton<IEventBus, InMemoryEventBus>();
        services.AddScoped<IMapper>(provider => provider.GetRequiredService<MapperConfiguration>().CreateMapper());
        services.AddScoped<ILoggingQueue, LoggingQueue>();
        return services;
    }
}
