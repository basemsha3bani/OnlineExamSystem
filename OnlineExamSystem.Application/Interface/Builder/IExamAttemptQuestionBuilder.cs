using Domain.DTOs;
using System.Collections.Generic;

namespace OnlineExamSystem.Application.Interface.Builder
{
    public interface IExamAttemptQuestionBuilder
    {
        Dictionary<int, List<QuestionsDTO>> BuildExamAttemptQuestions(int examId, List<ExamSectionsDTO> sections);
        List<QuestionsDTO> BuildExamAttemptQuestions(int studySubjectId, ExamSectionRulesDTO r, IEnumerable<int> excludedIds = null);
    }
}
  
