using System.ComponentModel.DataAnnotations;

namespace DataModel
{
    public class StudySubjectDataModel
    {
        public int Id { get; set; }
        public int NumberOfChapters { get; set; } = 1;
        [Required]
        public string SubjectName { get; set; }
    }
}
