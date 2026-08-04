using System.ComponentModel.DataAnnotations;
using System.Net;
using Inss.Platform.Application.Factories;
using Inss.Platform.Application.Loaders;
using Inss.Platform.Application.Navigators;
using Inss.Platform.Application.Providers;
using Inss.Platform.Application.Validation;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Primitives;
using Inss.Platform.Domain.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Inss.Platform.Application.Services;

public sealed class PageService : IPageService
{
    private readonly IAppProvider _appProvider;
    private readonly IAppFactory _appFactory;
    private readonly IServiceProvider _serviceProvider;

    public PageService(IAppProvider appProvider, IAppFactory appFactory, IServiceProvider serviceProvider)
    {
        _appProvider = appProvider;
        _appFactory = appFactory;
        _serviceProvider = serviceProvider;
    }
    
    public async ValueTask<PageModel> LoadAsync(PagePath path, Dictionary<string, string?> queryParams)
    {
        AppModel app;
        
        if (!await _appProvider.ExistsAsync("Test"))
        {
            app = await _appFactory.CreateAsync("Test");
            await _appProvider.SaveAsync("Test", app);
        }
        else
        {
            app = await _appProvider.GetAsync("Test");
        }

        PageModel page = app.Pages.Get(path);
        
        foreach (ComponentModel component in page.Components)
        {
            IEnumerable<IComponentLoader> loaders = _serviceProvider.GetKeyedServices<IComponentLoader>(component.Id.Value);
            
            foreach (IComponentLoader loader in loaders)
            {
                await loader.LoadAsync(component);
            }
        }

        return page;
    }

    public async ValueTask<PageModel?> ValidateAsync(PageModel page)
    {
        AppModel app = await _appProvider.GetAsync("Test");
        PageModel currentPage = app.Pages.Get(page.Path);

        foreach (ComponentModel currentComponent in currentPage.Components)
        {
            ComponentModel component = page.Components.Get(currentComponent.Id);
            component.CopyTo(currentComponent);
        }

        List<PageValidationError> pageValidationErrorList = [];
        
        foreach (ComponentModel component in currentPage.Components)
        {
            IEnumerable<IComponentValidator> validators = _serviceProvider.GetKeyedServices<IComponentValidator>(component.Id.Value);
            
            foreach (IComponentValidator validator in validators)
            {
                ValidationResult[] componentValidations = await validator.ValidateAsync(component);
                
                pageValidationErrorList.AddRange(componentValidations.Select(vr => new PageValidationError
                {
                    Properties = vr.MemberNames.ToArray(),
                    Message = vr.ErrorMessage ?? string.Empty
                }));
            }
        }

        if (pageValidationErrorList.Count > 0)
        {
            currentPage.PageValidationInfo = new PageValidationInfo { Errors = pageValidationErrorList.ToArray() };
            return currentPage;
        }

        return null;
    }
    
    public async ValueTask<PagePath?> SaveAsync(PageModel page)
    {
        AppModel app = await _appProvider.GetAsync("Test");
        
        try
        {
            PageModel currentPage = app.Pages.Get(page.Path);
            QueryParamList queryParams = [];
            
            foreach (ComponentModel currentComponent in currentPage.Components)
            {
                ComponentModel component = page.Components.Get(currentComponent.Id);
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
                PageModel nextPage = app.Pages.Get(nextPagePath);
                nextPage.PreviousPage ??= currentPage.Path;
            }
        
            return FormatRedirectPath(nextPagePath, queryParams);
        }
        finally
        {
            await _appProvider.SaveAsync("Test", app);
        }
    }
    
    private static PagePath? FormatRedirectPath(PagePath? path, QueryParamList queryParams)
    {
        if (path is null || queryParams.Count == 0)
        {
            return path;
        }
        
        string query = string.Join("&", queryParams
            .Where(kvp => !string.IsNullOrEmpty(kvp.Key) && kvp.Value != null)
            .Select(kvp => $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));
        
        return string.IsNullOrWhiteSpace(query) ? path : $"{path}?{query}";
    }
}