
using System.Collections.Generic;

namespace Domain.DTOs
{
    public class ExamDTO
    {
        [System.Text.Json.Serialization.JsonInclude]
        public int Id { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public string Title { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public int StudySubjectId { get; private set; }

        [System.Text.Json.Serialization.JsonInclude]
        public string StudySubjectName { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public decimal TotalMarks { get; private set; }

        [System.Text.Json.Serialization.JsonInclude]
        public List<ExamSectionsDTO> Sections { get; private set; } = new List<ExamSectionsDTO>();
    
        [System.Text.Json.Serialization.JsonConstructor]

        private ExamDTO() { }
        public static ExamDTO Define(int Id = default, string Title = default, int StudySubjectId = default, string StudySubjectName = default, decimal TotalMarks = default, List<ExamSectionsDTO> Sections = default)
        {
            var value = new ExamDTO();
            value.Id = Id;
            value.Title = Title;
            value.StudySubjectId = StudySubjectId;
            value.StudySubjectName = StudySubjectName;
            value.TotalMarks = TotalMarks;
            value.Sections = Sections ?? new List<ExamSectionsDTO>();
            return value;
        }
        public void ChangeId( int value) => Id = value;
        public void ChangeTitle( string value) => Title = value;
        public void ChangeStudySubjectId( int value) => StudySubjectId = value;
        public void ChangeStudySubjectName( string value) => StudySubjectName = value;
        public void ChangeTotalMarks( decimal value) => TotalMarks = value;
        public void ChangeSections( List<ExamSectionsDTO> value) => Sections = value;
}
}
