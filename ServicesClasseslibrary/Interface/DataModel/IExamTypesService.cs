using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServicesClasseslibrary
{
   public interface IExamTypesService 
    {
        void Add(ExamTypesDTO examTypes);
        void Edit(ExamTypesDTO examTypes);



        void Delete(int id);


        ExamTypesDTO GetById(int id);

        List<ExamTypesDTO> list();
    }
}
