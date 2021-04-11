using System;
using System.Collections.Generic;
using System.Text;

namespace DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface
{
    interface IQuestionAnswersOperations
    {
        void Add(QuestionAnswers questionAnswers);
        void Edit(QuestionAnswers questionAnswers);



        void Delete(int id);


        QuestionAnswers GetById(int id);

        List<QuestionAnswers> list();
    }
}
