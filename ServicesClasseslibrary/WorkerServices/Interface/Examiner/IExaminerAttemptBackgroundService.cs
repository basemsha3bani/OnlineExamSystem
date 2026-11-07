using Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineExamSystem.Application.Interface.Examiner
{
    public interface IExaminerAttemptBackgroundService
    {
        Task<List<Tuple<int, int, decimal>>> EvaluatePending(Action<int, Exception> onFailure = null);
        Task <List<OutboxMessage>> GetEvaluatedExamAttempts();
    }
}
