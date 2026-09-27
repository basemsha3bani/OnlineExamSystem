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

    public class ExamsOperations : IExamOprations, IModelMapper<ExamDataModel, Exams>
    {
        public ExamsOperations()
        {
            ContextGateway<Exams>.GetContextInstance();
        }

        public List<ExamDataModel> list()
        {
            var exams = ContextGateway<Exams>.List(l => l.Id == l.Id, l => l.StudySubject);

            return exams.Select(e => new ExamDataModel
            {
                Id = e.Id,
                Title = e.Title,
                StudySubjectId = e.StudySubjectId,
                StudySubjectName = e.StudySubject.SubjectName,
                TotalMarks = e.TotalMarks
            }).ToList();
        }
        public void Add(ExamDataModel m)
            {
            List<ExamSections> sections = m.Sections.Select(s=>new ExamSections
            {
                
                Percentage=s.Percentage,
                SectionName=s.SectionName,
               
            }).ToList();
            ContextGateway<Exams>.CreateDatabaseTransaction();
            try
            {
                ContextGateway<Exams>.Add(new Exams { Title = m.Title, StudySubjectId = m.StudySubjectId, TotalMarks = m.TotalMarks,Sections=sections });
            
                ContextGateway<Exams>.Commit();
            }
            catch (Exception ex)
            {
                ContextGateway<Exams>.Rollback();
                throw;
            }

        }
            public void Edit(ExamDataModel m)
        {
            Exams exam = ContextGateway<Exams>.GetById(g => g.Id == m.Id, g => g.Sections);



            
            exam.StudySubjectId = m.StudySubjectId;
            exam.TotalMarks = m.TotalMarks;
            exam.Sections.ForEach(es =>
            {
                ExamSections section = exam.Sections.First(w => w.Id == es.Id);


                section.SectionName = es.SectionName;
                section.Percentage = es.Percentage;
                

            }
                );

            ;
            ContextGateway<Questions>.CreateDatabaseTransaction();
            try
            {

                ContextGateway<Questions>.Edit(exam);
                ContextGateway<Questions>.Edit(exam.Sections);

                ContextGateway<Questions>.Commit();
            }
            catch (Exception ex)
            {
                ContextGateway<Questions>.Rollback();
                throw;
            }
        }
        

            public ExamDataModel GetById(int id)
            {
           Exams exam=  ContextGateway<Exams>.GetById(g=>g.Id==id,g=>g.Sections);
            return this.Map(exam);

        }

        public ExamDataModel Map(Exams repository)
        {
            return new ExamDataModel
            {
                Id = repository.Id,
                StudySubjectId = repository.StudySubjectId,
                Title = repository.Title,
                TotalMarks = repository.TotalMarks,
                Sections = repository.Sections.Select(
                    s =>
                    new ExamSectionsDataModel
                    {
                        ExamId = s.ExamId,
                        SectionName = s.SectionName,
                        Id = s.Id,
                        Percentage = s.Percentage
                    }).ToList()
            };
        }
    }
    
}
