namespace GovUk.Forms.Domain.FormValidation;

internal sealed class PageTitlesExistFormValidatorRule : FormValidatorRule
{
    internal override string[] Validate(FormModel form)
    {
        List<string> errors = [];
        
        foreach (SectionModel section in form.Sections)
        {
            foreach (PageModel page in section.Pages.GetAllPathPages())
            {
                if (string.IsNullOrWhiteSpace(page.Title))
                {
                    errors.Add("The page title is missing.");
                }
            }
        }
        
        return errors.ToArray();
    }
}