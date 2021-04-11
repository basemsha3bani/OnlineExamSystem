using DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface
{
    interface IExamTypesDetailsOperations
    {
        void Add(ExamTypesDetailsDataModel detailsDataModel);
        void Edit(ExamTypesDetailsDataModel detailsDataModel);



        void Delete(int id);


        ExamTypesDataModel GetById(int id);

        List<ExamTypesDetailsDataModel> list();
    }
}
