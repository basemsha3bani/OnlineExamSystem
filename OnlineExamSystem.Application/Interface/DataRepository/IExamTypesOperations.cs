using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineExamSystem.Application.Interface.DataRepository
{
    public interface IExamTypesOperations
    {
        void Add(ExamTypesDTO examTypes);
        void Edit(ExamTypesDTO examTypes);



        void Delete(int id);


        ExamTypesDTO GetById(int id);

        List<ExamTypesDTO> list();
    }
}
