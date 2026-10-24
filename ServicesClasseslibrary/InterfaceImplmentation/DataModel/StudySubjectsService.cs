using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using ServicesClasseslibrary.Interface.DataModel;
using System.Collections.Generic;

namespace ServicesClasseslibrary
{
    public class StudySubjectsService : IStudySubjectsService
    {
        private readonly IStudySubjectsOperations _ops;
        private readonly IQuestionsOperations questions;
        private readonly IExamOprations exams;
        private readonly IExamSectionsOperations sections;
        public StudySubjectsService(IStudySubjectsOperations ops, IQuestionsOperations questions, IExamOprations exams, IExamSectionsOperations sections) { _ops = ops; this.questions = questions; this.exams = exams; this.sections = sections; }
        public void Add(StudySubjectDataModel m) { Validate(m); _ops.Add(m); }
        private static void Validate(StudySubjectDataModel m) => _ = new Domain.Aggregates.StudySubject(m.Id, m.SubjectName, m.NumberOfChapters);
        public void Delete(int id) => _ops.Delete(id);
        public void Edit(StudySubjectDataModel m)
        {
            Validate(m);
            var subject = new Domain.Aggregates.StudySubject(m.Id, m.SubjectName, m.NumberOfChapters);
            foreach (var q in questions.list()) if (q.StudySubjectId == m.Id) subject.ValidateChapter(q.ChapterNumber);
            foreach (var exam in exams.list()) if (exam.StudySubjectId == m.Id)
                foreach (var section in sections.List(exam.Id))
                    foreach (var chapter in section.ChapterNumbers) subject.ValidateChapter(chapter);
            _ops.Edit(m);
        }
        public StudySubjectDataModel GetById(int id) => _ops.GetById(id);
        public List<StudySubjectDataModel> list() => _ops.list();
    }
}
