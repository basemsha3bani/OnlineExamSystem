using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using DataRepository.ModelMapper.Interface;
using System.Collections.Generic;
using System.Linq;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class StudySubjectsOperations : IStudySubjectsOperations, IModelMapper<StudySubjectDataModel>
    {
        public StudySubjectsOperations()
        {
            ContextGateway<StudySubject>.GetContextInstance();
        }

        public void Add(StudySubjectDataModel model)
        {
            var entity = new StudySubject { SubjectName = model.SubjectName };
            ContextGateway<StudySubject>.Add(entity);
        }

        public void Delete(int id)
        {
            var entity = ContextGateway<StudySubject>.GetById(g => g.Id == id);
            ContextGateway<StudySubject>.Delete(entity);
        }

        public void Edit(StudySubjectDataModel model)
        {
            var old = ContextGateway<StudySubject>.GetById(g => g.Id == model.Id);
            var @new = new StudySubject { Id = model.Id, SubjectName = model.SubjectName };
            ContextGateway<StudySubject>.Edit(old, @new);
        }

        public StudySubjectDataModel GetById(int id)
        {
            var entity = ContextGateway<StudySubject>.GetById(g => g.Id == id);
            return Map(entity);
        }

        public List<StudySubjectDataModel> list()
        {
            return ContextGateway<StudySubject>.List()
               .Select(s => new StudySubjectDataModel { Id = s.Id, SubjectName = s.SubjectName }).ToList();
        }

        public StudySubjectDataModel Map(IRepository obj)
        {
            var e = (StudySubject)obj;
            return new StudySubjectDataModel { Id = e.Id, SubjectName = e.SubjectName };
        }
    }
}
