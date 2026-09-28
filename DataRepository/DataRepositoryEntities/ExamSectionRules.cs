using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class ExamSectionRules:IRepository
    {
        public int Id { get; set; }
        [ForeignKey("section")]
        public int SectionId { get; set; }
        [ForeignKey("difficultyLevel")]
        public int DifficultyLevelId { get; set; }
        public int NoOfQuestions { get; set; }
        public ExamSections section { get; set; }
        public DifficultyLevels difficultyLevel { get; set; }
    }
}
