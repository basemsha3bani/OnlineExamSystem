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
            Exams exam = new Exams { Title = m.Title, StudySubjectId = m.StudySubjectId, TotalMarks = m.TotalMarks };
            List<ExamSections> sections = m.Sections.Select(s=>new ExamSections
            {
                
                Percentage=s.Percentage,
                SectionName=s.SectionName,
                Exam=exam
               
            }).ToList();
            ContextGateway<Exams>.CreateDatabaseTransaction();
            try
            {
                

                ContextGateway<Exams>.Add(exam);
                ContextGateway<Exams>.Add(sections);

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

            List<ExamSections> NewExamSections= new List<ExamSections>();

            
            exam.StudySubjectId = m.StudySubjectId;
            exam.TotalMarks = m.TotalMarks;
            m.Sections.ForEach(es =>
            {
                ExamSections section = exam.Sections.FirstOrDefault(w => w.Id == es.Id);

                if (section == null)
                {
                    NewExamSections.Add(new ExamSections
                    {
                        SectionName = es.SectionName,
                        Percentage = es.Percentage,
                        Exam = exam
                    });
                }
                else
                {
                    section.SectionName = es.SectionName;
                    section.Percentage = es.Percentage;
                }
                

            });

            ;
            ContextGateway<Questions>.CreateDatabaseTransaction();
            try
            {

                ContextGateway<Questions>.Edit(exam);
                ContextGateway<Questions>.Edit(exam.Sections);
                ContextGateway<Questions>.Add(NewExamSections);

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
