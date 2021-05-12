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
        ContextGateway<Questions> repositoryGateWay;
        public void Add(QuestionsDataModel questionsDataModel)
        {
            Questions Question= new Questions
            {
                Id = questionsDataModel.Id,
                QuestionText = questionsDataModel.QuestionText,
                DifficultyLevelId = questionsDataModel.DifficultyLevelId,


            };
            
            ContextGateway<Questions>.CreateDatabaseTransaction();
            ContextGateway<Questions>.Add(Question);
            
            foreach(QuestionAnswersDataModel questionAnswers in questionsDataModel.QuestionAnswersDataModel)
            {

                ContextGateway<Questions>.Add(
                    new QuestionAnswers
                    {
                        Id = questionAnswers.Id,
                        AnswerText = questionAnswers.AnswerText,
                        IsCorrect = questionAnswers.IsCorrext,
                        QuestionId = Question.Id
                    });
            }
            ContextGateway<Questions>.Commit();



        }

        public void Delete(int id)
        {
           
        }

        public void Edit(QuestionsDataModel questions)
        {
           
        }

        public QuestionsDataModel GetById(int id)
        {
            //repositoryGateWay = new RepositoryGateWay<Questions>();
            //RepositoryGateWay<QuestionAnswers> QuestionAnswersRepositoryGateWay;

           Questions questions = ContextGateway<Questions>.GetById(e => e.Id == id);
            //QuestionAnswersRepositoryGateWay = new RepositoryGateWay<QuestionAnswers>();
            questions.QuestionAnswers = ContextGateway<QuestionAnswers>.List(e => e.QuestionId == id);
            return this.Map(questions);
        }

        public List<QuestionsDataModel> list()
        {
            //repositoryGateWay = new RepositoryGateWay<Questions>();
            //RepositoryGateWay<QuestionAnswers> QuestionAnswersRepositoryGateWay = new RepositoryGateWay<QuestionAnswers>();
            ContextGateway<Questions>.GetContextInstance();
            return ContextGateway <Questions>.List(l=>l.Id==l.Id,i=>i.QuestionAnswers).Select
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
