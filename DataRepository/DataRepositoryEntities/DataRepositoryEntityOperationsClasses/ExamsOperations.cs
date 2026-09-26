using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using DataRepository.ModelMapper.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using static DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses.ExamsOperations;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
 
        public partial class ExamsOperations: IExamOprations, IModelMapper<ExamDataModel, Exams>
    {
            public  List<ExamDataModel> list()
            {
                var exams = ContextGateway<Exams>.List();
                var subjects = ContextGateway<StudySubject>.List().ToDictionary(x => x.Id, x => x.SubjectName);
                return exams.Select(e => new ExamDataModel
                {
                    Id = e.Id,
                    Title = e.Title,
                    StudySubjectId = e.StudySubjectId,
                  
                    TotalMarks = e.TotalMarks
                }).ToList();
            }
            public void Add(ExamDataModel m) => ContextGateway<Exams>.Add(new Exams { Title = m.Title, StudySubjectId = m.StudySubjectId, TotalMarks = m.TotalMarks });
            public void Edit(ExamDataModel m) => ContextGateway<Exams>.Edit(new Exams { Id = m.Id, Title = m.Title, StudySubjectId = m.StudySubjectId, TotalMarks = m.TotalMarks });

            public ExamDataModel GetById(int id)
            {
           Exams exam=  ContextGateway<Exams>.GetById(g=>g.Id==id);
            return this.Map(exam);

        }

        public ExamDataModel Map(Exams repository)
        {
            return new ExamDataModel
            {
                Id = repository.Id,
                StudySubjectId = repository.StudySubjectId,
                Title = repository.Title,
                TotalMarks = repository.TotalMarks
            };
        }
    }
    
}
