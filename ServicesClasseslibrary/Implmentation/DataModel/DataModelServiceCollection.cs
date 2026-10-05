using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.Implmentation.DataModel
{
    internal static class DataModelServiceCollection
    {
       
            public static IServiceCollection AddDataModelServices(this IServiceCollection services)
        
            {

                services.AddScoped<IDifficultyLevelsOperations, DifficultyLevelsOperations>();
                services.AddScoped<IQuestionsOperations, QuestionsOperations>();
                services.AddScoped<IStudySubjectsOperations, StudySubjectsOperations>();
                services.AddScoped<IExamOprations, ExamsOperations>();
                services.AddScoped<IExamSectionsOperations, ExamSectionsOperations>();
                services.AddScoped<IUserOperations, UserOperations>();
            services.AddScoped<IExamAttemptOprations, ExamAttemptOprations>();
            services.AddScoped<IExamSectionRuleOperations, ExamSectionRulesOperations>();
                services.AddScoped(typeof(ContextGateway<>));
                return services;
            }
    }
}
