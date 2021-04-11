using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataRepository.DataRepositoryEntities
{
    public class ExamTypesDetails
    {
        public int Id { get; set; }
        [ForeignKey("DifficultyLevel")]
        public int DifficultyLevelId { get; set; }

        [ForeignKey("ExamType")]
        public int ExamTypeId { get; set; }

        public int NumberOfQuestions { get; set; }

        public virtual DifficultyLevels DifficultyLevel { get; set; }





    }
}
