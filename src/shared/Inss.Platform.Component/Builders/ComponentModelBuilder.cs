using Inss.Platform.Application.Loaders;
using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Loading;

namespace Inss.Platform.Component.Builders;

public sealed class ComponentModelBuilder
{
    private readonly PageModelBuilder _pageModelBuilder;
    private readonly ComponentModel _component;

    internal ComponentModelBuilder(PageModelBuilder pageModelBuilder, ComponentModel component)
    {
        _pageModelBuilder = pageModelBuilder;
        _component = component;
    }

    public ComponentModel CurrentComponent => _component;

    public int ComponentIndex => _pageModelBuilder.CurrentPage.Components.IndexOf(_component);
    
    public ComponentModelBuilder WithLoader<TLoader>() where TLoader : IComponentLoader
    {
        _component.Loaders = [.._component.Loaders, new Loader { LoaderType = typeof(TLoader) }];
        return this;
    }

    public PageModelBuilder ComponentAdded()
    {
        return _pageModelBuilder;
    }
}