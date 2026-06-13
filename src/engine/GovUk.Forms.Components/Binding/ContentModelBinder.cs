using GovUk.Forms.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GovUk.Forms.Components.Binding;

public sealed class ContentModelBinder : IModelBinder
{
    private readonly IContentBinderFactory _contentBinderFactory;

    public ContentModelBinder(IContentBinderFactory contentBinderFactory)
    {
        _contentBinderFactory = contentBinderFactory;
    }
    
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        string typeName = bindingContext.HttpContext.Request.Form["TypeName"].ToString();
        IContentBinder contentBinder = _contentBinderFactory.Create(typeName);
        ContentModel instance = contentBinder.BindAndReturnModel(typeName, bindingContext.HttpContext.Request.Form);
        bindingContext.Result = ModelBindingResult.Success(instance);
        return Task.CompletedTask;
    }
}