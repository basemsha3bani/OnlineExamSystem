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
        private readonly ContextGateway<ExaminerAttempt> attemptGateWay;
        public ExaminerPerformanceOperations(ContextGateway<ExaminerPerformance> gateway,
          ContextGateway<ExaminerAttempt> attemptGateWay)
        {
            this.gateway = gateway;
            this.attemptGateWay = attemptGateWay;
        }

        public ExaminerPerformance GetPerformance(int id)
        {
        return  this.gateway.GetById(x=>x.UserId==id);

        }

        public async Task UpdateExaminerPerformance(AnalyticsJob job)
        {
          
                bool AddNewPerformance = false;
                var attempts = await attemptGateWay.ListAsync(w=>w.UserId==job.ExaminerId);   
                var evaluatedAttempts = attempts.Where(x => x.Status == "Submitted" && x.Score != null).ToList();   
                var performance = gateway.GetById(g=>g.UserId==job.ExaminerId);
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
                   gateway.Add(performance);
                
                }
                
               
                
                await gateway.SaveChangesAsync();
            }
            
            }

        
}
