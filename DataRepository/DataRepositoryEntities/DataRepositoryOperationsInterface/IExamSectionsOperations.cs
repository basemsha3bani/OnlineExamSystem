using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
using DataRepository.GateWay;
using System;
using System.Collections.Generic;

namespace DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface
{
    public interface IExamSectionsOperations
    {
        public void Add(ExamSectionsDataModel examSectionsDataModel);
       

        public List<ExamSectionsDataModel> List(int examId);

        
    }
}