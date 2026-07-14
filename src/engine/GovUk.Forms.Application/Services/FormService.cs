using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.Application.Services;

public sealed class FormService : IFormService
{
    private readonly IUserFormService _userFormService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IPagePropertiesProvider _pagePropertiesProvider;

    public FormService(IUserFormService userFormService, IServiceProvider serviceProvider, IPagePropertiesProvider pagePropertiesProvider)
    {
        _userFormService = userFormService;
        _serviceProvider = serviceProvider;
        _pagePropertiesProvider = pagePropertiesProvider;
    }
    
    public async Task<(ContentModel? Content, ContentPath? RedirectTo, PageValidationError[]? ValidationErrors)> LoadAsync(
        ContentPath requestPath, 
        Dictionary<string, string?> queryParams)
    {
        FormModel form = await _userFormService.GetAsync(requestPath);
        
        try
        {
            ContentModel content = form.GetContent(requestPath);

            if (content is PageModel page)
            {
                SectionModel section = form.GetSectionForPage(page.Path);
                IFlowchart flowchart = _serviceProvider.GetRequiredKeyedService<IFlowchart>(section.Path);
                
                if (section.PageValidation is not null)
                {
                    PageModel pageWithErrors = section.PageValidation.Page;
                    pageWithErrors.MetaData = page.MetaData;
                    PageValidationError[] errors = section.PageValidation.Errors;
                    section.PageValidation = null;
                    await flowchart.UpdateBackButtonAsync(form, section, pageWithErrors);
                    _pagePropertiesProvider.PageTitle = pageWithErrors.Title;
                    return new ValueTuple<ContentModel?, ContentPath?, PageValidationError[]?>(pageWithErrors, null, errors);
                }
                
                _pagePropertiesProvider.PageTitle = page.Title;
                ContentPath altPath = await flowchart.PreProcessAsync(form, section, page, queryParams);
                await flowchart.UpdateBackButtonAsync(form, section, page);
                return new ValueTuple<ContentModel?, ContentPath?, PageValidationError[]?>(
                    content, altPath != requestPath ? altPath : null, null);
            }

            if (form.Sections.Count == 1)
            {
                return new ValueTuple<ContentModel?, ContentPath?, PageValidationError[]?>(null, form.Sections[0].FirstPage.Path, null);
            }
            else
            {
                _pagePropertiesProvider.PreviousPagePath = "/";
            }
            
            return new ValueTuple<ContentModel?, ContentPath?, PageValidationError[]?>(form, null, null);
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
            ValidationResult[] validationResults = await flowchart.ValidateAsync(form, section, page);

            if (validationResults.Length > 0)
            {
                section.PageValidation = new PageValidationInfo
                {
                    Page = page,
                    Errors = validationResults
                        .Select(vr => new PageValidationError
                        {
                            Properties = vr.MemberNames.ToArray(),
                            Message = vr.ErrorMessage ?? string.Empty
                        })
                        .ToArray()
                };
                
                await _userFormService.SaveAsync(form);
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