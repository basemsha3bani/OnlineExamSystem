using DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServicesClasseslibrary
{
    public interface IExamOprationsService
    {
        void Add(ExamDataModel Exam);
        void Edit(ExamDataModel model);



        void Delete(int id);


        ExamDataModel GetById(int id);

        List<ExamDataModel> list();
    }
}
