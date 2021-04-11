using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataRepository.DataRepositoryEntities
{
    public class Exam
    {
        public int Id { get; set; }

        [ForeignKey("ExamType")]
        public int ExamTypeId { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ExamTypes ExamType { get; set; }

        public virtual ICollection<ExamQuestions> ExamQuestions { get; set; }

        



    }
}
