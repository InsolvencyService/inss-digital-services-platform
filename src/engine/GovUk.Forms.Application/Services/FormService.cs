using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.Services;

public sealed class FormService : IFormService
{
    private readonly IUserFormService _userFormService;
    private readonly ITreeViewManager _treeViewManager;

    public FormService(IUserFormService userFormService, ITreeViewManager treeViewManager)
    {
        _userFormService = userFormService;
        _treeViewManager = treeViewManager;
    }
    
    public async Task<(ContentModel? Content, ContentPath? RedirectTo)> LoadAsync(
        ContentPath requestPath, 
        Dictionary<string, string?> queryParams)
    {
        // TODO: Fix return to just the model
        FormModel form = await _userFormService.GetAsync(requestPath);

        try
        {
            SectionModel? section = null;
            
            if (form.Path == requestPath)
            {
                if (form.Sections.Count > 1)
                {
                    return new ValueTuple<ContentModel?, ContentPath?>(form, null); 
                }
                
                section = form.Sections.First();
            }

            if (section is null)
            {
                section = form.Sections.FindSection(requestPath);
                
                if (section is null) // TODO: Throw if the request path is not the form
                {
                    return new ValueTuple<ContentModel?, ContentPath?>(form, null);
                }
            }

            if (section.Pages.Count == 0 && section.TreeNodeId is null)
            {
                ContentPath redirectPath = _treeViewManager.TransitionToStart(section);
                return new ValueTuple<ContentModel?, ContentPath?>(null, redirectPath);    
            }
            
            PageModel page = await _treeViewManager.LoadAsync(form, section, requestPath, queryParams);
            return new ValueTuple<ContentModel?, ContentPath?>(page, null);
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
            SectionModel section = form.Sections.FindSection(page.Path)!; // TODO: Fix ! form.GetSectionForPage(page.Path);
            return await _treeViewManager.ValidateAsync(form, section, page);
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
                SectionModel section = form.Sections.FindSection(page.Path)!; // TODO: Fix ! form.GetSectionForPage(page.Path);
                return await _treeViewManager.SaveAsync(form, section, page);
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