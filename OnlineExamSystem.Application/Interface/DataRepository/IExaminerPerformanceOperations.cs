using Domain.Entities;
using Domain.Aggregates;
using Domain.DTOs;
using System.Threading.Tasks;

namespace OnlineExamSystem.Application.Interface.DataRepository
{
    public interface IExaminerPerformanceOperations
    {
        ExaminerPerformance GetPerformance(int id);
        System.Collections.Generic.List<ExaminerPerformance> GetPerformances(int id);
        Task UpdateExaminerPerformance(AnalyticsJobDTO job);
    }
}
