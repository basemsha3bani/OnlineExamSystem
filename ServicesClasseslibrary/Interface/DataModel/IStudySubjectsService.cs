using Domain.DTOs;
using System.Collections.Generic;

namespace ServicesClasseslibrary.Interface.DataModel
{
    public interface IStudySubjectsService
    {
        void Add(StudySubjectDTO model);
        void Edit(StudySubjectDTO model);
        void Delete(int id);
        StudySubjectDTO GetById(int id);
        List<StudySubjectDTO> list();
    }
}
