using AutoMapper;
using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class QuestionsOperations : IQuestionsOperations
    {
        private readonly ContextGateway<Questions> gateway;
        private readonly IMapper mapper;
        public QuestionsOperations(ContextGateway<Questions> gateway, IMapper mapper)
        { this.gateway = gateway; this.mapper = mapper; }
        public void Add(QuestionsDataModel model)
        {
            ValidateAnswers(model);
            var question = mapper.Map<Questions>(model);
            question.Id = 0;
            question.QuestionAnswers = mapper.Map<List<QuestionAnswers>>(model.QuestionAnswersDataModel);
            foreach (var answer in question.QuestionAnswers) { answer.Id = 0; answer.QuestionId = 0; }
            gateway.Add(question);
            gateway.SaveChanges();
        }
        public void Edit(QuestionsDataModel model)
        {
            ValidateAnswers(model);
            var question = gateway.GetById(q => q.Id == model.Id, q => q.QuestionAnswers)
                ?? throw new InvalidOperationException("Question not found.");
            if (model.QuestionAnswersDataModel.Where(a => a.Id != 0).GroupBy(a => a.Id).Any(g => g.Count() > 1)
                || model.QuestionAnswersDataModel.Any(a => a.Id != 0 && question.QuestionAnswers.All(e => e.Id != a.Id)))
                throw new InvalidOperationException("Invalid answer identity.");
            mapper.Map(model, question);
            foreach (var old in question.QuestionAnswers.ToList())
                if (model.QuestionAnswersDataModel.All(a => a.Id != old.Id)) question.QuestionAnswers.Remove(old);
            foreach (var incoming in model.QuestionAnswersDataModel)
            {
                var answer = incoming.Id == 0 ? null : question.QuestionAnswers.Single(a => a.Id == incoming.Id);
                if (answer == null)
                {
                    answer = mapper.Map<QuestionAnswers>(incoming);
                    answer.QuestionId = question.Id;
                    question.QuestionAnswers.Add(answer);
                }
                else
                {
                    mapper.Map(incoming, answer);
                    answer.QuestionId = question.Id;
                }
            }
            gateway.SaveChanges();
        }
        private void ValidateAnswers(QuestionsDataModel model)
        {
            if (model.QuestionAnswersDataModel == null || model.QuestionAnswersDataModel.Count(a => a.IsCorrect) != 1)
                throw new InvalidOperationException("Each question must have exactly one correct answer.");
        }
        public void Delete(int id)
        {
            var question = gateway.GetById(q => q.Id == id);
            if (question == null) return;
            gateway.Delete(question); gateway.SaveChanges();
        }
        public QuestionsDataModel GetById(int id) => mapper.Map<QuestionsDataModel>(gateway.GetById(q => q.Id == id, q => q.QuestionAnswers));
        public List<QuestionsDataModel> list() => mapper.Map<List<QuestionsDataModel>>(gateway.List(null, q => q.QuestionAnswers));
        public List<QuestionsDataModel> GetRandomQuestions(int studySubjectId, ExamSectionRulesDataModel rule)
        {
            return mapper.Map<List<QuestionsDataModel>>(gateway.Query.AsNoTracking().Include(q => q.QuestionAnswers)
                .Where(q => q.StudySubjectId == studySubjectId && q.DifficultyLevelId == rule.DifficultyLevelId)
                .OrderBy(q => Guid.NewGuid()).Take(rule.NoOfQuestions).ToList());
        }
    }
}
