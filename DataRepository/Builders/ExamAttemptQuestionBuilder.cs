using Domain.Aggregates;
using Domain.Entities;
using Domain.DTOs;
using DataRepository.GateWay;
using Microsoft.EntityFrameworkCore;
using ServicesClasseslibrary.Interface.Builder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataRepository.Builders;

public class ExamAttemptQuestionBuilder : IExamAttemptQuestionBuilder
{
    private readonly IDbContextFactory<DbConext> contexts;
    private readonly AutoMapper.IMapper mapper;

    public ExamAttemptQuestionBuilder(IDbContextFactory<DbConext> contexts, AutoMapper.IMapper mapper)
    {
        this.contexts = contexts;
        this.mapper = mapper;
    }

    public Dictionary<int, List<QuestionsDTO>> BuildExamAttemptQuestions(int examId, List<ExamSectionsDTO> sections)
    {
        using var db = contexts.CreateDbContext();
        var exam = db.Exams.AsNoTracking().Single(e => e.Id == examId);
        var subject = db.StudySubjects.AsNoTracking().Single(s => s.Id == exam.StudySubjectId);
        exam.ChangeStudySubject(subject);
        exam.ChangeSections(sections.Select(ToSection).ToList());
        exam.ValidateForAttempt();
        var difficultyIds = exam.Sections.SelectMany(s => s.examSectionRules).Select(r => r.DifficultyLevelId).Distinct().ToArray();
        var chapters = exam.Sections.SelectMany(s => s.ChapterNumbers).Distinct().ToArray();
        var rows = LoadQuestions(db, subject.Id, difficultyIds, chapters, Array.Empty<int>());
        var selected = new Domain.Services.ExamDomainService().BuildExamAttemptQuestions(exam, rows);
        var byId = rows.ToDictionary(q => q.Id, q => mapper.Map<QuestionsDTO>(q));
        var result = new Dictionary<int, List<QuestionsDTO>>();
        int offset = 0;
        foreach (var section in exam.Sections)
        {
            result[section.Id] = selected.Skip(offset).Take(section.NumberOfQuestions).Select(q => byId[q.Id]).ToList();
            offset += section.NumberOfQuestions;
        }
        return result;
    }

    public List<QuestionsDTO> BuildExamAttemptQuestions(int studySubjectId, ExamSectionRulesDTO rule, IEnumerable<int> excludedIds = null)
    {
        using var db = contexts.CreateDbContext();
        var excluded = (excludedIds ?? Enumerable.Empty<int>()).ToArray();
        var subject = db.StudySubjects.AsNoTracking().Single(s => s.Id == studySubjectId);
        var chapters = Enumerable.Range(1, subject.NumberOfChapters).ToList();
        var section = ExamSections.Define(Id: rule.SectionId, SectionName: "Question selection", NumberOfQuestions: rule.NoOfQuestions,
            ChapterNumbers: chapters, examSectionRules: new() {
                ExamSectionRules.Define(DifficultyLevelId: rule.DifficultyLevelId, NoOfQuestions: rule.NoOfQuestions) });
        var exam = Exam.Define(Title: "Question selection", StudySubjectId: subject.Id, StudySubject: subject, Sections: new() { section });
        var questions = LoadQuestions(db, studySubjectId, new[] { rule.DifficultyLevelId }, chapters.ToArray(), excluded);
        var selected = new Domain.Services.ExamDomainService().BuildExamAttemptQuestions(exam, questions);
        return mapper.Map<List<QuestionsDTO>>(selected);
    }

    private static ExamSections ToSection(ExamSectionsDTO section) => ExamSections.Define(Id: section.Id, ExamId: section.ExamId,
        SectionName: section.SectionName ?? "Question selection", NumberOfQuestions: section.NumberOfQuestions,
        ChapterNumbers: section.ChapterNumbers.ToList(), examSectionRules: section.ExamSectionRules.OrderBy(r => r.Id)
            .Select(r => ExamSectionRules.Define(Id: r.Id, DifficultyLevelId: r.DifficultyLevelId, NoOfQuestions: r.NoOfQuestions)).ToList());

    private static List<Questions> LoadQuestions(DbConext db, int subjectId, int[] difficultyIds, int[] chapters, int[] excludedIds)
    {
        // Keep root ordering stable while EF materializes the complete answer collections.
        var rows = db.Questions.AsNoTrackingWithIdentityResolution()
            .Where(q => q.StudySubjectId == subjectId && difficultyIds.Contains(q.DifficultyLevelId)
                && chapters.Contains(q.ChapterNumber) && !excludedIds.Contains(q.Id))
            .Include(q => q.QuestionAnswers).OrderBy(q => q.Id).ToList().DistinctBy(q => q.Id).ToArray();
        Random.Shared.Shuffle(rows);
        return rows.ToList();
    }
}
