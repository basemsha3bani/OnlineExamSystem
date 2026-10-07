//using DataRepository.ModelMappers;
//using DataRepository.ModelMappers.Interface;
using DataRepository;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using Microsoft.AspNetCore.Builder;

using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ServicesClasseslibrary.Implmentation.Builder;
using ServicesClasseslibrary.Implmentation.DataModel;
using ServicesClasseslibrary.Interface.Builder;
using ServicesClasseslibrary.Interface.Examiner;
using ServicesClasseslibrary.WorkerServices.ExamEvaluation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using static DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses.ExamsOperations;
using static ServicesClasseslibrary.WorkerServices.ExamEvaluation.AttemptEvaluationWorker;

namespace ServicesClasseslibrary.Implmentation.Examiner
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddExaminerServices(this IServiceCollection services)
        {
            services.AddHostedService<AttemptEvaluationWorker>();
            services.AddSignalR();

            services.AddScoped<IExaminerAttemptService, ExaminerAttemptService>();
            services.AddScoped<IExamAttemptQuestionBuilder, ExamAttemptQuestionBuilder>();


            return services;
        }
    }
       
      
}
