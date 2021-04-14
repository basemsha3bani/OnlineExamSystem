using DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServicesClasseslibrary
{
    public interface IExamQuestionsService
    {
        void Add(ExamQuestionsDataModel examQuestions);
        void Edit(ExamQuestionsDataModel examQuestions);



        void Delete(int id);


        ExamQuestionsDataModel GetById(int id);

        List<ExamQuestionsDataModel> list();

    }
}
