using Microsoft.Extensions.DependencyInjection;
using OnlineExamSystem.Application.Implmentation.Builder;
using OnlineExamSystem.Application.Interface;
using OnlineExamSystem.Application.Interface.Builder;
using OnlineExamSystem.Application.Interface.Examiner;
using OnlineExamSystem.Application.InterfaceImplmentation.Examiner;

namespace OnlineExamSystem.Application
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddBusinessLogicClasses(this IServiceCollection services)
        {
            services.AddScoped<IDifficultyLevelsService, DifficultyLevelsService>();
            services.AddScoped<IQuestionsService, QuestionsService>();
            services.AddScoped<IStudySubjectsService, StudySubjectsService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IExamSectionService, ExamSectionService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IExaminerAttemptService, ExaminerAttemptService>();
            


            return services;
        }
    }
}
