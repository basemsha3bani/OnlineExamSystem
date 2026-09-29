using DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServicesClasseslibrary.Interface.Builder
{
    public class ExamAttempt
    {
        public List<QuestionsDataModel> Questions { get; set; } = new List<QuestionsDataModel>();
    }
}
  
