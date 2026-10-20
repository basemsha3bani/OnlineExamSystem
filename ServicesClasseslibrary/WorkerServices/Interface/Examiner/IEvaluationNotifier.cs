using System.Threading;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.WorkerServices.Interface.Examiner
{
    public interface IEvaluationNotifier { Task SendOneBatchAsync(CancellationToken ct); }
}
