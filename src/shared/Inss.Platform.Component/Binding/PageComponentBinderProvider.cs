using Inss.Platform.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Inss.Platform.Component.Binding;

public sealed class PageComponentBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(Page))
        {
            return new BinderTypeModelBinder(typeof(PageComponentBinder));
        }

        return null!;
    }
}