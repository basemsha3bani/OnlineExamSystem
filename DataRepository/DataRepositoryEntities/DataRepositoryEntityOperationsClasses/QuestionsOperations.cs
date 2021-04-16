using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    class QuestionsOperations : IQuestionsOperations
    {
        RepositoryGateWay<Questions> QuestionsRepositoryGateWay = new RepositoryGateWay<Questions>();
        public void Add(QuestionsDataModel questionsDM)
        {
            Questions questions = new Questions();
            questions.QuestionText = questionsDM.QuestionText;
            questions.DifficultyLevelId = questionsDM.DifficultyLevelId;
            questions.QuestionAnswers = (from qa in questionsDM.QuestionAnswersDataModel
                                         select new QuestionAnswers
                                         {
                                             AnswerText=qa.AnswerText,
                                             IsCorrect=qa.IsCorrext,
                                             

                                         }).ToList();


            QuestionsRepositoryGateWay.Add(questions);

        }

        public void Delete(int id)
        {
           
        }

        public void Edit(QuestionsDataModel questions)
        {
           
        }

        public QuestionsDataModel GetById(int id)
        {
            return null;
           
        }

        public List<QuestionsDataModel> list()
        {
            List<Questions> questions = QuestionsRepositoryGateWay.List();
            return (from q in questions
                    select new QuestionsDataModel
                    {
                        QuestionText = q.QuestionText,
                        DifficultyLevelId = q.DifficultyLevelId,
                        Id = q.Id,

                        QuestionAnswersDataModel = (from qa in q.QuestionAnswers
                                                    select new DataModel.QuestionAnswersDataModel
                                                    {
                                                        Id = qa.Id,
                                                        QuestionId = q.Id,
                                                        AnswerText = qa.AnswerText,
                                                        IsCorrext = qa.IsCorrect
                                                    }).ToList()
                    }).ToList();

        }
    }
}
