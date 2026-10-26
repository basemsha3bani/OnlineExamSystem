using Domain.Entities;
using Domain.Aggregates;
using AutoMapper;
using Domain.DTOs;
using ServicesClasseslibrary.Interface.DataRepository;
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
        public void Add(QuestionsDTO model)
        {
            ValidateAnswers(model);
            var question = mapper.Map<Questions>(model);
            question.ChangeId(0);
            question.ChangeQuestionAnswers(mapper.Map<List<QuestionAnswers>>(model.QuestionAnswersDTO));
            foreach (var answer in question.QuestionAnswers) { answer.ChangeId(0); answer.ChangeQuestionId(0); }
            gateway.Add(question);
            gateway.SaveChanges();
        }
        public void Edit(QuestionsDTO model)
        {
            ValidateAnswers(model);
            var question = gateway.GetById(q => q.Id == model.Id, q => q.QuestionAnswers)
                ?? throw new InvalidOperationException("Question not found.");
            question.CheckAnswerIdentities(model.QuestionAnswersDTO);
            mapper.Map(model, question);
            foreach (var old in question.QuestionAnswers.ToList())
                if (model.QuestionAnswersDTO.All(a => a.Id != old.Id)) question.QuestionAnswers.Remove(old);
            foreach (var incoming in model.QuestionAnswersDTO)
            {
                var answer = incoming.Id == 0 ? null : question.QuestionAnswers.Single(a => a.Id == incoming.Id);
                if (answer == null)
                {
                    answer = mapper.Map<QuestionAnswers>(incoming);
                    answer.ChangeQuestionId(question.Id);
                    question.QuestionAnswers.Add(answer);
                }
                else
                {
                    mapper.Map(incoming, answer);
                    answer.ChangeQuestionId(question.Id);
                }
            }
            gateway.SaveChanges();
        }
        private void ValidateAnswers(QuestionsDTO model)
        {
            Questions.CheckAnswers(model.QuestionAnswersDTO);
        }
        public void Delete(int id)
        {
            var question = gateway.GetById(q => q.Id == id);
            if (question == null) return;
            gateway.Delete(question); gateway.SaveChanges();
        }
        public QuestionsDTO GetById(int id) => mapper.Map<QuestionsDTO>(gateway.GetById(q => q.Id == id, q => q.QuestionAnswers));
        public QuestionPageDTO Search(QuestionSearchDTO search)
        {
            var query = QuestionSearchQuery.Filter(gateway.Query.AsNoTracking(), search);
            var total = query.Count();
            var actual = QuestionSearchDTO.Define(search.StudySubjectId, search.ChapterNumber, search.PageFor(total), search.PageSize);
            return QuestionPageDTO.Define(
                Search: actual, TotalCount: total,
                Items: QuestionSearchQuery.Page(query, actual).AsEnumerable().Select(q => QuestionsDTO.Define(
                    Id: q.Id, QuestionText: q.QuestionText, StudySubjectId: q.StudySubjectId,
                    ChapterNumber: q.ChapterNumber, DifficultyLevelId: q.DifficultyLevelId )).ToList()
            );
        }
        public List<QuestionsDTO> list() => mapper.Map<List<QuestionsDTO>>(gateway.List(null, q => q.QuestionAnswers));
        public List<QuestionsDTO> GetRandomQuestions(int studySubjectId, ExamSectionRulesDTO rule)
        {
            return mapper.Map<List<QuestionsDTO>>(gateway.Query.AsNoTracking().Include(q => q.QuestionAnswers)
                .Where(q => q.StudySubjectId == studySubjectId && q.DifficultyLevelId == rule.DifficultyLevelId)
                .OrderBy(q => Guid.NewGuid()).Take(rule.NoOfQuestions).ToList());
        }
    }
}
