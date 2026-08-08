using Inss.Platform.Application.Extensions;
using Inss.Platform.Application.Navigators;
using Inss.Platform.Application.Providers; 
using Inss.Platform.Domain;
using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Inss.Platform.Application.Services;

public sealed class PageService : IPageService
{
    private readonly IAppProvider _appProvider;
    private readonly IServiceProvider _serviceProvider;

    public PageService(IAppProvider appProvider, IServiceProvider serviceProvider)
    {
        _appProvider = appProvider;
        _serviceProvider = serviceProvider;
    }
    
    public async ValueTask<PageModel> LoadAsync(PagePath path, QueryParamList queryParams)
    {
        AppModel app = await _appProvider.GetAsync();
        PageModel page = app.Pages.GetPage(path);
        await page.LoadAsync(app, queryParams, _serviceProvider);
        return page;
    }

    public async ValueTask<PageModel?> ValidateAsync(PageModel page)
    {
        AppModel app = await _appProvider.GetAsync();
        PageModel currentPage = app.Pages.GetPage(page.Path);
        page.CopyComponentValuesTo(currentPage);
        await currentPage.ValidateAsync(_serviceProvider);
        return currentPage.PageValidationInfo is not null ? currentPage : null;
    }
    
    public async ValueTask<PagePath?> SaveAsync(PageModel page)
    {
        AppModel app = await _appProvider.GetAsync();
        
        // TODO: Refactor as above
        try
        {
            PageModel currentPage = app.Pages.GetPage(page.Path);
            QueryParamList queryParams = [];
            
            foreach (ComponentModel currentComponent in currentPage.Components.Where(c => c.ComponentType == ComponentTypes.Bindable))
            {
                ComponentModel component = page.Components.GetComponent(currentComponent.Id);
                component.CopyTo(currentComponent);

                if (component is IQueryParamComponent queryParamComponent)
                {
                    queryParamComponent.Append(queryParams);
                }
            }
            
            INextPageNavigator nextPageNavigator = _serviceProvider.GetKeyedService<INextPageNavigator>(currentPage.Path.Value)
                                                   ?? _serviceProvider.GetRequiredService<INextPageNavigator>();
            PagePath? nextPagePath = await nextPageNavigator.NavigateNextAsync(currentPage);

            if (nextPagePath is not null)
            {
                PageModel nextPage = app.Pages.GetPage(nextPagePath);
                nextPage.PreviousPage ??= currentPage.Path;
            }
        
            return FormatRedirectPath(nextPagePath, queryParams);
        }
        finally
        {
            await _appProvider.SaveAsync(app);
        }
    }
    
    private static PagePath? FormatRedirectPath(PagePath? path, QueryParamList queryParams)
    {
        if (path is null || queryParams.Count == 0)
        {
            return path;
        }

        string? query = queryParams.BuildQueryParams();
        return string.IsNullOrWhiteSpace(query) ? path : $"{path}{query}";
    }
}