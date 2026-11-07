using Domain.Aggregates;
using Domain.DTOs;
using OnlineExamSystem.Application.Interface.Events;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineExamSystem.Application.Interface.DataRepository
{
    public interface IExamAttemptOprations
    {
        int Add(ExamAttemptDTO model);
        ExamAttemptDTO CheckIfAttemptSubmitted(int id);
        Task Edit(ExamAttemptDTO model);
        Task Evaluate(ExamAttemptDTO model, Guid iterationId);
        ExamAttemptDTO GetById(int id);
        List<ExamAttemptDTO> listByUser(int userId);
       Task <List<ExamAttemptDTO>> listSubmitted();
        Task<List<OutboxMessage>> listEvaluated();
        Task MarkOutboxMessageProcessedAsync(int attemptId, System.Threading.CancellationToken ct);
    }
}
