using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineExamSystem.Application.Interface
{
    public interface IQuestionAnswersService
    {
        void Add(QuestionAnswersDTO questionAnswers);
        void Edit(QuestionAnswersDTO questionAnswers);



        void Delete(int id);


        QuestionAnswersDTO GetById(int id);

        List<QuestionAnswersDTO> list();
    }
}
