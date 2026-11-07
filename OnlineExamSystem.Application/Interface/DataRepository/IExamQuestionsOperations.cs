using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineExamSystem.Application.Interface.DataRepository
{
    public interface IExamQuestionsOperations
    {
        void Add(ExamQuestionsDTO Exam);
        void Edit(ExamQuestionsDTO model);



        void Delete(int id);


        ExamQuestionsDTO GetById(int id);

        List<ExamQuestionsDTO> list();
    }
}
