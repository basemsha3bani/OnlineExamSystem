using Domain.Entities;
using Domain.Aggregates;
using Domain.DTOs;
using ServicesClasseslibrary.Interface.DataRepository;
using ServicesClasseslibrary.Interface.DataModel;
using System.Collections.Generic;
using System.Linq;

namespace ServicesClasseslibrary
{
    public class StudySubjectsService : IStudySubjectsService
    {
        private readonly IStudySubjectsOperations _ops;
        private readonly IQuestionsOperations questions;
        private readonly IExamOprations exams;
        private readonly IExamSectionsOperations sections;
        public StudySubjectsService(IStudySubjectsOperations ops, IQuestionsOperations questions, IExamOprations exams, IExamSectionsOperations sections) { _ops = ops; this.questions = questions; this.exams = exams; this.sections = sections; }
        public void Add(StudySubjectDTO m) { Validate(m); _ops.Add(m); }
        private static void Validate(StudySubjectDTO m) => Domain.Aggregates.StudySubject.CheckDefinition(m, System.Array.Empty<int>());
        public void Delete(int id) => _ops.Delete(id);
        public void Edit(StudySubjectDTO m)
        {

            var used = questions.list().Where(q => q.StudySubjectId == m.Id)
                .Select(q => q.ChapterNumber)
                .Concat(
                        exams.list()
                              .Where(e => e.StudySubjectId == m.Id)
                             .SelectMany(e => sections.List(e.Id))
                              .SelectMany(s => s.ChapterNumbers)
                        ).GroupBy(g =>(int) g).Select(s=>s.First()).ToList();

            Domain.Aggregates.StudySubject.CheckDefinition(m, used);
            _ops.Edit(m);
        }
        public StudySubjectDTO GetById(int id) => _ops.GetById(id);
        public List<StudySubjectDTO> list() => _ops.list();
    }
}
