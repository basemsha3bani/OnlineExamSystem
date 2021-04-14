using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataRepository.DataRepositoryEntities
{
   public class ExamQuestions
    {
        public int Id { get; set; }

        [ForeignKey("Exam")]
        public int ExamId { get; set; }

        public virtual Exam Exam{get;set;}

        [ForeignKey("Question")]
        public int QuestionId { get; set; }

        public virtual Questions Question { get; set; }


    }
}
