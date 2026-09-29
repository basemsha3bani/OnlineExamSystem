using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServicesClasseslibrary.Implmentation.Builder
{
  public interface IExamAttemptQuestionBuilder
    {
        List <QuestionsDataModel> BuildExamAttemptQuestions(int studySubjectId  , ExamSectionRulesDataModel r);
    }
    public class ExamAttemptQuestionBuilder : IExamAttemptQuestionBuilder
    {
        private readonly IQuestionsOperations   _questionRepository;

        public ExamAttemptQuestionBuilder(IQuestionsOperations questionRepository)
        {
            _questionRepository = questionRepository;
        }

        

        public List<QuestionsDataModel> BuildExamAttemptQuestions(int studySubjectId, ExamSectionRulesDataModel rule)
        {

            var x = _questionRepository.GetRandomQuestions(studySubjectId, rule);
            return x;
        }
    }
}
