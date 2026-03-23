namespace GovUk.Forms.Domain.FormValidation;

internal sealed class FormPathExistsFormValidatorRule : FormValidatorRule
{
    internal override string[] Validate(FormModel form)
    {
        return !form.Path.IsValid() ? ["The form path is an invalid format."] : [];
    }
}