using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
   public class ExamTypesDataModel
    {
        public int Id { get; set; }
        public String ExamTypeName { get; set; }
        public int DifficultyLevelId { get; set; }

        public int NumberOfQuestions { get; set; }
    }
}
