using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.Components.Binding;

public sealed class ContentBinderFactory : IContentBinderFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ContentBinderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IContentBinder Create(string typeName)
    {
        return _serviceProvider.GetKeyedService<IContentBinder>(typeName) ?? _serviceProvider.GetRequiredService<IContentBinder>();
    }
}