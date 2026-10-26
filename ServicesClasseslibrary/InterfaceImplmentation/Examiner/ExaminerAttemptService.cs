using Domain.Entities;
using Domain.Aggregates;
using Domain.DTOs;
using ServicesClasseslibrary.Interface.DataRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServicesClasseslibrary.Implmentation.Builder;
using ServicesClasseslibrary.Interface.Builder;
using ServicesClasseslibrary.Interface.Examiner;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.InterfaceImplmentation.Examiner
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

        public async Task<List<Tuple<int, int,decimal>>> EvaluatePending(Action<int, Exception> onFailure = null)
        {
            Tuple<int, int, decimal > evaluatedTuple = new Tuple<int,int,decimal>(0,0,0);
            var pending = examAttemptOprations.listSubmitted().Select(s=>s.Id);
            ///define evaluated as a list of tuples to store the evaluated attempts
            ///each tuple will contain the attempt id and the userId
            
            List< Tuple<int, int,decimal> > evaluatedTuples = new List<Tuple<int, int, decimal>>();

            foreach (int id in pending)
            {
                try
                {
                  
                    var pendingAttempt = examAttemptOprations.CheckIfAttemptSubmitted(id);
                    if (pendingAttempt == null) continue;
                    var attempt = ExamAttempt.Restore(pendingAttempt);
                    attempt.Evaluate();
                   await examAttemptOprations.Edit(attempt.ToDTO());
                    evaluatedTuple = new Tuple<int,int,decimal>(id,attempt.UserId,attempt.Score.Value);
                    evaluatedTuples.Add(evaluatedTuple);
                }
                catch (DbUpdateConcurrencyException) { /* Another worker completed this attempt. */ }
                catch (Exception ex)
                {
                    if (onFailure == null) throw;
                    onFailure(id, ex); // Leave pending for retry without blocking other attempts.
                }
               
            }
            return evaluatedTuples;
        }
    }

  
}
   
