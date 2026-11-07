using Domain.DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using ServicesClasseslibrary.WorkerServices.ExamEvaluation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.WorkerServices.Interface.Examiner
{
    public interface IExamAttemptEvaluator { Task EvaluateExamAttemptsAsync(); }
    public interface IEvaluationPublisher { Task PublishEvaluatedExams(); }
   
}
