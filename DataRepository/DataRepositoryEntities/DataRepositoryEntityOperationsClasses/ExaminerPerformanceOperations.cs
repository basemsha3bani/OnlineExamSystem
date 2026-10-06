using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class ExaminerPerformanceOperations: IExaminerPerformanceOperations
    {
        private readonly ContextGateway<ExaminerPerformance> gateway;
        public ExaminerPerformanceOperations(ContextGateway<ExaminerPerformance> gateway    )
        {
            this.gateway = gateway;
        }

        public ExaminerPerformance GetPerformance(int id)
        {
        return  this.gateway.GetById(x=>x.UserId==id);

        }

        public async Task UpdateExaminerPerformance(AnalyticsJob job)
        {
            using (var context = new DbConext())
            {
                bool AddNewPerformance = false;
                var attempts = await context.ExaminerAttempts.Where(w=>w.UserId==job.ExaminerId).ToListAsync();   
                var evaluatedAttempts = attempts.Where(x => x.Status == "Submitted" && x.Score != null).ToList();   
                var performance = context.Set<ExaminerPerformance>().Find(job.ExaminerId);
                if (performance == null)
                {
                    performance = new ExaminerPerformance();
                    AddNewPerformance = true;
                }
                
                    decimal totalScore = attempts.Where(x => x.Score != null).Sum(x => x.Score.Value)*100;
                    performance.TotalAttempts = attempts.Count;
                    performance.EvaluatedCount = attempts.Count(x => x.Status == "Scored");
                    performance.InProgressCount = attempts.Count(x => x.Status == "InProgress");
                    performance.AverageScore = performance.EvaluatedCount > 0 ?((double) totalScore / performance.EvaluatedCount):0;
                    performance.UserId=job.ExaminerId;
                    performance.HighestScore = evaluatedAttempts.Count > 0 ? (double)evaluatedAttempts.Max(x => x.Score.Value) : 0;
                    performance.LowestScore = evaluatedAttempts.Count > 0 ? (double)evaluatedAttempts.Min(x => x.Score.Value) : 0;
                     performance.PassRate = evaluatedAttempts.Count > 0 ? (double)evaluatedAttempts.Count(x => x.Score*100>=50) / performance.EvaluatedCount * 100 : 0;
                if (AddNewPerformance)
                {
                await   context.ExaminerPerformance.AddAsync(performance);    
                }
                else
                {
                 context.ExaminerPerformance.Update(performance);
                }
                await context.SaveChangesAsync();
            }
            
            }

        }
}
