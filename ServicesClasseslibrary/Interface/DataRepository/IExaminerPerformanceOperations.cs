using Domain.Entities;
using Domain.Aggregates;
using Domain.DTOs;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.Interface.DataRepository
{
    public interface IExaminerPerformanceOperations
    {
        ExaminerPerformance GetPerformance(int id);
        Task UpdateExaminerPerformance(AnalyticsJobDTO job);
    }
}
