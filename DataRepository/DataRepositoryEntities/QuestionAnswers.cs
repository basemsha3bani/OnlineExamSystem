using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataRepository.DataRepositoryEntities
{
  public  class QuestionAnswers:IRepository
    {
        public int Id { get; set; }
        [ForeignKey("Question")]
        public int QuestionId { get; set; }

        public String AnswerText { get; set; }

        public bool IsCorrect { get; set; }

        public virtual Questions Question { get; set; }
    }
}
