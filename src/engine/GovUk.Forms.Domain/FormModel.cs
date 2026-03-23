using GovUk.Forms.Domain.Enums;
using GovUk.Forms.Domain.Exceptions;
using GovUk.Forms.Domain.FormValidation;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Domain;

public class FormModel : ContentModel
{
    public SectionModelList Sections { get; init; } = [];
    
    public bool CanSubmit => Sections.All(s => s.State == SectionStateTypes.Completed);
    
    public ContentModel GetContent(ContentPath path)
    {
        if (Path == path)
        {
            return this;
        }
        
        foreach (PageModel page in GetAllPages())
        {
            if (page.Path == path)
            {
                return page;
            }
        }

        throw new ModelException($"Unable to find page for path {path}.");
    }
    
    public SectionModel GetSectionForPage(ContentPath path)
    {
        foreach (SectionModel section in Sections)
        {
            foreach (PageModel page in section.Pages.GetAllPathPages())
            {
                if (path.Value.Equals(page.Path.Value, StringComparison.OrdinalIgnoreCase))
                {
                    return section;
                }   
            }
        }

        throw new ModelException($"Unable to find section for page path {path}.");
    }
    
    public PageModelList GetAllPages()
    {
        PageModelList allPages = [];

        foreach (SectionModel section in Sections)
        {
            foreach (PageModel page in section.Pages.GetAllPathPages())
            {
                if (allPages.FirstOrDefault(p => p.Path == page.Path) is null)
                {
                    allPages.Add(page);
                }
            }
        }
    
        return allPages;
    }

    public void Validate()
    {
        List<string> formErrors = [];
        
        foreach (FormValidatorRule rule in ValidationRules)
        {
            string[] errors = rule.Validate(this);
            formErrors.AddRange(errors);
        }
        
        if (formErrors.Count > 0)
        {
            throw new FormValidationException(formErrors);
        }
    }

    public FormModel GetSubmittable()
    {
        FormModel submittableForm = new() { Id = Id, Path = Path };

        foreach (SectionModel section in Sections)
        {
            SectionModel submittableSection = new()
            {
                Id = section.Id,
                Path = section.Path,
                State = SectionStateTypes.Completed,
                Title = section.Title
            };

            foreach (PageModel page in section.Pages)
            {
                if (page is SummaryModel)
                {
                    continue;
                }
                
                if (page is GroupPageModel groupPage)
                {
                    foreach (PageModel subPage in groupPage.SubmittablePages)
                    {
                        submittableSection.Pages.Add(subPage.Clone());
                    }
                }
                else
                {
                    submittableSection.Pages.Add(page.Clone());
                }
            }
            
            submittableForm.Sections.Add(submittableSection);
        }
        
        return submittableForm;
    }
    
    private static IEnumerable<FormValidatorRule> ValidationRules
    {
        get
        {
            yield return new FormPathExistsFormValidatorRule();
            yield return new FormHasSectionsFormValidatorRule();
            yield return new SectionPathsExistFormValidatorRule();
            yield return new SectionTitlesExistFormValidatorRule();
            yield return new SectionHasPagesFormValidatorRule();
            yield return new SectionHasUniqueGroupsFormValidatorRule();
            yield return new PagePathsExistFormValidatorRule();
            yield return new PageTitlesExistFormValidatorRule();
            yield return new GroupPagesAreDefinedFormValidatorRule();
        }
    }
}