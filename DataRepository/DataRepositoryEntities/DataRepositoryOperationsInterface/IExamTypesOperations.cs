using DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface
{
    interface IExamTypesOperations
    {
        void Add(ExamTypesDataModel examTypes);
        void Edit(ExamTypesDataModel examTypes);



        void Delete(int id);


        ExamTypesDataModel GetById(int id);

        List<ExamTypesDataModel> list();
    }
}
