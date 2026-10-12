using System.Threading;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.Interface.Examiner
{
    public interface IEvaluationNotifier { Task SendOneBatchAsync(CancellationToken ct); }
}
