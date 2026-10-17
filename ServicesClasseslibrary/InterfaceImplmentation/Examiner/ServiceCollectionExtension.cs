//using DataRepository.ModelMappers;
//using DataRepository.ModelMappers.Interface;
using DataRepository;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using Microsoft.AspNetCore.Builder;

using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Serilog;
using ServicesClasseslibrary.Implmentation.Builder;
using ServicesClasseslibrary.Implmentation.DataModel;
using ServicesClasseslibrary.Interface.Builder;
using ServicesClasseslibrary.Interface.Examiner;
using ServicesClasseslibrary.InterfaceImplmentation.Examiner;
using ServicesClasseslibrary.WorkerServices.ExamEvaluation;

namespace ServicesClasseslibrary.Implmentation.Examiner
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddExaminerServices(this IServiceCollection services)
        {
            QueueConfiguration queueConfiguration= new QueueConfiguration();    
            services.AddHostedService<AttemptEvaluationWorker>();
            services.AddHostedService<AttemptEvaluationCompletionChecker>(); 
            services.AddSignalR();
            services.AddScoped<IExaminerAttemptService, ExaminerAttemptService>();
            services.AddScoped<IExamAttemptQuestionBuilder, ExamAttemptQuestionBuilder>();
            services.AddSingleton<IConnection>(sp => {
                var factory = new ConnectionFactory
                {
                    HostName = queueConfiguration.GetQueueHostConfiguration(),
                    AutomaticRecoveryEnabled = true
                };
                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            });
            services.AddSingleton<IChannel>(sp => {
                return sp.GetRequiredService<IConnection>().CreateChannelAsync().GetAwaiter().GetResult();
            });

            services.AddSingleton<IEvaluationResultQueue, RabbitMqEvaluationResultQueue>();
            services.AddScoped<IEvaluationPublisher, AttemptEvaluationOrchestrator>();
            services.AddScoped<IEvaluationNotifier, AttemptEvaluationOrchestrator>();
          
            return services;
        }
    }
       
      
}
