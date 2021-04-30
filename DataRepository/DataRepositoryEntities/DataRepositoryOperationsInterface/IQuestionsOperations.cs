using DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface
{
   public interface IQuestionsOperations
    {
        void Add(Questions questions);
        void Edit(Questions questions);



        void Delete(int id);


        QuestionsDataModel GetById(int id);

        List<QuestionsDataModel> list();

    }
}
