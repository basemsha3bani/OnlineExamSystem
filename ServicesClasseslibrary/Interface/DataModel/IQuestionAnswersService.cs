using DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServicesClasseslibrary.Interface
{
    public interface IQuestionAnswersService
    {
        void Add(QuestionAnswersDataModel questionAnswers);
        void Edit(QuestionAnswersDataModel questionAnswers);



        void Delete(int id);


        QuestionAnswersDataModel GetById(int id);

        List<QuestionAnswersDataModel> list();
    }
}
