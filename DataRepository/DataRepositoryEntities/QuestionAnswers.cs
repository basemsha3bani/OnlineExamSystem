using DataModel;
using DataRepository.ModelMapper.Interface;
using DevExpress.Data.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataRepository.DataRepositoryEntities
{
  public  class QuestionAnswers:IRepository,IModelMapper<QuestionAnswersDataModel, QuestionAnswers>
    {
        public int Id { get; set; }
        [ForeignKey("Question")]
        public int QuestionId { get; set; }

        public String AnswerText { get; set; }

        public bool IsCorrect { get; set; }

        public virtual Questions Question { get; set; }

        public QuestionAnswersDataModel Map(QuestionAnswers qa)
        {
            return new QuestionAnswersDataModel
            {
                QuestionId = qa.QuestionId,
                AnswerText = qa.AnswerText,
                IsCorrect = qa.IsCorrect,
                Id = qa.Id,
                radioButtonDisplay = string.Join("", qa.Id, "rbIsCorrect")
            };
        }
    }
}
