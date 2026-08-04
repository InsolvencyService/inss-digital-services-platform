using System.ComponentModel.DataAnnotations;
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
        App app;
        
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
        
        foreach (Component component in page.Components)
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
        App app = await _appProvider.GetAsync("Test");
        PageModel currentPage = app.Pages.Get(page.Path);

        foreach (Component currentComponent in currentPage.Components)
        {
            Component component = page.Components.Get(currentComponent.Id);
            component.CopyTo(currentComponent);
        }

        List<PageValidationError> pageValidationErrorList = [];
        
        foreach (Component component in currentPage.Components)
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
        App app = await _appProvider.GetAsync("Test");
        
        try
        {
            PageModel currentPage = app.Pages.Get(page.Path);

            foreach (Component currentComponent in currentPage.Components)
            {
                Component component = page.Components.Get(currentComponent.Id);
                component.CopyTo(currentComponent);
            }
        
            INextPageNavigator nextPageNavigator = _serviceProvider.GetKeyedService<INextPageNavigator>(currentPage.Path.Value)!;
            PagePath? nextPagePath = await nextPageNavigator.NavigateNextAsync(currentPage);

            if (nextPagePath is not null)
            {
                PageModel nextPage = app.Pages.Get(nextPagePath);

                if (nextPage.PreviousPage is not null)
                {
                    nextPage.PreviousPage = currentPage.Path;
                }
            }
        
            return nextPagePath;
        }
        finally
        {
            await _appProvider.SaveAsync("Test", app);
        }
    }
}