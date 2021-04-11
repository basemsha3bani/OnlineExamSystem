using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataRepository.DataRepositoryEntities
{
   public class ExamTypes
    {
        public int Id { get; set; }
        public String ExamTypeName { get; set; }

        [ForeignKey("DifficultyLevel")]
        public int DifficultyLevelId { get; set; }

        public int NumberOfQuestions { get; set; }


        public virtual DifficultyLevels DifficultyLevel { get; set; }
    }
}
