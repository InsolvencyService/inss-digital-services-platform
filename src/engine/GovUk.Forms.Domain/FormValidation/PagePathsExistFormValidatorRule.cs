namespace GovUk.Forms.Domain.FormValidation;

internal sealed class PagePathsExistFormValidatorRule : FormValidatorRule
{
    internal override string[] Validate(FormModel form)
    {
        List<string> errors = [];
        
        foreach (SectionModel section in form.Sections)
        {
            foreach (PageModel page in section.Pages.GetAllPathPages())
            {
                if (!page.Path.IsValid())
                {
                    errors.Add("The page path is an invalid format.");
                }
            }
        }
        
        return errors.ToArray();
    }
}