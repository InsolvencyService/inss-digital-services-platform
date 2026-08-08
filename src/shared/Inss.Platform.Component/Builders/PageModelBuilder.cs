using Inss.Platform.Application.Navigators;
using Inss.Platform.Application.Validators;
using Inss.Platform.Component.Extensions;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Exceptions;
using Inss.Platform.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Inss.Platform.Component.Builders;

public sealed class PageModelBuilder
{
    private readonly PageModel _page;

    private PageModelBuilder(string title, PagePath path, string? submitButton, bool displayFullWidth)
    {
        _page = new PageModel { Title = title, Path = path, SubmitButton = submitButton, DisplayFullWidth = displayFullWidth };
    }

    public PageModel CurrentPage => _page;
    
    public static PageModelBuilder For(string title, PagePath path, string? submitButton = null, bool displayFullWidth = false)
    {
        return new PageModelBuilder(title, path, submitButton, displayFullWidth);
    }

    public PageModelBuilder NextPageIs(PagePath nextPage)
    {
        _page.NextPages.Add(nextPage);
        return this;
    }
    
    public PageModelBuilder PreviousPageIs(PagePath previousPage)
    {
        _page.PreviousPage = previousPage;
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

    public PageModelBuilder WithPageValidator<TValidator>() where TValidator : IPageValidator
    {
        _page.PageValidator = typeof(TValidator);
        return this;
    }
    
    public ComponentId GetNextComponentId()
    {
        return $"{_page.Path}/component/{_page.Components.Count}"; 
    }
    
    public PageModel Build(IServiceCollection services)
    {
        _page.Register(services);
        return _page;
    }
}