using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OnlineExamSystem.Application.Interface.Events;
using OnlineExamSystem.Application.Interface.Examiner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace ServicesClasseslibrary.WorkerServices.ExamEvaluation.JobCompletion.EventPublishing
{
    internal class ExamEvaluationCompetionEventPublisher
    {
        private readonly IServiceScopeFactory scopes;
        public async Task PublishEvaluatedExams()
        {
            ///Use ExaminerAttemptService To Read from OutBox Table 
            using var scope = scopes.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IExaminerAttemptBackgroundService>();
            var evaluated = await service.GetEvaluatedExamAttempts();
            foreach (var attempt in evaluated.DistinctBy(a => a.Id))
            {
                ExamGradedEvent examGradedEvent = (ExamGradedEvent)JsonConvert.DeserializeObject(attempt.Payload);

                ///Publish Event to Event Bus
                IEventBus eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
                await eventBus.PublishAsync(examGradedEvent);


            }


        }
    }
}
