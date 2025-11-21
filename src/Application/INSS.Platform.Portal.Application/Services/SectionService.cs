using INSS.Platform.Portal.Application.Resolvers;
using INSS.Platform.Portal.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace INSS.Platform.Portal.Application.Services;

public sealed class SectionService : IModelService<SectionModel>
{
    private readonly IFormStateService _formStateService;
    private readonly IUserSessionResolver _userSessionResolver;

    public SectionService(IFormStateService formStateService, IUserSessionResolver userSessionResolver)
    {
        _formStateService = formStateService;
        _userSessionResolver = userSessionResolver;
    }
    
    public async Task<SectionModel> LoadAsync(string? pageUrl)
    {
        FormModel form = await _formStateService.GetAsync(_userSessionResolver.GetUserId());
        SectionModel section = form.FindSection(pageUrl!);
        return section;
    }

    public Task ValidateAsync(ModelStateDictionary modelState, SectionModel model)
    {
        return Task.CompletedTask;
    }

    public async Task<string> SaveAsync(string requestPath, SectionModel model)
    {
        FormModel form = await _formStateService.GetAsync(_userSessionResolver.GetUserId());
        SectionModel section = form.FindSection(requestPath) ?? throw new InvalidOperationException($"Section not found for path: {requestPath}");
        section.IsComplete = true;
        await _formStateService.SaveAsync(_userSessionResolver.GetUserId(), form);
        return form.PageUrl;
    }

    public Task<string> GetPageUrlAsync(string? pageUrl, string id)
    {
        throw new NotImplementedException();
    }
}