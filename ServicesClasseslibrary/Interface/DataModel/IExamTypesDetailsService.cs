using DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServicesClasseslibrary
{
   public interface IExamTypesDetailsService
    {
        void Add(ExamTypesDetailsDataModel examTypesDetails);
        void Edit(ExamTypesDetailsDataModel examTypesDetails);



        void Delete(int id);


        ExamTypesDetailsDataModel GetById(int id);

        List<ExamTypesDetailsDataModel> list();
    }
}
