using Domain.DTOs;
using System.Collections.Generic;

namespace ServicesClasseslibrary.Interface.Builder
{
    public interface IExamAttemptQuestionBuilder
    {
        Dictionary<int, List<QuestionsDTO>> BuildExamAttemptQuestions(int examId, List<ExamSectionsDTO> sections);
        List<QuestionsDTO> BuildExamAttemptQuestions(int studySubjectId, ExamSectionRulesDTO r, IEnumerable<int> excludedIds = null);
    }
}
  
