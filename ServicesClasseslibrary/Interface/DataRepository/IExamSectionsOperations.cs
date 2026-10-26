using Domain.DTOs;
using System;
using System.Collections.Generic;

namespace ServicesClasseslibrary.Interface.DataRepository
{
    public interface IExamSectionsOperations
    {
        ExamSectionsDTO GeById(int id);
        void Edit(ExamSectionsDTO model);
        public void Add(ExamSectionsDTO examSectionsDataModel);
       

        public List<ExamSectionsDTO> List(int examId);

        
    }
}