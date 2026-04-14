using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.Application.Services;

public sealed class FormService : IFormService
{
    private readonly IFormStorageProvider _formStorageProvider;
    private readonly IUserSessionProvider _userSessionProvider;
    private readonly ISubmitFormService _submitFormService;
    private readonly IFormProvider _formProvider;
    private readonly IServiceProvider _serviceProvider;

    public FormService(
        IFormStorageProvider formStorageProvider, 
        IUserSessionProvider userSessionProvider,
        ISubmitFormService submitFormService,
        IFormProvider formProvider,
        IServiceProvider serviceProvider)
    {
        _formStorageProvider = formStorageProvider;
        _userSessionProvider = userSessionProvider;
        _submitFormService = submitFormService;
        _formProvider = formProvider;
        _serviceProvider = serviceProvider;
    }
    
    public async Task<(ContentModel? Content, ContentPath? RedirectTo)> LoadAsync(ContentPath path, string? state)
    {
        FormModel form = await GetFormAsync(path);
        
        try
        {
            ContentModel content = form.GetContent(path);

            if (content is PageModel page)
            {
                SectionModel section = form.GetSectionForPage(page.Path);
                IFlowchart flowchart = _serviceProvider.GetRequiredKeyedService<IFlowchart>(section.Path);
                ContentPath altPath = await flowchart.PreProcessAsync(form, section, page, state);
                return new ValueTuple<ContentModel?, ContentPath?>(content, altPath != path ? altPath : null);
            }

            if (form.Sections.Count == 1)
            {
                return new ValueTuple<ContentModel?, ContentPath?>(null, form.Sections[0].FirstPage.Path);
            }
            
            return new ValueTuple<ContentModel?, ContentPath?>(form, null);
        }
        finally
        {
            string userSessionId = await _userSessionProvider.ResolveAsync();
            await _formStorageProvider.SaveAsync(userSessionId, form);
        }
    }
    
    public async Task<ValidationResult[]> ValidateAsync(ContentModel postedContent)
    {
        FormModel form = await GetFormAsync(postedContent.Path);
        
        if (postedContent is PageModel page)
        {
            SectionModel section = form.GetSectionForPage(page.Path);
            IFlowchart flowchart = _serviceProvider.GetRequiredKeyedService<IFlowchart>(section.Path);
            ValidationResult[] validationResults = await flowchart.ValidateAsync(page);

            if (validationResults.Length > 0)
            {
                PageModel savedPage = section.Pages.GetPage(page.Path);
                savedPage.MetaData.CopyTo(page.MetaData);
            }
            
            return validationResults;
        }

        return [];
    }
    
    public async Task<ContentPath> SaveAsync(ContentModel postedContent)
    {
        FormModel form = await GetFormAsync(postedContent.Path);
        string userSessionId = await _userSessionProvider.ResolveAsync();
        
        try
        {
            if (postedContent is PageModel page)
            {
                SectionModel section = form.GetSectionForPage(page.Path);
                IFlowchart flowchart = _serviceProvider.GetRequiredKeyedService<IFlowchart>(section.Path);
                return await flowchart.ProcessAsync(form, section, page);
            }

            FormModel submittableForm = form.GetSubmittable();

            await _submitFormService.SubmitAsync(submittableForm, userSessionId);
            
            return submittableForm.Path;
        }
        finally
        {
            await _formStorageProvider.SaveAsync(userSessionId, form);
        }
    }

    private async Task<FormModel> GetFormAsync(ContentPath path)
    {
        ContentPath formPath = path.GetRoot();
        string userSessionId = await _userSessionProvider.ResolveAsync();
        
        if (!await _formStorageProvider.ExistsAsync(formPath, userSessionId))
        {
            FormModel form = _formProvider.Create(formPath);
            
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

        return await _formStorageProvider.GetAsync(formPath, userSessionId);
    }
}