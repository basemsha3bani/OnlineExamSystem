using DataModel;
using System.Collections.Generic;

namespace ServicesClasseslibrary.Interface.Builder
{
    public interface IExamAttemptQuestionBuilder
    {
        List<QuestionsDataModel> BuildExamAttemptQuestions(int studySubjectId, ExamSectionRulesDataModel r, IEnumerable<int> excludedIds = null);
    }
}
  
