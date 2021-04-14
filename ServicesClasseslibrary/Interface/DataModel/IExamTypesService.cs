using DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServicesClasseslibrary
{
   public interface IExamTypesService 
    {
        void Add(ExamTypesDataModel examTypes);
        void Edit(ExamTypesDataModel examTypes);



        void Delete(int id);


        ExamTypesDataModel GetById(int id);

        List<ExamTypesDataModel> list();
    }
}
