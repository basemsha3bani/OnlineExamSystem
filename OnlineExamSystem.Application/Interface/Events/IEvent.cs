using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExamSystem.Application.Interface.Events
{
    // Base for all events - gives you CorrelationId for Telemetry
    public interface IEvent
    {
        Guid EventId { get; }
        DateTime OccurredAt { get; }
        Guid CorrelationId { get; }
    }

    public record ExamSubmittedEvent(
        Guid EventId,
        int AttemptId,
        DateTime OccurredAt,
        Guid CorrelationId
    ) : IEvent;

    public record ExamSavedEvent(
        Guid EventId,
        int AttemptId,
        int SectionIndex,
        DateTime OccurredAt,
        Guid CorrelationId
    ) : IEvent;

    public record ExamGradedEvent(
        Guid EventId,
        int AttemptId,
        int ExaminerId,
        DateTime OccurredAt,
        Guid CorrelationId,
        Decimal score
    ) : IEvent;
}
