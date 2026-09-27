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
   public  class QuestionsOperations : IQuestionsOperations, IModelMapper<QuestionsDataModel,Questions>
    {
        public QuestionsOperations()
        {
            ContextGateway<DifficultyLevels>.GetContextInstance();
        }

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
           ;
            List<QuestionAnswers> answers=questionsDataModel.QuestionAnswersDataModel.Select(s=> new QuestionAnswers
            {
               
                AnswerText = s.AnswerText,
                IsCorrect = s.IsCorrect,
                Question=Question
            }).ToList();
            ContextGateway<Questions>.Add(Question);
            ContextGateway<Questions>.Add(answers);
            ContextGateway<Questions>.Commit();



        }

        public void Delete(int id)
        {
           
        }

        public void Edit(QuestionsDataModel questionsDataModel)
        {
            Questions Question= ContextGateway<Questions>.GetById(g => g.Id == questionsDataModel.Id,g=>g.QuestionAnswers);



            Question.QuestionText = questionsDataModel.QuestionText;
            Question.DifficultyLevelId = questionsDataModel.DifficultyLevelId;
            Question.StudySubjectId = questionsDataModel.StudySubjectId;
            questionsDataModel.QuestionAnswersDataModel.ForEach (qa =>
                {
                    QuestionAnswers answer = Question.QuestionAnswers.First(w => w.Id == qa.Id);

                    answer.QuestionId = qa.QuestionId;
                    answer.AnswerText = qa.AnswerText;
                    answer.IsCorrect = qa.IsCorrect;
                    
                                   }
                ) ;

            ;
            ContextGateway<Questions>.CreateDatabaseTransaction();
            try
            {

                ContextGateway<Questions>.Edit(Question);
                ContextGateway<Questions>.Edit(Question.QuestionAnswers);

                ContextGateway<Questions>.Commit();
            }
            catch(Exception ex)
            {
                ContextGateway<Questions>.Rollback();
                throw;
            }
            
           
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

      


    

        public QuestionsDataModel Map(Questions questions)
        {
            QuestionsDataModel questionsDataModel =
               new QuestionsDataModel
               {
                   Id = questions.Id,
                   DifficultyLevelId=questions.DifficultyLevelId,
                   StudySubjectId=questions.StudySubjectId,
                   QuestionText = questions.QuestionText,
                   QuestionAnswersDataModel
                   = (from qa in questions.QuestionAnswers
                      select qa.Map(qa)

                      ).ToList()

               };
            return questionsDataModel;
        }
    }
}
