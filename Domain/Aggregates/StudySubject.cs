using Domain.Exceptions;
using Domain.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Aggregates
{
    public class StudySubject : Domain.Abstractions.IAggregateRoot
    {
        public int Id { get; private set; }
        public int NumberOfChapters { get; private set; } = 1;

        public string SubjectName { get; private set; }

        private StudySubject() { }
        public static StudySubject Define(int Id = default, int NumberOfChapters = 1, string SubjectName = default)
        {
            var value = new StudySubject();
            value.ChangeId(Id);
            value.ChangeNumberOfChapters(NumberOfChapters);
            value.ChangeSubjectName(SubjectName);
            return value;
        }
        public void ChangeId(int value) => Id = EntityRules.NonNegative(value, "Id");
        public void ChangeNumberOfChapters(int value) => NumberOfChapters = EntityRules.Positive(value, "NumberOfChapters");
        public void ChangeSubjectName(string value) => SubjectName = EntityRules.Required(value, "SubjectName");
        public void ValidateChapter(int chapter)
        {
            if (chapter < 1 || chapter > NumberOfChapters) throw new InvalidChapterException(chapter, NumberOfChapters);
        }

        public static void CheckDefinition(StudySubjectDTO model, IEnumerable<int> usedChapters)
        {
            int RequiredNumberOfChapters = usedChapters?.Max()??1;
            if(model.NumberOfChapters < RequiredNumberOfChapters)
            {
                throw new InvalidNumberOfChapterException(RequiredNumberOfChapters);
            }
            var subject = StudySubject.Define(Id: model.Id, SubjectName: model.SubjectName, NumberOfChapters: model.NumberOfChapters);
           
        }

    }
}
