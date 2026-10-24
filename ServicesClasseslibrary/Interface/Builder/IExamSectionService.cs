using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
using System.Collections.Generic;

namespace ServicesClasseslibrary
{
    public interface IExamSectionService
    {
        public List<ExamSectionsDataModel> List(int examId);
        ExamSectionsDataModel GetById(int id);
        void Edit(ExamSectionsDataModel model);
        public void Add(ExamSectionsDataModel m);
    }
}

