using Domain.Entities;
using Domain.Aggregates;
using Domain.DTOs;
using System.Linq;
namespace DataRepository.GateWay
{
    public static class QuestionSearchQuery
    {
        public static IQueryable<Questions> Filter(IQueryable<Questions> query, QuestionSearchDTO search)
        {
            if (search.StudySubjectId.HasValue) query = query.Where(q => q.StudySubjectId == search.StudySubjectId.Value);
            if (search.ChapterNumber.HasValue) query = query.Where(q => q.ChapterNumber == search.ChapterNumber.Value);
            return query;
        }
        public static IQueryable<Questions> Page(IQueryable<Questions> query, QuestionSearchDTO search)
            => query.OrderBy(q => q.Id).Skip((search.PageNumber - 1) * search.PageSize).Take(search.PageSize);
    }
}
