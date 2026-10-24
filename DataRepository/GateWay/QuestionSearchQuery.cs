using DataModel;
using DataRepository.DataRepositoryEntities;
using System.Linq;
namespace DataRepository.GateWay
{
    public static class QuestionSearchQuery
    {
        public static IQueryable<Questions> Filter(IQueryable<Questions> query, QuestionSearch search)
        {
            if (search.StudySubjectId.HasValue) query = query.Where(q => q.StudySubjectId == search.StudySubjectId.Value);
            if (search.ChapterNumber.HasValue) query = query.Where(q => q.ChapterNumber == search.ChapterNumber.Value);
            return query;
        }
        public static IQueryable<Questions> Page(IQueryable<Questions> query, QuestionSearch search)
            => query.OrderBy(q => q.Id).Skip((search.PageNumber - 1) * search.PageSize).Take(search.PageSize);
    }
}
