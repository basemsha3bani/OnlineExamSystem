using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using ServicesClasseslibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServicesClasseslibrary
{
    public class QuestionsService : IQuestionsService
    {
        private readonly IQuestionsOperations _questionsOperations;
      
        public  QuestionsService(IQuestionsOperations questionsOperations)
        {
            _questionsOperations = questionsOperations;
           
        }
        public void Add(QuestionsDataModel questionAnswers)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
           
        }

        public void Edit(QuestionsDataModel questionAnswers)
        {
           
        }

        public QuestionsDataModel GetById(int id)
        {
          return _questionsOperations.GetById(id);
        }

        public List<QuestionsDataModel> list()
        {
            var questions = _questionsOperations.list();
            return (from qrec in questions
                    select new QuestionsDataModel
                    {
                        Id = qrec.Id,
                        QuestionText = qrec.QuestionText,
                        DifficultyLevelId = qrec.DifficultyLevelId
                    }).ToList();
        }
    }
}
