using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using DataRepository.ModelMapper.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
   public partial class QuestionsOperations : IQuestionsOperations, IModelMapper<QuestionsDataModel,Questions>
    {
        
        public void Add(QuestionsDataModel questionsDataModel)
        {
            Questions Question= new Questions
            {
                Id = questionsDataModel.Id,
                QuestionText = questionsDataModel.QuestionText,
                DifficultyLevelId = questionsDataModel.DifficultyLevelId,
                StudySubjectId=questionsDataModel.StudySubjectId,


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

        public void Edit(QuestionsDataModel questionsDataModel)
        {
            Questions Question = new Questions
            {
                Id = questionsDataModel.Id,
                QuestionText = questionsDataModel.QuestionText,
                DifficultyLevelId = questionsDataModel.DifficultyLevelId,
                StudySubjectId = questionsDataModel.StudySubjectId,
                QuestionAnswers = (from QuestionAnswersDataModel questionAnswers in questionsDataModel.QuestionAnswersDataModel
                                   select new QuestionAnswers
                                   {
                                       Id = questionAnswers.Id,
                                       AnswerText = questionAnswers.AnswerText,
                                       IsCorrect = questionAnswers.IsCorrext,
                                       QuestionId = questionsDataModel.Id
                                   }).ToList()

            };
            ContextGateway<Questions>.CreateDatabaseTransaction();
            ContextGateway<Questions>.Edit(Question);

            //foreach (QuestionAnswersDataModel questionAnswers in questionsDataModel.QuestionAnswersDataModel)
            //{

            //    ContextGateway<Questions>.Edit(
            //        new QuestionAnswers
            //        {
            //            Id = questionAnswers.Id,
            //            AnswerText = questionAnswers.AnswerText,
            //            IsCorrect = questionAnswers.IsCorrext,
            //            QuestionId = Question.Id
            //        });
            //}
            ContextGateway<Questions>.Commit();
        }

        public QuestionsDataModel GetById(int id)
        {
            //repositoryGateWay = new RepositoryGateWay<Questions>();
            //RepositoryGateWay<QuestionAnswers> QuestionAnswersRepositoryGateWay;
            ContextGateway<Questions>.GetContextInstance();
            Questions questions = ContextGateway<Questions>.GetById(e => e.Id == id,qa=>qa.QuestionAnswers);
            //QuestionAnswersRepositoryGateWay = new RepositoryGateWay<QuestionAnswers>();
            //ContextGateway<QuestionAnswers>.GetContextInstance();
            //questions.QuestionAnswers = ContextGateway<QuestionAnswers>.List(e => e.QuestionId == id);
            return this.Map(questions);
        }

        public List<QuestionsDataModel> list()
        {
            //repositoryGateWay = new RepositoryGateWay<Questions>();
            //RepositoryGateWay<QuestionAnswers> QuestionAnswersRepositoryGateWay = new RepositoryGateWay<QuestionAnswers>();
            ContextGateway<Questions>.GetContextInstance();
            return ContextGateway<Questions>.List(l => l.Id == l.Id, i => i.QuestionAnswers).Select
                (s => this.Map(s)).ToList();

               
           
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
                           Id = qa.Id,
                           radioButtonDisplay=string.Join("",qa.Id,"rbIsCorrect")

                       }).ToList()

                };
        return questionsDataModel;


    }

        public QuestionsDataModel Map(Questions questions)
        {
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
                          Id = qa.Id,
                          radioButtonDisplay = string.Join("", qa.Id, "rbIsCorrect")

                      }).ToList()

               };
            return questionsDataModel;
        }
    }
}
