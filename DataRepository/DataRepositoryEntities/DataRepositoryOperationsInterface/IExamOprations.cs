using DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface
{
    interface IExamOprations
    {

        void Add(ExamDataModel Exam);
        void Edit(ExamDataModel model);



        void Delete(int id);


        ExamDataModel GetById(int id);

        List<ExamDataModel> list();
    }
}
