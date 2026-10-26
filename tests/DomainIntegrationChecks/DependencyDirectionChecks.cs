using AutoMapper;
using DataRepository;
using DataRepository.GateWay;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServicesClasseslibrary;
using ServicesClasseslibrary.Interface.DataRepository;
using ServicesClasseslibrary.Implmentation.Builder;
using ServicesClasseslibrary.Interface.Builder;
using ServicesClasseslibrary.InterfaceImplmentation.Examiner;

static class DependencyDirectionChecks
{
    public static void Run()
    {
        var application = typeof(QuestionsService).Assembly;
        var repository = typeof(DbConext).Assembly;
        if (application.GetReferencedAssemblies().Any(a => a.Name == repository.GetName().Name)
            || !repository.GetReferencedAssemblies().Any(a => a.Name == application.GetName().Name))
            throw new Exception("Repository/application dependency direction is incorrect.");
        var contracts = application.GetTypes().Where(t => t.Namespace == typeof(IQuestionsOperations).Namespace && t.IsInterface).ToList();
        if (contracts.Count != 12 || contracts.Any(t => !t.IsPublic))
            throw new Exception("Repository operation contracts must be public and owned by the service library.");

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContextFactory<DbConext>();
        services.AddDataRepository();
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        using var scope = provider.CreateScope();
        foreach (var contract in new[] { typeof(IDifficultyLevelsOperations), typeof(IQuestionsOperations), typeof(IStudySubjectsOperations),
            typeof(IExamOprations), typeof(IExamSectionsOperations), typeof(IUserOperations), typeof(IExamAttemptOprations),
            typeof(IExamSectionRuleOperations), typeof(IExaminerPerformanceOperations) })
            if (scope.ServiceProvider.GetRequiredService(contract).GetType().Assembly != repository)
                throw new Exception("Repository contract resolved outside the repository assembly.");
        if (scope.ServiceProvider.GetRequiredService<IExamAttemptQuestionBuilder>().GetType().Assembly != repository)
            throw new Exception("EF question builder must be registered by the repository.");
        scope.ServiceProvider.GetRequiredService<IMapper>();
        ActivatorUtilities.CreateInstance<QuestionsService>(scope.ServiceProvider);
        ActivatorUtilities.CreateInstance<ExamService>(scope.ServiceProvider);
        ActivatorUtilities.CreateInstance<ExamSectionService>(scope.ServiceProvider);
        ActivatorUtilities.CreateInstance<ExaminerAttemptService>(scope.ServiceProvider);
        Console.WriteLine("Passed dependency direction, public service-owned repository contracts, mapper/builder registration, and service construction.");
    }
}
