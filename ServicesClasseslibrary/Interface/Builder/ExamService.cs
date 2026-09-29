using DataModel;
using DataRepository.DataRepositoryEntities;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses.DataRepository.DataRepositoryEntities;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using ServicesClasseslibrary.Interface.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses.QuestionsOperations;

namespace ServicesClasseslibrary
{
   
    public class ExamService : IExamService
    {
        private readonly IStudySubjectsService _subjectService;
        private readonly IExamOprations _examsOperations;
        private readonly IExamSectionRuleOperations _rulesOps;
        public ExamService(IStudySubjectsService subjectService, IExamOprations examsOperations,IExamSectionRuleOperations rulesOps)
        {
            _subjectService = subjectService;
            _examsOperations = examsOperations;
            _rulesOps= rulesOps;
        }
        public List<ExamDataModel> List() => _examsOperations.list();
        public ExamDataModel GetById(int id)
        {
            var e = _examsOperations.GetById(id);
            return new ExamDataModel { Id = e.Id,StudySubjectId=e.StudySubjectId, Title = e.Title, StudySubjectName = e.StudySubjectName, TotalMarks = e.TotalMarks,
            Sections=e.Sections.Select(s=>
            new ExamSectionsDataModel
            {
                Id=s.Id,

                SectionName=s.SectionName,
                ExamId=s.ExamId,
                Percentage=s.Percentage
            }).ToList()};
        }
        public void Add(ExamDataModel exam)
        {
            _examsOperations.Add(exam);
           
        }

        public void Edit(ExamDataModel exam)
        {
            _examsOperations.Edit(exam);
           
        }

        public List<ExamSectionRulesDataModel> GetSectionRules(int sectionId)
        {
            // Service -> Operations -> Gateway
                var entities = _rulesOps.GetBySectionId(sectionId);

            return entities.Select(e => new ExamSectionRulesDataModel
            {
                Id = e.Id,
                SectionId = e.SectionId,
                DifficultyLevelId = e.DifficultyLevelId,
                DifficultyLevel = e.difficultyLevel.DifficultyLevelName,
                subjectName = e.section.Exam.StudySubject.SubjectName,
                NoOfQuestions = e.NoOfQuestions,
               
            }).ToList();
        }
        

        public void AddSectionRule(ExamSectionRulesDataModel model)
        {
           _rulesOps.Add(new ExamSectionRules
            {
                SectionId = model.SectionId,
                DifficultyLevelId = model.DifficultyLevelId,
                NoOfQuestions = model.NoOfQuestions
            });
          
        }

        
    }
}
