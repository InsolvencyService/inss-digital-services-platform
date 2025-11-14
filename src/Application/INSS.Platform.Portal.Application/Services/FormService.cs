using INSS.Platform.Portal.Application.Factories;
using INSS.Platform.Portal.Application.Resolvers;
using INSS.Platform.Portal.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace INSS.Platform.Portal.Application.Services;

public sealed class FormService : IModelService<FormModel>
{
    private readonly IFormModelFactory _formModelFactory;
    private readonly IFormStateService _formStateService;
    private readonly IUserSessionResolver _userSessionResolver;
    
    public FormService(
        IFormModelFactory formModelFactory, 
        IFormStateService  formStateService,
        IUserSessionResolver userSessionResolver)
    {
        _formModelFactory = formModelFactory;
        _formStateService = formStateService;
        _userSessionResolver = userSessionResolver;
    }
    
    public async Task<FormModel> LoadAsync(string? pageUrl)
    {
        FormModel? form = await _formStateService.GetAsync(_userSessionResolver.GetUserId()) ?? await _formModelFactory.CreateAsync();
        if (form != null)
        {
            form.PopAllNavigationHistory();
            form.AddNavigation(form.PageUrl);
            await _formStateService.SaveAsync(_userSessionResolver.GetUserId(), form);
        }
        return form!;
    }

    public Task ValidateAsync(ModelStateDictionary modelState, FormModel model)
    {
        return Task.CompletedTask;
    }

    public async Task<string> SaveAsync(string requestPath, FormModel model)
    {
        FormModel? form = await _formStateService.GetAsync(_userSessionResolver.GetUserId());
        if (form != null)
        {
            form.PopAllNavigationHistory();
            await _formStateService.SaveAsync(_userSessionResolver.GetUserId(), form);
        }
        
        // TODO: Push to an API
        
        return await Task.FromResult(requestPath);
    }
}