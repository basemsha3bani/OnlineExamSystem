//using DataRepository.ModelMappers;
//using DataRepository.ModelMappers.Interface;
using DataRepository;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
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

            services.AddScoped<IDifficultyLevelsOperations, DifficultyLevelsOperations>();
            services.AddScoped<IQuestionsOperations, QuestionsOperations>();
            services.AddScoped<IStudySubjectsOperations, StudySubjectsOperations>();
            services.AddScoped<IExamOprations, ExamsOperations>();
            services.AddScoped<IExamSectionsOperations, ExamSectionsOperations>();
            services.AddScoped<IUserOperations, UserOperations>();




            return services;
        }
       
    }
}
