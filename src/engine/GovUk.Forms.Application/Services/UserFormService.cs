using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.Application.Services;

public sealed class UserFormService : IUserFormService
{
    private readonly IUserSessionProvider _userSessionProvider;
    private readonly IFormStorageProvider _formStorageProvider;
    private readonly IFormProvider _formProvider;
    private readonly ISubmitFormService _submitFormService;
    private readonly IServiceProvider _serviceProvider;

    public UserFormService(
        IUserSessionProvider userSessionProvider, 
        IFormStorageProvider formStorageProvider, 
        IFormProvider formProvider,
        ISubmitFormService submitFormService,
        IServiceProvider serviceProvider)
    {
        _userSessionProvider = userSessionProvider;
        _formStorageProvider = formStorageProvider;
        _formProvider = formProvider;
        _submitFormService = submitFormService;
        _serviceProvider = serviceProvider;
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
            FormModel form = _formProvider.Create(formPath);
            form.Id = userSessionId;
            
            foreach (SectionModel section in form.Sections)
            {
                IFlowchart flowchart = _serviceProvider.GetRequiredKeyedService<IFlowchart>(section.Path);
                flowchart.TransitionPageToStart(section.FirstPage);
            }
            
            IFormPrePopulationService formPrePopulationService = 
                _serviceProvider.GetService<IFormPrePopulationService>() ?? NoopFormPrePopulationService.Default;
            await formPrePopulationService.PrePopulateAsync(form, userSessionId);
            await _formStorageProvider.SaveAsync(userSessionId, form);
        }
    }
}