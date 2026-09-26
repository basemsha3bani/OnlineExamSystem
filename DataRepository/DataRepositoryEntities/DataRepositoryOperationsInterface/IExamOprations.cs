using DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface
{
   public interface IExamOprations
    {

        void Add(ExamDataModel Exam);
        void Edit(ExamDataModel model);



        


        ExamDataModel GetById(int id);

        List<ExamDataModel> list();
    }
}
