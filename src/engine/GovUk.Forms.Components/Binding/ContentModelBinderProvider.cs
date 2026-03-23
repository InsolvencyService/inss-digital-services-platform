using GovUk.Forms.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace GovUk.Forms.Components.Binding;

public sealed class ContentModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(ContentModel))
        {
            return new BinderTypeModelBinder(typeof(ContentModelBinder));
        }

        return null!;
    }
}