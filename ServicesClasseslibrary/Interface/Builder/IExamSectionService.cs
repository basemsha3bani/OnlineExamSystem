using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
using System.Collections.Generic;

namespace ServicesClasseslibrary
{
    public interface IExamSectionService
    {
        public List<ExamSectionsDataModel> List(int examId);
        public void Add(ExamSectionsDataModel m);
    }
}

