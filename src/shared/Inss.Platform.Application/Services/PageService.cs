using System.ComponentModel.DataAnnotations;
using Inss.Platform.Application.Factories;
using Inss.Platform.Application.Loaders;
using Inss.Platform.Application.Navigators;
using Inss.Platform.Application.Providers;
using Inss.Platform.Application.Validation;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Primitives;
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
    
    public async ValueTask<Page> LoadAsync(PagePath path, Dictionary<string, string?> queryParams)
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

        Page page = app.Pages.Get(path);
        
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

    public async ValueTask<Page?> ValidateAsync(Page page)
    {
        App app = await _appProvider.GetAsync("Test");
        Page currentPage = app.Pages.Get(page.Path);

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
    
    public async ValueTask<PagePath?> SaveAsync(Page page)
    {
        App appPages = await _appProvider.GetAsync("Test");
        
        // Switch page out for updated one?
        
        // Save the page
        await _appProvider.SaveAsync("Test", appPages);
        
        // Determine next page

        // Work out which page to go to next using a decider
        
        // If we have a page, set the previous page path on it to the parameter page path
        
        INextPageNavigator nextPageNavigator = _serviceProvider.GetKeyedService<INextPageNavigator>(page.Path.Value)!;// ?? _serviceProvider.GetRequiredService<INextPageNavigator>();
        return await nextPageNavigator.NavigateNextAsync(page);
    }
}