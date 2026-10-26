using Domain.DTOs;
using ServicesClasseslibrary.Interface.DataRepository;
using ServicesClasseslibrary.Interface.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.Implmentation.Analytics
{
    internal class AnalyticsService : IAnalyticsService
    {
        private IExaminerPerformanceOperations PerformanceOperations { get; set; }
        public AnalyticsService(IExaminerPerformanceOperations examinerPerformanceOperations)
        {
            PerformanceOperations = examinerPerformanceOperations;
        }

        public async Task RecalculateAsync(AnalyticsJobDTO job)
        {
           await PerformanceOperations.UpdateExaminerPerformance(job);
        }
    }
}
