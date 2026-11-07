using Domain.Entities;
using Domain.Aggregates;
using Domain.DTOs;
using OnlineExamSystem.Application.Interface.DataRepository;

using Microsoft.Extensions.Logging;
using OnlineExamSystem.Application.Implmentation.Builder;
using OnlineExamSystem.Application.Interface.Builder;
using OnlineExamSystem.Application.Interface.Examiner;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineExamSystem.Application.InterfaceImplmentation.Examiner
{
    public class ExaminerAttemptService: IExaminerAttemptService
    {
        private readonly IExamAttemptQuestionBuilder builder;
        
        private readonly IExamAttemptOprations examAttemptOprations;
        private readonly IExamSectionsOperations  examSectionsOperations;
        public ExaminerAttemptService(IExamAttemptQuestionBuilder builder, IExamOprations examOprations,IExamAttemptOprations examAttemptOprations,IExamSectionsOperations examSectionsOperations) { this.builder = builder;  this.examAttemptOprations = examAttemptOprations;this.examSectionsOperations = examSectionsOperations; }

        public int Start(int examId, int userId)
        {
            var sections = examSectionsOperations.List(examId);
           
            var selectedBySection = builder.BuildExamAttemptQuestions(examId, sections);
            var attempt = ExamAttempt.Start(examId, userId, sections, selectedBySection);
           int attemptId = examAttemptOprations.Add(attempt.ToDTO());
           
            return attemptId;
        }

        public ExamAttemptDTO Get(int id)
        {

            return examAttemptOprations.GetById(id);
        }
        public List<ExamAttemptDTO> List(int userId)
        {

            return examAttemptOprations.listByUser(userId);
        }
        public static AttemptSnapshotDTO Read(ExamAttemptDTO attempt) => ExamAttempt.Read(attempt);

        public async Task Save(int id,  int sectionIndex, Dictionary<int, int?> answers, bool submit)
        {
           
            var attempt = ExamAttempt.Restore(examAttemptOprations.GetById(id));
            if (!attempt.SaveAnswers(sectionIndex, answers, submit)) return;
            // Submitted rows are the durable queue; answers and enqueue are one atomic write.
           await examAttemptOprations.Edit(attempt.ToDTO());
        }

        
    }

  
}
   
