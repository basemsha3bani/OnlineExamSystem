using Domain.Aggregates;
using Domain.Entities;
//using DataRepository.ModelMappers;
//using DataRepository.ModelMappers.Interface;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OnlineExamSystem.Application.Interface.Events;
using OnlineExamSystem.Application.Interface.Examiner;
using RabbitMQ.Client;
using Serilog;
using ServicesClasseslibrary.WorkerServices.ExamEvaluation;
using ServicesClasseslibrary.WorkerServices.ExamEvaluation.JobCompletion.EventBus;
using ServicesClasseslibrary.WorkerServices.ExamEvaluation.JobExecuting;
using ServicesClasseslibrary.WorkerServices.Interface.Examiner;
using ServicesClasseslibrary.WorkerServices.InterfaceImplmentation.Examiner;
using System.Diagnostics;

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
            services.AddScoped<IExaminerAttemptBackgroundService, ExaminerAttemptBackgroundService>();
            services.AddSingleton<IEvaluationResultQueue, RabbitMqEvaluationResultQueue>();
            services.AddScoped<IEvaluationPublisher, AttemptEvaluationOrchestrator>();
            services.AddScoped<IEvaluationNotifier, AttemptEvaluationOrchestrator>();
            services.AddScoped<IExamAttemptEvaluator, AttemptEvaluationOrchestrator>();
            
            services.AddSingleton(new ActivitySource("ExamSystem"));
            services.AddSingleton<IEventBus, InMemoryEventBus>();
            return services;
        }
    }
       
      
}
