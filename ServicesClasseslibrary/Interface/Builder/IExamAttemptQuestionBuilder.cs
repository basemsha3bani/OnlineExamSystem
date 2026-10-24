using DataModel;
using System.Collections.Generic;

namespace ServicesClasseslibrary.Interface.Builder
{
    public interface IExamAttemptQuestionBuilder
    {
        Dictionary<int, List<QuestionsDataModel>> BuildExamAttemptQuestions(int examId, List<ExamSectionsDataModel> sections);
        List<QuestionsDataModel> BuildExamAttemptQuestions(int studySubjectId, ExamSectionRulesDataModel r, IEnumerable<int> excludedIds = null);
    }
}
  
