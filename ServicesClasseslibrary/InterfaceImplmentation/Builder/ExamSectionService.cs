using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using System.Collections.Generic;
using System.Linq;

namespace ServicesClasseslibrary.Implmentation.Builder
{
    public class ExamSectionService : IExamSectionService
    {
        private readonly IExamSectionsOperations _examSectionsOperations;
        private readonly IExamOprations exams;
        private readonly IStudySubjectsOperations subjects;
        public ExamSectionService(IExamSectionsOperations examSectionsOperations, IExamOprations exams, IStudySubjectsOperations subjects)
        {
            _examSectionsOperations = examSectionsOperations;
            this.exams = exams; this.subjects = subjects;
        }

        public List<ExamSectionsDataModel> List(int examId) => _examSectionsOperations.List(examId);
        public void Add(ExamSectionsDataModel m)
        {
            if (exams.GetById(m.ExamId) == null) throw new Domain.Exceptions.DomainException("Exam not found.");
            Domain.Services.ExamConfiguration.ValidateSectionName(m.SectionName);
            m.NumberOfQuestions = 0; m.ChapterNumbers = new List<int>(); m.ExamSectionRules = new List<ExamSectionRulesDataModel>();
            _examSectionsOperations.Add(m);
        }
        public ExamSectionsDataModel GetById(int id) => _examSectionsOperations.GeById(id);
        public void Edit(ExamSectionsDataModel m)
        {
            var current = GetById(m.Id) ?? throw new Domain.Exceptions.DomainException("Section not found.");
            m.ExamId = current.ExamId;
            var exam = exams.GetById(m.ExamId);
            var row = subjects.GetById(exam.StudySubjectId);
            var subject = new Domain.Aggregates.StudySubject(row.Id, row.SubjectName, row.NumberOfChapters);
            Domain.Services.ExamConfiguration.ValidateSectionDraft(subject, m.SectionName, m.NumberOfQuestions, m.ChapterNumbers);
            Domain.Services.ExamConfiguration.ValidateAllocation(m.NumberOfQuestions, current.ExamSectionRules.Select(r => new Domain.Aggregates.SectionDifficulty(new Domain.Aggregates.Difficulty(r.DifficultyLevelId), r.NoOfQuestions)));
            _examSectionsOperations.Edit(m);
        }
    }
}
