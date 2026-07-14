namespace GovUk.Forms.Domain.FormValidation;

internal sealed class GroupPagesAreDefinedFormValidatorRule : FormValidatorRule
{
    internal override string[] Validate(FormModel form)
    {
        List<string> errors = [];
        
        foreach (SectionModel section in form.Sections)
        {
            foreach (PageModel page in section.Pages)
            {
                if (page is AddAnotherGroup addAnotherGroup)
                {
                    if (addAnotherGroup.WorkingPages.Count == 0)
                    {
                        errors.Add("There are no working pages for the add another group defined.");
                    }
                }
            }
        }
        
        return errors.ToArray();
    }
}