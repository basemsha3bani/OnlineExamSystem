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
        public QuestionsService(IQuestionsOperations questionsOperations)
        {
            _questionsOperations = questionsOperations;
        }
        
        public void Add(QuestionsDataModel questions)
        {
            _questionsOperations.Add(questions);
        }

        public void Delete(int id)
        {
           
        }

        public void Edit(QuestionsDataModel questions)
        {
            _questionsOperations.Edit(questions);
        }

        public QuestionsDataModel GetById(int id)
        {
            return null;
        }

        public List<QuestionsDataModel> list()
        {
           return _questionsOperations.list();
        }
    }
}
