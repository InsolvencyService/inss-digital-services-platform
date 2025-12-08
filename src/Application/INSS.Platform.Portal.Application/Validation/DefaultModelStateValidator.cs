using INSS.Platform.Portal.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace INSS.Platform.Portal.Application.Validation;

public sealed class DefaultModelStateValidator : IModelStateValidator<BaseModel>
{
    public Task ValidateAsync(ModelStateDictionary modelState, BaseModel model)
    {
        return Task.CompletedTask;
    }
}