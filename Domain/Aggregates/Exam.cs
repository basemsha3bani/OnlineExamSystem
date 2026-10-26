using Domain.Exceptions;
using Domain.DTOs;
using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Aggregates
{
    public class Exam : Domain.Abstractions.IAggregateRoot
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public int StudySubjectId { get; private set; }

        public decimal TotalMarks { get; private set; }

        public StudySubject StudySubject { get; private set; }

        public List<ExamSections> Sections { get; private set; } = new List<ExamSections>();

        private Exam() { }
        public static Exam Define(int Id = default, string Title = default, int StudySubjectId = default, decimal TotalMarks = default, StudySubject StudySubject = default, List<ExamSections> Sections = default)
        {
            var value = new Exam();
            value.ChangeId(Id);
            value.ChangeTitle(Title);
            value.ChangeStudySubjectId(StudySubjectId);
            value.ChangeTotalMarks(TotalMarks);
            value.ChangeStudySubject(StudySubject);
            value.Sections = Sections ?? new List<ExamSections>();
            return value;
        }
        public void ChangeId(int value) => Id = EntityRules.NonNegative(value, "Id");
        public void ChangeTitle(string value) => Title = EntityRules.Required(value, "Title");
        public void ChangeStudySubjectId(int value) => StudySubjectId = EntityRules.NonNegative(value, "StudySubjectId");
        public void ChangeTotalMarks(decimal value) => TotalMarks = value;
        public void ChangeStudySubject(StudySubject value) => StudySubject = value;
        public void ChangeSections(List<ExamSections> value) => Sections = value;

        public static Domain.Aggregates.StudySubject CheckDefinition(ExamDTO model, StudySubjectDTO subject, IEnumerable<int> usedChapters)
        {
            EntityRules.Exists(subject, "Subject");
            EntityRules.Required(model.Title, "Exam title");
            var definition = global::Domain.Aggregates.StudySubject.Define(Id: subject.Id, SubjectName: subject.SubjectName, NumberOfChapters: subject.NumberOfChapters);
           
            return definition;
        }
        public void CheckSectionIdentities(IEnumerable<ExamSectionsDTO> requested)
        {
            if (requested.Any(s => s.Id != 0 && Sections.All(e => e.Id != s.Id)))
            throw new DomainException("Invalid section identity.");
        }

        public void ValidateForAttempt()
        {
            EntityRules.Exists(StudySubject, "Subject");
            if (StudySubjectId != StudySubject.Id) throw new DomainException("Exam subject does not match its definition.");
            if (Sections == null || Sections.Count == 0) throw new DomainException("This exam has no configured sections.");
            foreach (var section in Sections)
            {
                EntityRules.Exists(section, "Section");
                section.ValidateForAttempt(StudySubject);
            }
        }

    }
}
