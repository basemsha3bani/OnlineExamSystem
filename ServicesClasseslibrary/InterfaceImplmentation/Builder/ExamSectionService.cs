using Domain.Entities;
using Domain.Aggregates;
using Domain.DTOs;
using ServicesClasseslibrary.Interface.DataRepository;
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

        public List<ExamSectionsDTO> List(int examId) => _examSectionsOperations.List(examId);
        public void Add(ExamSectionsDTO m)
        {
            ExamSections.CheckDraft(m, exams.GetById(m.ExamId));
            m.ChangeNumberOfQuestions(0); m.ChangeChapterNumbers(new List<int>()); m.ChangeExamSectionRules(new List<ExamSectionRulesDTO>());
            _examSectionsOperations.Add(m);
        }
        public ExamSectionsDTO GetById(int id) => _examSectionsOperations.GeById(id);
        public void Edit(ExamSectionsDTO m)
        {
            var current = GetById(m.Id);
            m.ChangeExamId(ExamSections.ParentOf(current));
            var exam = exams.GetById(m.ExamId);
            var row = subjects.GetById(ExamSections.SubjectOf(exam));
            ExamSections.CheckConfiguration(m, current, row);
            _examSectionsOperations.Edit(m);
        }
    }
}
