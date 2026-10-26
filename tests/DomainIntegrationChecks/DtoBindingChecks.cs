using System.Globalization;
using Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using OnlineExamSystem.Models;

static class DtoBindingChecks
{
    public static async Task RunAsync()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddControllersWithViews(o => o.ModelBinderProviders.Insert(0, new DtoModelBinderProvider()));
        using var provider = services.BuildServiceProvider();
        var metadata = provider.GetRequiredService<IModelMetadataProvider>();
        var factory = provider.GetRequiredService<IModelBinderFactory>();
        var values = new Dictionary<string, StringValues> {
            ["Id"] = "7", ["QuestionText"] = "Bound question", ["DifficultyLevelId"] = "2", ["StudySubjectId"] = "1",
            ["QuestionAnswersDTO[0].AnswerText"] = "First", ["QuestionAnswersDTO[1].AnswerText"] = "Second",
            ["QuestionAnswersDTO[1].IsCorrect"] = "true"
        };
        var info = metadata.GetMetadataForType(typeof(QuestionsDTO));
        var binder = factory.CreateBinder(new ModelBinderFactoryContext { Metadata = info, CacheToken = typeof(QuestionsDTO) });
        var action = new ActionContext(new DefaultHttpContext { RequestServices = provider }, new RouteData(), new ActionDescriptor());
        var context = DefaultModelBindingContext.CreateBindingContext(action,
            new FormValueProvider(BindingSource.Form, new FormCollection(values), CultureInfo.InvariantCulture), info, null, "");
        await binder.BindModelAsync(context);
        var model = (QuestionsDTO)context.Result.Model!;
        provider.GetRequiredService<IObjectModelValidator>().Validate(action, null, "", model);
        if (!context.ModelState.IsValid || model.Id != 7 || model.QuestionText != "Bound question" || model.QuestionAnswersDTO?.Count != 2
            || model.QuestionAnswersDTO[1].AnswerText != "Second" || !model.QuestionAnswersDTO[1].IsCorrect)
            throw new Exception("Private-setter DTO form binding lost input: " + System.Text.Json.JsonSerializer.Serialize(model)
                + "; " + string.Join("; ", context.ModelState.SelectMany(p => p.Value!.Errors.Select(e => p.Key + ": " + e.ErrorMessage))));
        Console.WriteLine("Passed MVC form binding of private DTO setters and nested answer collections.");
    }
}
