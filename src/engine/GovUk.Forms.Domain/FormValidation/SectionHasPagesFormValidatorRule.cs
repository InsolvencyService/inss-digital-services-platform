namespace GovUk.Forms.Domain.FormValidation;

internal sealed class SectionHasPagesFormValidatorRule : FormValidatorRule
{
    internal override string[] Validate(FormModel form)
    {
        List<string> errors = [];
        
        foreach (SectionModel section in form.Sections)
        {
            if (section.Pages.Count == 0)
            {
                errors.Add("The section contains no pages. At least 1 page should exist."); 
            }
        }
        
        return errors.ToArray();
    }
}