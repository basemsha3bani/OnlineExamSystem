using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
    public class QuestionsDataModel
    {
        public int Id { get; set; }

        public String QuestionText{ get; set; }

        public int DifficultyLevelId { get; set; }
    }
}
