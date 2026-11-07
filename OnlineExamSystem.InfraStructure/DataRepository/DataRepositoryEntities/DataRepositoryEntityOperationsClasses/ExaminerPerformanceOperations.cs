using Domain.Entities;
using Domain.Aggregates;
using Domain.DTOs;
using DataRepository.GateWay;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Application.Interface.DataRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class ExaminerPerformanceOperations : IExaminerPerformanceOperations
    {
        private readonly ContextGateway<ExaminerPerformance> gateway;
        private readonly ContextGateway<ExamAttempt> attempts;
        private readonly ContextGateway<Exam> exams;
        public ExaminerPerformanceOperations(ContextGateway<ExaminerPerformance> gateway,
            ContextGateway<ExamAttempt> attempts, ContextGateway<Exam> exams)
        { this.gateway = gateway; this.attempts = attempts; this.exams = exams; }

        public List<ExaminerPerformance> GetPerformances(int id) =>
            gateway.List(p => p.UserId == id, p => p.Subject).OrderBy(p => p.Subject.SubjectName).ToList();

        public ExaminerPerformance GetPerformance(int id)
        {
            var summary = ExaminerPerformance.Define();
            summary.Recalculate(id, 0, attempts.List(a => a.UserId == id));
            return summary;
        }

        public async Task UpdateExaminerPerformance(AnalyticsJobDTO job)
        {
            // Include every attempt, including repeats of the same exam.
            var rows = await (from attempt in attempts.Query.AsNoTracking()
                              join exam in exams.Query.AsNoTracking() on attempt.ExamId equals exam.Id
                              where attempt.UserId == job.ExaminerId
                              select new { Attempt = attempt, SubjectId = exam.StudySubjectId }).ToListAsync();
            var existing = await gateway.Query.Where(p => p.UserId == job.ExaminerId).ToListAsync();
            var groups = rows.GroupBy(r => r.SubjectId).ToList();
            foreach (var stale in existing.Where(p => !groups.Any(g => g.Key == p.SubjectId)))
                gateway.Delete(stale);
            foreach (var group in groups)
            {
                var performance = existing.SingleOrDefault(p => p.SubjectId == group.Key);
                if (performance == null)
                {
                    performance = ExaminerPerformance.Define(UserId: job.ExaminerId);
                    performance.ChangeSubjectId(group.Key);
                    gateway.Add(performance);
                }
                performance.Recalculate(job.ExaminerId, job.TriggerAttemptId, group.Select(r => r.Attempt));
            }
            await gateway.SaveChangesAsync();
        }
    }
}
