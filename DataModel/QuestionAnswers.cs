using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace  DataModel
{
  public  class QuestionAnswersDataModel
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }
       // [Required]
        public String AnswerText { get; set; }

        public bool IsCorrext { get; set; }  
    }
}
