using AutoMapper;
using DataModel;
using DataRepository.DataRepositoryEntities;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;

namespace DataRepository.ModelMapper
{
    public class RepositoryMappingProfile : Profile
    {
        public RepositoryMappingProfile()
        {
            CreateMap<DifficultyLevels, DifficultyLevelsDataModel>();
            CreateMap<DifficultyLevelsDataModel, DifficultyLevels>();
            CreateMap<StudySubject, StudySubjectDataModel>();
            CreateMap<StudySubjectDataModel, StudySubject>();
            CreateMap<QuestionAnswers, QuestionAnswersDataModel>()
                .ForMember(d => d.radioButtonDisplay, o => o.MapFrom(s =>"rbIsCorrect"));
            CreateMap<QuestionAnswersDataModel, QuestionAnswers>()
                .ForMember(d => d.Question, o => o.Ignore());
            CreateMap<Questions, QuestionsDataModel>()
                .ForMember(d => d.QuestionAnswersDataModel, o => o.MapFrom(s => s.QuestionAnswers));
            CreateMap<QuestionsDataModel, Questions>()
                .ForMember(d => d.DifficultyLevel, o => o.Ignore())
                // Repositories reconcile tracked children explicitly, preserving their identity.
                .ForMember(d => d.QuestionAnswers, o => o.Ignore());
            CreateMap<Exams, ExamDataModel>()
                .ForMember(d => d.StudySubjectName, o => o.MapFrom(s => s.StudySubject.SubjectName));
            CreateMap<ExamDataModel, Exams>()
                .ForMember(d => d.StudySubject, o => o.Ignore())
                .ForMember(d => d.Sections, o => o.Ignore());
            CreateMap<ExamSections, ExamSectionsDataModel>()
                .ForMember(d => d.ExamSectionRulesDataModel, o => o.MapFrom(s => s.examSectionRules))
                .ForMember(d => d.exam, o => o.MapFrom(s => s.Exam == null ? null : new ExamDataModel {
                    Id = s.Exam.Id, StudySubjectId = s.Exam.StudySubjectId }));
            CreateMap<ExamSectionsDataModel, ExamSections>()
                .ForMember(d => d.Exam, o => o.Ignore())
                .ForMember(d => d.examSectionRules, o => o.Ignore());
            CreateMap<ExamSectionRules, ExamSectionRulesDataModel>()
                .ForMember(d => d.DifficultyLevel, o => o.MapFrom(s => s.difficultyLevel.DifficultyLevelName))
                .ForMember(d => d.subjectName, o => o.MapFrom(s => s.section.Exam.StudySubject.SubjectName));
            CreateMap<ExamSectionRulesDataModel, ExamSectionRules>()
                .ForMember(d => d.section, o => o.Ignore())
                .ForMember(d => d.difficultyLevel, o => o.Ignore());
            CreateMap<LoginModel, User>()
                .ForMember(d => d.PasswordHash, o => o.MapFrom(s => s.Password));
            CreateMap<User, LoginModel>()
                .ForMember(d => d.Password, o => o.Ignore());
        }
    }
}
