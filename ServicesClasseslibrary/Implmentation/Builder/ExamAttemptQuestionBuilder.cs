using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DataRepository.GateWay;

namespace ServicesClasseslibrary.Implmentation.Builder
{
  public interface IExamAttemptQuestionBuilder
    {
        List <QuestionsDataModel> BuildExamAttemptQuestions(int studySubjectId, ExamSectionRulesDataModel r, IEnumerable<int> excludedIds = null);
    }
    public class ExamAttemptQuestionBuilder : IExamAttemptQuestionBuilder
    {
        private readonly IDbContextFactory<DbConext> contexts;
        private readonly AutoMapper.IMapper mapper;

        public ExamAttemptQuestionBuilder(IDbContextFactory<DbConext> contexts, AutoMapper.IMapper mapper)
        {
            this.contexts = contexts;
            this.mapper = mapper;
        }

        

        public List<QuestionsDataModel> BuildExamAttemptQuestions(int studySubjectId, ExamSectionRulesDataModel rule, IEnumerable<int> excludedIds = null)
        {

            using var db = contexts.CreateDbContext();
            var excluded = (excludedIds ?? Enumerable.Empty<int>()).ToArray();
            var questions = db.Questions.AsNoTracking().Include(q => q.QuestionAnswers)
                .Where(q => q.StudySubjectId == studySubjectId && q.DifficultyLevelId == rule.DifficultyLevelId && !excluded.Contains(q.Id))
                .OrderBy(q => Guid.NewGuid()).Take(rule.NoOfQuestions).ToList();
            return mapper.Map<List<QuestionsDataModel>>(questions);
        }
    }
}
