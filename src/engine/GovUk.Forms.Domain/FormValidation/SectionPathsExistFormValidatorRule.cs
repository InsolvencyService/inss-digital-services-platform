namespace GovUk.Forms.Domain.FormValidation;

internal sealed class SectionPathsExistFormValidatorRule : FormValidatorRule
{
    internal override string[] Validate(FormModel form)
    {
        List<string> errors = [];
        
        foreach (SectionModel section in form.Sections)
        {
            if (!section.Path.IsValid())
            {
                errors.Add("The section path is an invalid format.");
            }
        }
        
        return errors.ToArray();
    }
}