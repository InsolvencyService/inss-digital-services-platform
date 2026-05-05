using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.Application.Services;

public sealed class FormService : IFormService
{
    private readonly IUserFormService _userFormService;
    private readonly IServiceProvider _serviceProvider;

    public FormService(IUserFormService userFormService, IServiceProvider serviceProvider)
    {
        _userFormService = userFormService;
        _serviceProvider = serviceProvider;
    }
    
    public async Task<(ContentModel? Content, ContentPath? RedirectTo)> LoadAsync(ContentPath path, string? state)
    {
        FormModel form = await _userFormService.GetAsync(path);
        
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
            await _userFormService.SaveAsync(form);
        }
    }
    
    public async Task<ValidationResult[]> ValidateAsync(ContentModel postedContent)
    {
        FormModel form = await _userFormService.GetAsync(postedContent.Path);
        
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
        FormModel form = await _userFormService.GetAsync(postedContent.Path);
        
        try
        {
            if (postedContent is PageModel page)
            {
                SectionModel section = form.GetSectionForPage(page.Path);
                IFlowchart flowchart = _serviceProvider.GetRequiredKeyedService<IFlowchart>(section.Path);
                return await flowchart.ProcessAsync(form, section, page);
            }

            FormModel submittableForm = form.GetSubmittable();

            await _userFormService.SubmitAsync(submittableForm);
            
            return submittableForm.Path;
        }
        finally
        {
            await _userFormService.SaveAsync(form);
        }
    }
}