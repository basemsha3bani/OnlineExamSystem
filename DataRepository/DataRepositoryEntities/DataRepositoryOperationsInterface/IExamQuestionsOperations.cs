using DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface
{
    interface IExamQuestionsOperations
    {
        void Add(ExamQuestionsDataModel Exam);
        void Edit(ExamQuestionsDataModel model);



        void Delete(int id);


        ExamQuestionsDataModel GetById(int id);

        List<ExamQuestionsDataModel> list();
    }
}
