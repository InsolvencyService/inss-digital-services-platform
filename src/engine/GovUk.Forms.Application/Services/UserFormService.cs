using GovUk.Forms.Application.Factories;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.Services;

public sealed class UserFormService : IUserFormService
{
    private readonly IUserSessionProvider _userSessionProvider;
    private readonly IFormStorageProvider _formStorageProvider;
    private readonly IFormFactory _formFactory;
    private readonly ISubmitFormService _submitFormService;

    public UserFormService(
        IUserSessionProvider userSessionProvider, 
        IFormStorageProvider formStorageProvider, 
        IFormFactory formFactory,
        ISubmitFormService submitFormService)
    {
        _userSessionProvider = userSessionProvider;
        _formStorageProvider = formStorageProvider;
        _formFactory = formFactory;
        _submitFormService = submitFormService;
    }

    public async Task<FormModel> GetAsync(ContentPath path)
    {
        ContentPath formPath = path.GetRoot();
        string userSessionId = await _userSessionProvider.ResolveAsync();
        await AddIfNotExistsAsync(formPath, userSessionId);
        return await _formStorageProvider.GetAsync(formPath, userSessionId);
    }

    public async Task SaveAsync(FormModel form)
    {
        // Only save the form if the Id exists
        if (form.Id != ContentId.Empty)
        {
            string userSessionId = await _userSessionProvider.ResolveAsync();
            await _formStorageProvider.SaveAsync(userSessionId, form);
        }
    }

    public async Task SubmitAsync(FormModel form)
    {
        FormModel submittableForm = form.GetSubmittable();
        string userSessionId = await _userSessionProvider.ResolveAsync();
        await _submitFormService.SubmitAsync(submittableForm, userSessionId);
    }

    public async Task RemoveAsync(FormModel form)
    {
        // Reset the form Id to empty as the form service auto saves and once this from has been removed
        string userSessionId = await _userSessionProvider.ResolveAsync();
        await _formStorageProvider.RemoveAsync(userSessionId, form);
        form.Id = ContentId.Empty;
    }

    private async Task AddIfNotExistsAsync(ContentPath formPath, string userSessionId)
    {
        if (!await _formStorageProvider.ExistsAsync(formPath, userSessionId))
        {
            FormModel form = _formFactory.Create();
            form.Id = userSessionId;
            await _formStorageProvider.SaveAsync(userSessionId, form);
        }
    }
}