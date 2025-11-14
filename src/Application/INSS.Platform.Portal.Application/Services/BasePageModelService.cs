using INSS.Platform.Portal.Application.Resolvers;
using INSS.Platform.Portal.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace INSS.Platform.Portal.Application.Services;

public abstract class BasePageModelService<TPageModel> : IModelService<TPageModel> where TPageModel : PageModel
{
    private readonly IFormStateService _formStateService;
    private readonly IJourneyService _journeyService;
    private readonly IUserSessionResolver _userSessionResolver;

    protected BasePageModelService(
        IFormStateService  formStateService, 
        IJourneyService  journeyService,
        IUserSessionResolver userSessionResolver)
    {
        _formStateService = formStateService;
        _journeyService = journeyService;
        _userSessionResolver = userSessionResolver;
    }

    public async Task<TPageModel> LoadAsync(string? pageUrl)
    {
        FormModel form = await _formStateService.GetAsync(_userSessionResolver.GetUserId());
        TPageModel page = form.FindPage<TPageModel>(pageUrl!);
        _journeyService.TransitionPrevious(form, page);
        return page;
    }

    public async Task ValidateAsync(ModelStateDictionary modelState, TPageModel model)
    {
        FormModel form = await _formStateService.GetAsync(_userSessionResolver.GetUserId());

        if (!modelState.IsValid)
        {
            model.PreviousPageUrl = form.NavigationHistory.LastOrDefault() ?? string.Empty;
            return;
        }

        await ValidateAdditionalAsync(modelState, model);

        if (!modelState.IsValid)
        {
            model.PreviousPageUrl = form.NavigationHistory.LastOrDefault() ?? string.Empty;
        }
    }

    public async Task<string> SaveAsync(string requestPath, TPageModel model)
    {
        FormModel form = await _formStateService.GetAsync(_userSessionResolver.GetUserId());
        TPageModel page = form.FindPage<TPageModel>(requestPath);
        form.AddNavigation(page.PageUrl);

        CopySourceToTargetModel(model, page);

        await _formStateService.SaveAsync(_userSessionResolver.GetUserId(), form);
        
        _journeyService.TransitionNext(form, page);
        return page.NextPageUrl;
    }

    protected virtual void CopySourceToTargetModel(TPageModel sourceModel, TPageModel targetModel)
    {
        // Override
    }

    protected virtual Task ValidateAdditionalAsync(ModelStateDictionary modelState, TPageModel model)
    {
        // Override if required
        return Task.CompletedTask;
    }
}