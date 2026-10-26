using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServicesClasseslibrary.Interface.DataRepository
{
    public interface IExamTypesDetailsOperations
    {
        void Add(ExamTypesDetailsDTO detailsDataModel);
        void Edit(ExamTypesDetailsDTO detailsDataModel);



        void Delete(int id);


        ExamTypesDTO GetById(int id);

        List<ExamTypesDetailsDTO> list();
    }
}
