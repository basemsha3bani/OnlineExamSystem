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
using ServicesClasseslibrary.WorkerServices.Interface.Examiner;

namespace ServicesClasseslibrary.WorkerServices.InterfaceImplementation.Examiner
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddExaminerServices(this IServiceCollection services)
        {
            services.AddSingleton<ServicesClasseslibrary.WorkerServices.Messaging.BrokerWorkerOptions>();
            services.AddSingleton<IConnectionFactory>(sp =>
            {
                var config = sp.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
                var factory = new ConnectionFactory {
                    HostName = config["RabbitMq:Host"] ?? "localhost",
                    // Each retrying worker owns channel recovery, avoiding duplicate recovered consumers.
                    AutomaticRecoveryEnabled = false,
                    RequestedConnectionTimeout = System.TimeSpan.FromSeconds(5)
                };
                if (int.TryParse(config["RabbitMq:Port"], out var port)) factory.Port = port;
                if (!string.IsNullOrWhiteSpace(config["RabbitMq:UserName"])) factory.UserName = config["RabbitMq:UserName"];
                if (!string.IsNullOrWhiteSpace(config["RabbitMq:Password"])) factory.Password = config["RabbitMq:Password"];
                if (!string.IsNullOrWhiteSpace(config["RabbitMq:VirtualHost"])) factory.VirtualHost = config["RabbitMq:VirtualHost"];
                return factory;
            });
            services.AddSingleton<ServicesClasseslibrary.WorkerServices.Messaging.IRabbitMqConnectionProvider, ServicesClasseslibrary.WorkerServices.Messaging.RabbitMqConnectionProvider>();
            services.AddSingleton<ServicesClasseslibrary.WorkerServices.Messaging.RabbitMqPublisher>();
            services.AddSingleton<ServicesClasseslibrary.WorkerServices.Messaging.IRabbitMqPublisher>(sp => sp.GetRequiredService<ServicesClasseslibrary.WorkerServices.Messaging.RabbitMqPublisher>());
            services.AddHostedService(sp => sp.GetRequiredService<ServicesClasseslibrary.WorkerServices.Messaging.RabbitMqPublisher>());
            services.AddSingleton<ServicesClasseslibrary.WorkerServices.Messaging.RabbitMqConsumer>();
            services.AddHostedService<AttemptEvaluationWorker>();
            services.AddHostedService<AttemptEvaluationCompletionChecker>();
            services.AddSignalR();
            services.AddScoped<IExaminerAttemptService, ExaminerAttemptService>();
            services.AddScoped<IExamAttemptQuestionBuilder, ExamAttemptQuestionBuilder>();
            services.AddSingleton<IEvaluationResultQueue, RabbitMqEvaluationResultQueue>();
            services.AddScoped<IEvaluationPublisher, AttemptEvaluationOrchestrator>();
            services.AddScoped<IEvaluationNotifier, AttemptEvaluationOrchestrator>();
          
            return services;
        }
    }
       
      
}
