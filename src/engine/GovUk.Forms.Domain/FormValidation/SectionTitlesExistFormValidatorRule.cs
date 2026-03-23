namespace GovUk.Forms.Domain.FormValidation;

internal sealed class SectionTitlesExistFormValidatorRule : FormValidatorRule
{
    internal override string[] Validate(FormModel form)
    {
        List<string> errors = [];
        
        foreach (SectionModel section in form.Sections)
        {
            if (string.IsNullOrWhiteSpace(section.Title))
            {
                errors.Add("The section title is missing.");
            }
        }
        
        return errors.ToArray();
    }
}