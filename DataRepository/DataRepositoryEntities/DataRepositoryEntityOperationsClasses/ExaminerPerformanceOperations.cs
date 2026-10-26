using Domain.Entities;
using Domain.Aggregates;
using Domain.DTOs;
using ServicesClasseslibrary.Interface.DataRepository;
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
        private readonly ContextGateway<ExamAttempt> attemptGateWay;
        public ExaminerPerformanceOperations(ContextGateway<ExaminerPerformance> gateway,
          ContextGateway<ExamAttempt> attemptGateWay)
        {
            this.gateway = gateway;
            this.attemptGateWay = attemptGateWay;
        }

        public ExaminerPerformance GetPerformance(int id)
        {
        return  this.gateway.GetById(x=>x.UserId==id);

        }

        public async Task UpdateExaminerPerformance(AnalyticsJobDTO job)
        {
          
                bool AddNewPerformance = false;
                var attempts = await attemptGateWay.ListAsync(w=>w.UserId==job.ExaminerId);   
                var performance = gateway.GetById(g=>g.UserId==job.ExaminerId);
                if (performance == null)
                {
                    performance = ExaminerPerformance.Define();
                    AddNewPerformance = true;
                }
                
                performance.Recalculate(job.ExaminerId, job.TriggerAttemptId, attempts);
                if (AddNewPerformance)
                {
                   gateway.Add(performance);
                
                }
                
               
                
                await gateway.SaveChangesAsync();
            }
            
            }

        
}
