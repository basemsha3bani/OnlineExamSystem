using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServicesClasseslibrary.Interface
{
   public interface IQuestionsService
    {
        void Add(QuestionsDTO questionAnswers);
        void Edit(QuestionsDTO questionAnswers);



        void Delete(int id);


        QuestionsDTO GetById(int id);

        QuestionPageDTO Search(QuestionSearchDTO search);
        List<QuestionsDTO> list();
    }
}
