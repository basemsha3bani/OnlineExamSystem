using Domain.Entities;
using Domain.Aggregates;
using Domain.DTOs;
using DataRepository.GateWay;
using Microsoft.EntityFrameworkCore;
static class QuestionListingChecks
{
    public static void Run(DbConext db)
    {
        var rows = Enumerable.Range(1, 45).Reverse().Select(id => Questions.Define(
            Id: id, StudySubjectId: id <= 30 ? 1 : 2, QuestionText: "Question", DifficultyLevelId: 1, ChapterNumber: id % 2 == 0 ? 2 : 1 )).AsQueryable();
        var search = QuestionSearchDTO.Define(1, 2, 2, 10);
        var filtered = QuestionSearchQuery.Filter(rows, search);
        var selected = QuestionSearchQuery.Page(filtered, search).Select(q => q.Id).ToList();
        if (filtered.Count() != 15 || !selected.SequenceEqual(new[] { 22, 24, 26, 28, 30 })) throw new Exception("Filters must be applied before stable pagination");
        if (QuestionSearchQuery.Filter(rows, QuestionSearchDTO.Define(2)).Count() != 15 || QuestionSearchQuery.Filter(rows, QuestionSearchDTO.Define(null, 1)).Count() != 23) throw new Exception("Subject/chapter filters must work independently");
        if (QuestionSearchQuery.Filter(rows, QuestionSearchDTO.Define(99, 1)).Any()) throw new Exception("Nonmatching filters must return no results");
        var bounded = QuestionSearchDTO.Define(-1, 0, int.MaxValue, int.MaxValue);
        if (bounded.PageSize != 20 || bounded.PageFor(45) != 3 || bounded.PageFor(0) != 1 || bounded.StudySubjectId != null || bounded.ChapterNumber != null || QuestionSearchDTO.Define(pageNumber: -10).PageNumber != 1) throw new Exception("Invalid paging/filter values were not bounded");
        var sql = QuestionSearchQuery.Page(QuestionSearchQuery.Filter(db.Questions.AsNoTracking(), search), search).ToQueryString();
        if (!sql.Contains("WHERE") || !sql.Contains("[StudySubjectId]") || !sql.Contains("[ChapterNumber]") || !sql.Contains("ORDER BY") || !sql.Contains("OFFSET") || !sql.Contains("FETCH NEXT")) throw new Exception("Filtering/pagination must translate to SQL");
        Console.WriteLine("Passed subject/chapter filtering, stable pagination, empty results, page boundaries, and SQL translation.");
    }
}
