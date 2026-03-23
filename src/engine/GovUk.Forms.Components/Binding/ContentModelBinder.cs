using GovUk.Forms.Components.Extensions;
using GovUk.Forms.Components.Resolvers;
using GovUk.Forms.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GovUk.Forms.Components.Binding;

public sealed class ContentModelBinder : IModelBinder
{
    private readonly ITypeNameResolver _typeNameResolver;

    public ContentModelBinder(ITypeNameResolver typeNameResolver)
    {
        _typeNameResolver = typeNameResolver;
    }
    
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ContentModel instance = bindingContext.HttpContext.Request.Form.HydrateContentModel(_typeNameResolver);
        bindingContext.Result = ModelBindingResult.Success(instance);
        return Task.CompletedTask;
    }
}