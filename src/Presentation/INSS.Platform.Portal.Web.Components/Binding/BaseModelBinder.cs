using System.Text.Json;
using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Web.Components.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace INSS.Platform.Portal.Web.Components.Binding;

public sealed class BaseModelBinder : IModelBinder
{
    private readonly IModelTypeService _modelTypeService;

    public BaseModelBinder(IModelTypeService modelTypeService)
    {
        _modelTypeService = modelTypeService;
    }
    
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        string modelKind = bindingContext.ValueProvider.GetValue("Kind").FirstValue 
            ?? throw new InvalidOperationException("Unable to retrieve the Kind from the form.");

        Type modelType = _modelTypeService.GetModelType(modelKind);
        
        Dictionary<string, object?> dict = bindingContext.HttpContext.Request.Form.ToObjectDictionary(modelType);
        
        string json = JsonSerializer.Serialize(dict);
                
        object? instance = JsonSerializer.Deserialize(json, modelType);
                
        if (instance is not null)
        {
            bindingContext.Result = ModelBindingResult.Success(instance);
            return Task.CompletedTask;
        }
        
        bindingContext.Result = ModelBindingResult.Failed();
        
        return Task.CompletedTask;
    }
}