using Domain.Entities;
using System.Collections.Generic;
namespace OnlineExamSystem.Models
{
    public class PerformanceAnalyticsViewModel
    {
        public int? SubjectId { get; set; }
        public List<ExaminerPerformance> Subjects { get; set; } = new();
        public List<ExaminerPerformance> Summaries { get; set; } = new();
    }
}
