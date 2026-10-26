using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServicesClasseslibrary
{
   public interface IExamTypesDetailsService
    {
        void Add(ExamTypesDetailsDTO examTypesDetails);
        void Edit(ExamTypesDetailsDTO examTypesDetails);



        void Delete(int id);


        ExamTypesDetailsDTO GetById(int id);

        List<ExamTypesDetailsDTO> list();
    }
}
