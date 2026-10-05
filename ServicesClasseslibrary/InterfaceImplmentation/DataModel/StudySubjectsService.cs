using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using ServicesClasseslibrary.Interface.DataModel;
using System.Collections.Generic;

namespace ServicesClasseslibrary
{
    public class StudySubjectsService : IStudySubjectsService
    {
        private readonly IStudySubjectsOperations _ops;
        public StudySubjectsService(IStudySubjectsOperations ops) { _ops = ops; }
        public void Add(StudySubjectDataModel m) => _ops.Add(m);
        public void Delete(int id) => _ops.Delete(id);
        public void Edit(StudySubjectDataModel m) => _ops.Edit(m);
        public StudySubjectDataModel GetById(int id) => _ops.GetById(id);
        public List<StudySubjectDataModel> list() => _ops.list();
    }
}
