using DataModel;
using ServicesClasseslibrary.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServicesClasseslibrary
{
    public class ExamTypesService : IExamTypesService
    {
        void Add(ExamTypesDataModel examTypes)
        {

        }
        void Edit(ExamTypesDataModel examTypes)
        {

        }
        
      


        void Delete(int id)
        {

        }

       



        ExamTypesDataModel GetById(int id)
        {
            return null;
        }

        List<ExamTypesDataModel> list()
        {
            return null;
        }

        void IExamTypesService.Add(ExamTypesDataModel examTypes)
        {
            throw new NotImplementedException();
        }

        void IExamTypesService.Edit(ExamTypesDataModel examTypes)
        {
            throw new NotImplementedException();
        }

        void IExamTypesService.Delete(int id)
        {
            throw new NotImplementedException();
        }

        ExamTypesDataModel IExamTypesService.GetById(int id)
        {
            throw new NotImplementedException();
        }

        List<ExamTypesDataModel> IExamTypesService.list()
        {
            throw new NotImplementedException();
        }
    } 
}
