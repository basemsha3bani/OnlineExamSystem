//using DataRepository.ModelMappers;
//using DataRepository.ModelMappers.Interface;
using DataRepository;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using Microsoft.Extensions.DependencyInjection;
using OnlineExamSystem.Services;
using Serilog;
using ServicesClasseslibrary.Implmentation.Builder;
using ServicesClasseslibrary.Implmentation.DataModel;
using ServicesClasseslibrary.Interface.Builder;
using ServicesClasseslibrary.Logging.Sevices;
using System;
using System.Collections.Generic;
using System.Text;
using static DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses.ExamsOperations;
using static OnlineExamSystem.Services.AttemptEvaluationWorker;


namespace ServicesClasseslibrary.Examiner
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddExaminerServices(this IServiceCollection services)
        {
            services.AddHostedService<AttemptEvaluationWorker>();
            services.AddScoped<ExaminerAttemptService>();
            services.AddScoped<IExamAttemptQuestionBuilder,ExamAttemptQuestionBuilder>();
            

            return services;
        }
    }
      
}
