using Inss.Platform.Domain;
using Inss.Platform.Domain.Components;

namespace Inss.Platform.Application.Loaders;

public sealed class LoaderContext
{
    public LoaderContext(AppModel app, PageModel page, ComponentModel component, QueryParamList queryParams)
    {
        App = app;
        Page = page;
        Component = component;
        QueryParams = queryParams;
    }
    
    public AppModel App { get; }
    
    public PageModel Page { get; }
    
    public ComponentModel Component { get; }
    
    public QueryParamList QueryParams { get; }
}