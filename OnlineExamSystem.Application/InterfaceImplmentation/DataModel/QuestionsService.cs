using Domain.Entities;
using Domain.Aggregates;
using Domain.DTOs;
using OnlineExamSystem.Application.Interface.DataRepository;
using OnlineExamSystem.Application.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineExamSystem.Application
{
    public class QuestionsService : IQuestionsService
    {
        private readonly IQuestionsOperations _questionsOperations;
        private readonly IStudySubjectsOperations subjects;
      
        public  QuestionsService(IQuestionsOperations questionsOperations, IStudySubjectsOperations subjects)
        {
            _questionsOperations = questionsOperations;
            this.subjects = subjects;
           
        }
        public void Add(QuestionsDTO question)
        {
            Validate(question);
            _questionsOperations.Add(question);
        }

        private void Validate(QuestionsDTO model)
        {
            var subject = model.StudySubjectId.HasValue ? subjects.GetById(model.StudySubjectId.Value) : null;
            Domain.Entities.Questions.CheckDefinition(model, subject);
        }

        public void Delete(int id)
        {
           
        }

        public void Edit(QuestionsDTO questionAnswers)
        {
            Validate(questionAnswers);
            _questionsOperations.Edit(questionAnswers);
        }

        public QuestionsDTO GetById(int id)
        {
          return _questionsOperations.GetById(id);
        }

        public QuestionPageDTO Search(QuestionSearchDTO search) => _questionsOperations.Search(search);

        public List<QuestionsDTO> list()
        {
            var questions = _questionsOperations.list();
            return (questions);
        }
    }
}
