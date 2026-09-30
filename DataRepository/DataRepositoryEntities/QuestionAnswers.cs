using System.ComponentModel.DataAnnotations.Schema;

namespace DataRepository.DataRepositoryEntities
{
    public class QuestionAnswers : IRepository
    {
        public int Id { get; set; }
        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
        public virtual Questions Question { get; set; }
    }
}
