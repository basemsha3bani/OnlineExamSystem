using Microsoft.Extensions.DependencyInjection;
using ServicesClasseslibrary.Implmentation.Builder;
using ServicesClasseslibrary.Interface;
using ServicesClasseslibrary.Interface.DataModel;
using ServicesClasseslibrary.WorkerServices.Analytics;
using ServicesClasseslibrary.WorkerServices.InterfaceImplementation.Examiner;
using ServicesClasseslibrary.WorkerServices.Logging;

namespace ServicesClasseslibrary;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddServiceClasses(this IServiceCollection services)
    {
        services.AddScoped<IDifficultyLevelsService, DifficultyLevelsService>();
        services.AddScoped<IQuestionsService, QuestionsService>();
        services.AddScoped<IStudySubjectsService, StudySubjectsService>();
        services.AddScoped<IExamService, ExamService>();
        services.AddScoped<IExamSectionService, ExamSectionService>();
        services.AddScoped<IUserService, UserService>();
        services.AddAnalyticsService();
        services.AddExaminerServices();
        services.AddLoggingService();
        return services;
    }
}
