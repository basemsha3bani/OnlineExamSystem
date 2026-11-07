using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineExamSystem.Application
{
    public interface IExamQuestionsService
    {
        void Add(ExamQuestionsDTO examQuestions);
        void Edit(ExamQuestionsDTO examQuestions);



        void Delete(int id);


        ExamQuestionsDTO GetById(int id);

        List<ExamQuestionsDTO> list();

    }
}
