using DataModel;
using System.Collections.Generic;

namespace ServicesClasseslibrary.Interface.DataModel
{
    public interface IStudySubjectsService
    {
        void Add(StudySubjectDataModel model);
        void Edit(StudySubjectDataModel model);
        void Delete(int id);
        StudySubjectDataModel GetById(int id);
        List<StudySubjectDataModel> list();
    }
}
