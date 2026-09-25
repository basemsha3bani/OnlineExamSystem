using DataModel;
using System.Collections.Generic;

namespace DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface
{
    public interface IStudySubjectsOperations
    {
        void Add(StudySubjectDataModel model);
        void Edit(StudySubjectDataModel model);
        void Delete(int id);
        StudySubjectDataModel GetById(int id);
        List<StudySubjectDataModel> list();
    }
}
