using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using ServicesClasseslibrary.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServicesClasseslibrary
{
    public class QuestionsService : IQuestionsService
    {
        private readonly IQuestionsOperations _questionsOperations;
        public  QuestionsService(IQuestionsOperations questionsOperations)
        {
            throw new NotImplementedException();
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
            return null;
        }

        public List<QuestionsDataModel> list()
        {
            return null;
        }
    }
}
