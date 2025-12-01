using INSS.Platform.Portal.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace INSS.Platform.Portal.Web.Components.Binding;

public sealed class BaseModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(BaseModel))
        {
            return new BinderTypeModelBinder(typeof(BaseModelBinder));
        }

        return null!;
    }
}