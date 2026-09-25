using System.ComponentModel.DataAnnotations;

namespace DataModel
{
    public class StudySubjectDataModel
    {
        public int Id { get; set; }
        [Required]
        public string SubjectName { get; set; }
    }
}
