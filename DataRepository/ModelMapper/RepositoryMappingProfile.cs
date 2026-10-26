using Domain.Entities;
using Domain.Aggregates;
using AutoMapper;
using Domain.DTOs;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
using System.Collections.Generic;
namespace DataRepository.ModelMapper;
public class RepositoryMappingProfile : Profile
{
    public RepositoryMappingProfile()
    {
        CreateMap<DifficultyLevels, DifficultyLevelsDTO>().ConvertUsing((s, d, c) => s == null ? null : DifficultyLevelsDTO.Define(Id: s.Id, DifficultyLevelName: s.DifficultyLevelName));
        CreateMap<DifficultyLevelsDTO, DifficultyLevels>().ConvertUsing((s, d, c) =>
        {
            if (s == null) return null;
            if (d == null) return DifficultyLevels.Define(Id: s.Id, DifficultyLevelName: s.DifficultyLevelName);
            _ = DifficultyLevels.Define(Id: s.Id, DifficultyLevelName: s.DifficultyLevelName);
            d.ChangeId(s.Id);
            d.ChangeDifficultyLevelName(s.DifficultyLevelName);
            return d;
        });
        CreateMap<StudySubject, StudySubjectDTO>().ConvertUsing((s, d, c) => s == null ? null : StudySubjectDTO.Define(Id: s.Id, NumberOfChapters: s.NumberOfChapters, SubjectName: s.SubjectName));
        CreateMap<StudySubjectDTO, StudySubject>().ConvertUsing((s, d, c) =>
        {
            if (s == null) return null;
            if (d == null) return StudySubject.Define(Id: s.Id, NumberOfChapters: s.NumberOfChapters, SubjectName: s.SubjectName);
            _ = StudySubject.Define(Id: s.Id, NumberOfChapters: s.NumberOfChapters, SubjectName: s.SubjectName);
            d.ChangeId(s.Id);
            d.ChangeNumberOfChapters(s.NumberOfChapters);
            d.ChangeSubjectName(s.SubjectName);
            return d;
        });
        CreateMap<QuestionAnswers, QuestionAnswersDTO>().ConvertUsing((s, d, c) => s == null ? null : QuestionAnswersDTO.Define(Id: s.Id, QuestionId: s.QuestionId, AnswerText: s.AnswerText, IsCorrect: s.IsCorrect, radioButtonDisplay: "rbIsCorrect"));
        CreateMap<QuestionAnswersDTO, QuestionAnswers>().ConvertUsing((s, d, c) =>
        {
            if (s == null) return null;
            if (d == null) return QuestionAnswers.Define(Id: s.Id, QuestionId: s.QuestionId, AnswerText: s.AnswerText, IsCorrect: s.IsCorrect);
            _ = QuestionAnswers.Define(Id: s.Id, QuestionId: s.QuestionId, AnswerText: s.AnswerText, IsCorrect: s.IsCorrect);
            d.ChangeId(s.Id);
            d.ChangeQuestionId(s.QuestionId);
            d.ChangeAnswerText(s.AnswerText);
            d.ChangeIsCorrect(s.IsCorrect);
            return d;
        });
        CreateMap<Questions, QuestionsDTO>().ConvertUsing((s, d, c) => s == null ? null : QuestionsDTO.Define(Id: s.Id, ChapterNumber: s.ChapterNumber, QuestionText: s.QuestionText, DifficultyLevelId: s.DifficultyLevelId, StudySubjectId: s.StudySubjectId, QuestionAnswersDTO: c.Mapper.Map<List<QuestionAnswersDTO>>(s.QuestionAnswers)));
        CreateMap<QuestionsDTO, Questions>().ConvertUsing((s, d, c) =>
        {
            if (s == null) return null;
            if (d == null) return Questions.Define(Id: s.Id, ChapterNumber: s.ChapterNumber, QuestionText: s.QuestionText, DifficultyLevelId: s.DifficultyLevelId, StudySubjectId: s.StudySubjectId);
            _ = Questions.Define(Id: s.Id, ChapterNumber: s.ChapterNumber, QuestionText: s.QuestionText, DifficultyLevelId: s.DifficultyLevelId, StudySubjectId: s.StudySubjectId);
            d.ChangeId(s.Id);
            d.ChangeChapterNumber(s.ChapterNumber);
            d.ChangeQuestionText(s.QuestionText);
            d.ChangeDifficultyLevelId(s.DifficultyLevelId);
            d.ChangeStudySubjectId(s.StudySubjectId);
            return d;
        });
        CreateMap<Exam, ExamDTO>().ConvertUsing((s, d, c) => s == null ? null : ExamDTO.Define(Id: s.Id, Title: s.Title, StudySubjectId: s.StudySubjectId, StudySubjectName: s.StudySubject == null ? null : s.StudySubject.SubjectName, TotalMarks: s.TotalMarks, Sections: c.Mapper.Map<List<ExamSectionsDTO>>(s.Sections)));
        CreateMap<ExamDTO, Exam>().ConvertUsing((s, d, c) =>
        {
            if (s == null) return null;
            if (d == null) return Exam.Define(Id: s.Id, Title: s.Title, StudySubjectId: s.StudySubjectId, TotalMarks: s.TotalMarks);
            _ = Exam.Define(Id: s.Id, Title: s.Title, StudySubjectId: s.StudySubjectId, TotalMarks: s.TotalMarks);
            d.ChangeId(s.Id);
            d.ChangeTitle(s.Title);
            d.ChangeStudySubjectId(s.StudySubjectId);
            d.ChangeTotalMarks(s.TotalMarks);
            return d;
        });
        CreateMap<ExamSections, ExamSectionsDTO>().ConvertUsing((s, d, c) => s == null ? null : ExamSectionsDTO.Define(Id: s.Id, NumberOfQuestions: s.NumberOfQuestions, ChapterNumbers: s.ChapterNumbers, ExamId: s.ExamId, SectionName: s.SectionName, exam: s.Exam == null ? null : ExamDTO.Define(Id: s.Exam.Id, Title: s.Exam.Title, StudySubjectId: s.Exam.StudySubjectId), ExamSectionRules: c.Mapper.Map<List<ExamSectionRulesDTO>>(s.examSectionRules)));
        CreateMap<ExamSectionsDTO, ExamSections>().ConvertUsing((s, d, c) =>
        {
            if (s == null) return null;
            if (d == null) return ExamSections.Define(Id: s.Id, NumberOfQuestions: s.NumberOfQuestions, ChapterNumbers: s.ChapterNumbers, ExamId: s.ExamId, SectionName: s.SectionName);
            _ = ExamSections.Define(Id: s.Id, NumberOfQuestions: s.NumberOfQuestions, ChapterNumbers: s.ChapterNumbers, ExamId: s.ExamId, SectionName: s.SectionName);
            d.ChangeId(s.Id);
            d.ChangeNumberOfQuestions(s.NumberOfQuestions);
            d.ChangeChapterNumbers(s.ChapterNumbers);
            d.ChangeExamId(s.ExamId);
            d.ChangeSectionName(s.SectionName);
            return d;
        });
        CreateMap<ExamSectionRules, ExamSectionRulesDTO>().ConvertUsing((s, d, c) => s == null ? null : ExamSectionRulesDTO.Define(Id: s.Id, SectionId: s.SectionId, DifficultyLevelId: s.DifficultyLevelId, DifficultyLevel: s.difficultyLevel == null ? null : s.difficultyLevel.DifficultyLevelName, subjectName: s.section?.Exam?.StudySubject?.SubjectName, NoOfQuestions: s.NoOfQuestions));
        CreateMap<ExamSectionRulesDTO, ExamSectionRules>().ConvertUsing((s, d, c) =>
        {
            if (s == null) return null;
            if (d == null) return ExamSectionRules.Define(Id: s.Id, SectionId: s.SectionId, DifficultyLevelId: s.DifficultyLevelId, NoOfQuestions: s.NoOfQuestions);
            _ = ExamSectionRules.Define(Id: s.Id, SectionId: s.SectionId, DifficultyLevelId: s.DifficultyLevelId, NoOfQuestions: s.NoOfQuestions);
            d.ChangeId(s.Id);
            d.ChangeSectionId(s.SectionId);
            d.ChangeDifficultyLevelId(s.DifficultyLevelId);
            d.ChangeNoOfQuestions(s.NoOfQuestions);
            return d;
        });
        CreateMap<User, LoginDTO>().ConvertUsing((s, d, c) => s == null ? null : LoginDTO.Define(Id: s.Id, Username: s.Username, Password: null, Role: s.Role));
        CreateMap<LoginDTO, User>().ConvertUsing((s, d, c) =>
        {
            if (s == null) return null;
            if (d == null) return User.Define(Id: s.Id, Username: s.Username, PasswordHash: s.Password, Role: s.Role);
            _ = User.Define(Id: s.Id, Username: s.Username, PasswordHash: s.Password, Role: s.Role);
            d.ChangeId(s.Id);
            d.ChangeUsername(s.Username);
            d.ChangePasswordHash(s.Password);
            d.ChangeRole(s.Role);
            return d;
        });
        CreateMap<ExamAttempt, ExamAttemptDTO>().ConvertUsing((s, d, c) => s == null ? null : ExamAttemptDTO.Define(Id: s.Id, ExamId: s.ExamId, UserId: s.UserId, ExamTitle: s.ExamTitle, SnapshotJson: s.SnapshotJson, Status: s.Status, StartedAt: s.StartedAt, SubmittedAt: s.SubmittedAt, EvaluatedAt: s.EvaluatedAt, TotalQuestions: s.TotalQuestions, CorrectQuestions: s.CorrectQuestions, Score: s.Score, Version: s.Version));
        CreateMap<ExamAttemptDTO, ExamAttempt>().ConvertUsing((s, d, c) =>
        {
            if (s == null) return null;
            if (d == null) return ExamAttempt.Define(Id: s.Id, ExamId: s.ExamId, UserId: s.UserId, ExamTitle: s.ExamTitle, SnapshotJson: s.SnapshotJson, Status: s.Status, StartedAt: s.StartedAt, SubmittedAt: s.SubmittedAt, EvaluatedAt: s.EvaluatedAt, TotalQuestions: s.TotalQuestions, CorrectQuestions: s.CorrectQuestions, Score: s.Score, Version: s.Version);
            _ = ExamAttempt.Define(Id: s.Id, ExamId: s.ExamId, UserId: s.UserId, ExamTitle: s.ExamTitle, SnapshotJson: s.SnapshotJson, Status: s.Status, StartedAt: s.StartedAt, SubmittedAt: s.SubmittedAt, EvaluatedAt: s.EvaluatedAt, TotalQuestions: s.TotalQuestions, CorrectQuestions: s.CorrectQuestions, Score: s.Score, Version: s.Version);
            d.ChangeId(s.Id);
            d.ChangeExamId(s.ExamId);
            d.ChangeUserId(s.UserId);
            d.ChangeExamTitle(s.ExamTitle);
            d.ChangeSnapshotJson(s.SnapshotJson);
            d.ChangeStatus(s.Status);
            d.ChangeStartedAt(s.StartedAt);
            d.ChangeSubmittedAt(s.SubmittedAt);
            d.ChangeEvaluatedAt(s.EvaluatedAt);
            d.ChangeTotalQuestions(s.TotalQuestions);
            d.ChangeCorrectQuestions(s.CorrectQuestions);
            d.ChangeScore(s.Score);
            d.ChangeVersion(s.Version);
            return d;
        });
    }
}
