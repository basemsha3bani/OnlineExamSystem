using DataModel;
using System.Threading.Tasks;

namespace DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface
{
    public interface IExaminerPerformanceOperations
    {
        ExaminerPerformance GetPerformance(int id);
        Task UpdateExaminerPerformance(AnalyticsJob job);
    }
}
