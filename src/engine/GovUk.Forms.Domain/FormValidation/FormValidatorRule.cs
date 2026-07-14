namespace GovUk.Forms.Domain.FormValidation;

internal abstract class FormValidatorRule
{
    internal abstract string[] Validate(FormModel form);
}