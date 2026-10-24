using DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServicesClasseslibrary.Interface
{
   public interface IQuestionsService
    {
        void Add(QuestionsDataModel questionAnswers);
        void Edit(QuestionsDataModel questionAnswers);



        void Delete(int id);


        QuestionsDataModel GetById(int id);

        QuestionPage Search(QuestionSearch search);
        List<QuestionsDataModel> list();
    }
}
