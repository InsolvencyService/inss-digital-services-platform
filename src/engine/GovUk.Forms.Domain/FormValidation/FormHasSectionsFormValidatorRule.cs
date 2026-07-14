namespace GovUk.Forms.Domain.FormValidation;

internal sealed class FormHasSectionsFormValidatorRule : FormValidatorRule
{
    internal override string[] Validate(FormModel form)
    {
        return form.Sections.Count == 0 ? ["The form contains no sections. At least 1 section should exist."] : [];
    }
}