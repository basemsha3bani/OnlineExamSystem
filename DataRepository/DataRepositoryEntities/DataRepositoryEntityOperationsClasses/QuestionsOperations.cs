using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using DataRepository.ModelMapper.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
   public class QuestionsOperations : IQuestionsOperations, IModelMapper<QuestionsDataModel>
    {
        RepositoryGateWay<Questions> repositoryGateWay;
        public void Add(Questions questions)
        {
            
        }

        public void Delete(int id)
        {
           
        }

        public void Edit(Questions questions)
        {
           
        }

        public QuestionsDataModel GetById(int id)
        {
            repositoryGateWay = new RepositoryGateWay<Questions>();
            RepositoryGateWay<QuestionAnswers> QuestionAnswersRepositoryGateWay;

            Questions questions = repositoryGateWay.GetById(e => e.Id == id);
            QuestionAnswersRepositoryGateWay = new RepositoryGateWay<QuestionAnswers>();
            questions.QuestionAnswers = QuestionAnswersRepositoryGateWay.List(e => e.QuestionId == id);
            return this.Map(questions);
        }

        public List<QuestionsDataModel> list()
        {
            repositoryGateWay = new RepositoryGateWay<Questions>();
            RepositoryGateWay<QuestionAnswers> QuestionAnswersRepositoryGateWay = new RepositoryGateWay<QuestionAnswers>();
          
            return repositoryGateWay.List(l=>l.Id==l.Id,i=>i.QuestionAnswers).Select
                (s => new QuestionsDataModel
                {
                    Id = s.Id,
                    QuestionText = s.QuestionText,
                    QuestionAnswersDataModel
                    = (from qa in s.QuestionAnswers
            select new QuestionAnswersDataModel
                       {
                           QuestionId = qa.QuestionId,
                           AnswerText = qa.AnswerText,
                           IsCorrext = qa.IsCorrect,
                           Id = qa.Id

                       }).ToList()

                }).ToList();
           
        }

        public QuestionsDataModel Map(IRepository repository)
        {
            Questions questions = (Questions)repository;
            QuestionsDataModel questionsDataModel =
                new QuestionsDataModel
                {
                    Id = questions.Id,
                    QuestionText = questions.QuestionText,
                    QuestionAnswersDataModel
                    = (from qa in questions.QuestionAnswers
                       select new QuestionAnswersDataModel
                       {
                           QuestionId = qa.QuestionId,
                           AnswerText = qa.AnswerText,
                           IsCorrext = qa.IsCorrect,
                           Id = qa.Id

                       }).ToList()

                };
        return questionsDataModel;


    }
    }
}
