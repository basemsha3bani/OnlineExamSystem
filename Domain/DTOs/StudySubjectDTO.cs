namespace Domain.DTOs
{
    public class StudySubjectDTO
    {
        [System.Text.Json.Serialization.JsonInclude]
        public int Id { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public int NumberOfChapters { get; private set; } = 1;
        [System.Text.Json.Serialization.JsonInclude]
        public string SubjectName { get; private set; }
    
        [System.Text.Json.Serialization.JsonConstructor]

        private StudySubjectDTO() { }
        public static StudySubjectDTO Define(int Id = default, int NumberOfChapters = 1, string SubjectName = default)
        {
            var value = new StudySubjectDTO();
            value.Id = Id;
            value.NumberOfChapters = NumberOfChapters;
            value.SubjectName = SubjectName;
            return value;
        }
        public void ChangeId( int value) => Id = value;
        public void ChangeNumberOfChapters( int value) => NumberOfChapters = value;
        public void ChangeSubjectName( string value) => SubjectName = value;
}
}
