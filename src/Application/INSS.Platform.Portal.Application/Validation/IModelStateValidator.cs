using INSS.Platform.Portal.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace INSS.Platform.Portal.Application.Validation;

public interface IModelStateValidator
{
    Task ValidateAsync(ModelStateDictionary modelState, BaseModel model);
}

public interface IModelStateValidator<in TModel> : IModelStateValidator where TModel : BaseModel;