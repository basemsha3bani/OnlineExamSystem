using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OnlineExamSystem.Models;

// Form input is transport data. Bind through DTO factories without exposing
// setters on domain types or teaching MVC how to construct entities.
public sealed class DtoModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType.Namespace != "Domain.DTOs") return null;
        var factory = context.Metadata.ModelType.GetMethod("Define", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
        if (factory == null) return null;
        return new DtoModelBinder(factory, context.Metadata.Properties
            .Where(p => context.Metadata.ModelType.GetMethod("Change" + p.PropertyName) != null)
            .Select(p => (p, context.CreateBinder(p))).ToList());
    }

    private sealed class DtoModelBinder(MethodInfo factory, List<(ModelMetadata Metadata, IModelBinder Binder)> properties) : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext context)
        {
            if (!context.IsTopLevelObject && !context.ValueProvider.ContainsPrefix(context.ModelName))
            {
                context.Result = ModelBindingResult.Failed();
                return;
            }
            var value = factory.Invoke(null, factory.GetParameters().Select(p => p.DefaultValue).ToArray());
            foreach (var (metadata, binder) in properties)
            {
                var name = ModelNames.CreatePropertyModelName(context.ModelName, metadata.PropertyName);
                using (context.EnterNestedScope(metadata, metadata.PropertyName, name, null))
                {
                    await binder.BindModelAsync(context);
                    if (context.Result.IsModelSet)
                        value.GetType().GetMethod("Change" + metadata.PropertyName).Invoke(value, new[] { context.Result.Model });
                }
            }
            context.Result = ModelBindingResult.Success(value);
        }
    }
}
