using Inss.Platform.Application.Navigators;
using Inss.Platform.Component.Extensions;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Exceptions;
using Inss.Platform.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Inss.Platform.Component.Builders;

public sealed class PageModelBuilder
{
    private readonly PageModel _page;

    private PageModelBuilder(string title, PagePath path, Content? submitButton)
    {
        _page = new() { Title = title, Path = path, SubmitButton = submitButton };
    }

    public PageModel CurrentPage => _page;
    
    public static PageModelBuilder For(string title, PagePath path, Content? submitButton = null)
    {
        return new PageModelBuilder(title, path, submitButton);
    }

    public PageModelBuilder NextPagesAre(params PagePath[] nextPages)
    {
        _page.NextPages.AddRange(nextPages);
        return this;
    }
    
    public PageModelBuilder NextPagesAre<TNextPageNavigator>(params PagePath[] nextPages) where TNextPageNavigator : INextPageNavigator
    {
        if (nextPages.Length < 2)
        {
            throw new ComponentException("You must provided at least 2 next pages to use a custom navigator.");
        }
        
        _page.NextPageNavigator = typeof(TNextPageNavigator);
        _page.NextPages.AddRange(nextPages);
        return this;
    }
    
    public PageModel Build(IServiceCollection services)
    {
        _page.Register(services);
        return _page;
    }
}