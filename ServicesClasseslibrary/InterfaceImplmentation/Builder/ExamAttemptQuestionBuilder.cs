using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DataRepository.GateWay;
using ServicesClasseslibrary.Interface.Builder;

namespace ServicesClasseslibrary.Implmentation.Builder
{
 
    public class ExamAttemptQuestionBuilder : IExamAttemptQuestionBuilder
    {
        private readonly IDbContextFactory<DbConext> contexts;
        private readonly AutoMapper.IMapper mapper;

        public ExamAttemptQuestionBuilder(IDbContextFactory<DbConext> contexts, AutoMapper.IMapper mapper)
        {
            this.contexts = contexts;
            this.mapper = mapper;
        }

        

        public Dictionary<int, List<QuestionsDataModel>> BuildExamAttemptQuestions(int examId, List<ExamSectionsDataModel> sections)
        {
            using var db = contexts.CreateDbContext();
            var examRow = db.Exams.AsNoTracking().Single(e => e.Id == examId);
            var row = db.StudySubjects.AsNoTracking().Single(s => s.Id == examRow.StudySubjectId);
            var subject = new Domain.Aggregates.StudySubject(row.Id, row.SubjectName, row.NumberOfChapters);
            var domainSections = sections.Select(s => new Domain.Aggregates.ExamSection(s.Id, s.NumberOfQuestions,
                s.ChapterNumbers.Select(c => new Domain.Aggregates.SectionChapter(c)),
                s.ExamSectionRules.OrderBy(r => r.Id).Select(r => new Domain.Aggregates.SectionDifficulty(new Domain.Aggregates.Difficulty(r.DifficultyLevelId), r.NoOfQuestions)))).ToList();
            var exam = new Domain.Aggregates.Exam(examId, subject, domainSections);
            var difficultyIds = domainSections.SelectMany(s => s.SectionDifficulties).Select(d => d.Difficulty.Id).Distinct().ToArray();
            var chapters = domainSections.SelectMany(s => s.SectionChapters).Select(c => c.ChapterNumber).Distinct().ToArray();
            var rows = LoadQuestions(db, subject.Id, difficultyIds, chapters, Array.Empty<int>());
            var pool = rows.Select(q => new Domain.Aggregates.Question(q.Id, subject, q.ChapterNumber, new Domain.Aggregates.Difficulty(q.DifficultyLevelId))).ToList();
            var selected = new Domain.Services.ExamDomainService().BuildExamAttemptQuestions(exam, pool);
            var byId = rows.ToDictionary(q => q.Id, q => mapper.Map<QuestionsDataModel>(q));
            var result = new Dictionary<int, List<QuestionsDataModel>>();
            int offset = 0;
            foreach (var section in domainSections)
            {
                result[section.Id] = selected.Skip(offset).Take(section.NumberOfQuestions).Select(q => byId[q.Id]).ToList();
                offset += section.NumberOfQuestions;
            }
            return result;
        }

        public List<QuestionsDataModel> BuildExamAttemptQuestions(int studySubjectId, ExamSectionRulesDataModel rule, IEnumerable<int> excludedIds = null)
        {

            using var db = contexts.CreateDbContext();
            var excluded = (excludedIds ?? Enumerable.Empty<int>()).ToArray();
            var row = db.StudySubjects.AsNoTracking().Single(s => s.Id == studySubjectId);
            var subject = new Domain.Aggregates.StudySubject(row.Id, row.SubjectName, row.NumberOfChapters);
            var questions = LoadQuestions(db, studySubjectId, new[] { rule.DifficultyLevelId },
                Enumerable.Range(1, subject.NumberOfChapters).ToArray(), excluded);
            var section = new Domain.Aggregates.ExamSection(rule.SectionId, rule.NoOfQuestions,
                Enumerable.Range(1, subject.NumberOfChapters).Select(c => new Domain.Aggregates.SectionChapter(c)),
                new[] { new Domain.Aggregates.SectionDifficulty(new Domain.Aggregates.Difficulty(rule.DifficultyLevelId), rule.NoOfQuestions) });
            var selected = new Domain.Services.ExamDomainService().BuildExamAttemptQuestions(new Domain.Aggregates.Exam(0, subject, new[] { section }),
                questions.Select(q => new Domain.Aggregates.Question(q.Id, subject, q.ChapterNumber, new Domain.Aggregates.Difficulty(q.DifficultyLevelId))).ToList());
            var selectedIds = selected.Select(q => q.Id).ToHashSet();
            questions = questions.Where(q => selectedIds.Contains(q.Id)).ToList();
            return mapper.Map<List<QuestionsDataModel>>(questions);
        }
        private static List<DataRepository.DataRepositoryEntities.Questions> LoadQuestions(
            DbConext db, int subjectId, int[] difficultyIds, int[] chapters, int[] excludedIds)
        {
            // A collection Include needs stable root ordering while EF assembles answers.
            // Randomize only after materialization so one question's answers stay together.
            var rows = db.Questions.AsNoTrackingWithIdentityResolution()
                .Where(q => q.StudySubjectId == subjectId
                    && difficultyIds.Contains(q.DifficultyLevelId)
                    && chapters.Contains(q.ChapterNumber)
                    && !excludedIds.Contains(q.Id))
                .Include(q => q.QuestionAnswers)
                .OrderBy(q => q.Id)
                .ToList()
                .DistinctBy(q => q.Id)
                .ToArray();
            Random.Shared.Shuffle(rows);
            return rows.ToList();
        }
    }
}
