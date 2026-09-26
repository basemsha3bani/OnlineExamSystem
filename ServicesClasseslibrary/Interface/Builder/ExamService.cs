using DataModel;
using DataRepository.DataRepositoryEntities;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using ServicesClasseslibrary.Interface.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using static DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses.QuestionsOperations;

namespace ServicesClasseslibrary
{
   
    public class ExamService : IExamService
    {
        private readonly IStudySubjectsService _subjectService;
        private readonly IExamOprations _examsOperations;
        public ExamService(IStudySubjectsService subjectService, IExamOprations examsOperations)
        {
            _subjectService = subjectService;
            _examsOperations = examsOperations;
        }
        public List<ExamDataModel> List() => _examsOperations.list();
        public ExamDataModel GetById(int id)
        {
            var e = _examsOperations.GetById(id);
            return new ExamDataModel { Id = e.Id, Title = e.Title, StudySubjectId = e.StudySubjectId, TotalMarks = e.TotalMarks };
        }
        public void Add(ExamDataModel exam)
        {
            _examsOperations.Add(exam);
           
        }

        public void Edit(ExamDataModel exam)
        {
            _examsOperations.Edit(exam);
           
        }

    }
}
