using DataModel;
using DataRepository.DataRepositoryEntities;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
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

namespace ServicesClasseslibrary.Implmentation.Examiner
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
            if (sections.Count == 0) throw new InvalidOperationException("The selected exam does not exist.");
           
            var snapshot = new AttemptSnapshot();
            var used = new HashSet<int>();
            foreach (var section in sections)
            {
                var savedSection = new AttemptSection { Name = section.SectionName };
                foreach (var rule in section.ExamSectionRules.OrderBy(r => r.Id))
                {
                    if (rule.NoOfQuestions <= 0) throw new InvalidOperationException("The exam has an invalid question count.");
                    var questions = builder.BuildExamAttemptQuestions(section.exam.StudySubjectId,
                        new ExamSectionRulesDataModel { Id = rule.Id, SectionId = section.Id,
                            DifficultyLevelId = rule.DifficultyLevelId, NoOfQuestions = rule.NoOfQuestions }, used);
                    // Exclude questions already used in another section or rule.
                    var selected = questions.Where(q => !used.Contains(q.Id)).Take(rule.NoOfQuestions).ToList();
                    if (selected.Count != rule.NoOfQuestions)
                        throw new InvalidOperationException("There are not enough unique questions for this exam configuration.");
                    foreach (var question in selected)
                    {
                        if (question.QuestionAnswersDataModel == null || question.QuestionAnswersDataModel.Count(a => a.IsCorrect) != 1)
                            throw new InvalidOperationException("Each question must have exactly one correct answer.");
                        used.Add(question.Id);
                        savedSection.Questions.Add(new AttemptQuestion { Id = question.Id, Text = question.QuestionText,
                            Options = question.QuestionAnswersDataModel.Select(a => new AttemptOption {
                                Id = a.Id, Text = a.AnswerText, IsCorrect = a.IsCorrect }).ToList() });
                    }
                }
                if (savedSection.Questions.Count == 0) throw new InvalidOperationException("Each exam section must contain configured questions.");
                snapshot.Sections.Add(savedSection);
            }
            if (!snapshot.Questions.Any()) throw new InvalidOperationException("This exam has no configured questions.");
            var attempt = new ExamAttemptDataModel { ExamId = examId, UserId = userId, ExamTitle = sections.First().exam.Title,
                StartedAt = DateTime.UtcNow, TotalQuestions = snapshot.Questions.Count(), SnapshotJson = JsonSerializer.Serialize(snapshot) };
           int attemptId= examAttemptOprations.Add(attempt);
           
            return attemptId;
        }

        public ExamAttemptDataModel Get(int id)
        {

            return examAttemptOprations.GetById(id);
        }
        public List<ExamAttemptDataModel> List(int userId)
        {

            return examAttemptOprations.listByUser(userId);
        }
        public static AttemptSnapshot Read(ExamAttemptDataModel attempt) => JsonSerializer.Deserialize<AttemptSnapshot>(attempt.SnapshotJson);

        public void Save(int id,  int sectionIndex, Dictionary<int, int?> answers, bool submit)
        {
           
            var attempt = examAttemptOprations.GetById(id);
            if (attempt == null) throw new InvalidOperationException("Attempt not found.");
            if (attempt.Status != "InProgress") return; // Repeated submission cannot change answers.
            var snapshot = Read(attempt);
            if (sectionIndex < 0 || sectionIndex >= snapshot.Sections.Count) throw new InvalidOperationException("Invalid section.");
            var questions = snapshot.Sections[sectionIndex].Questions;
            if (answers.Keys.Any(idValue => questions.All(q => q.Id != idValue))) throw new InvalidOperationException("Invalid question.");
            foreach (var question in questions)
            {
                answers.TryGetValue(question.Id, out var selected);
                if (selected.HasValue && question.Options.All(o => o.Id != selected.Value)) throw new InvalidOperationException("Invalid answer.");
                question.SelectedOptionId = selected;
            }
            attempt.SnapshotJson = JsonSerializer.Serialize(snapshot);
            if (submit) { attempt.Status = "Submitted"; attempt.SubmittedAt = DateTime.UtcNow; }
            // Submitted rows are the durable queue; answers and enqueue are one atomic write.
            examAttemptOprations.Edit(attempt);
        }

        public void EvaluatePending(Action<int, Exception> onFailure = null)
        {

            var pending = examAttemptOprations.listSubmitted().Select(s=>s.Id);
             
            foreach (int id in pending)
            {
                try
                {
                  
                    var attempt = examAttemptOprations.CheckIfAttemptSubmitted(id);
                    if (attempt == null) continue;
                    var snapshot = Read(attempt);
                    attempt.CorrectQuestions = AttemptScoring.CountCorrect(snapshot);
                    attempt.Score = AttemptScoring.Ratio(attempt.CorrectQuestions.Value, attempt.TotalQuestions);
                    attempt.EvaluatedAt = DateTime.UtcNow;
                    attempt.Status = "Scored";
                    examAttemptOprations.Edit(attempt);
                }
                catch (DbUpdateConcurrencyException) { /* Another worker completed this attempt. */ }
                catch (Exception ex)
                {
                    if (onFailure == null) throw;
                    onFailure(id, ex); // Leave pending for retry without blocking other attempts.
                }
            }
        }
    }

  
}
   
