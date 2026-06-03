namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;

internal sealed class EmployeeValidatorContext : ValidatorContext
{
    internal string Forenames { get; set; }
    
    internal string Surname { get; set; }
    
    internal DateOnly Dob { get; set; }
    
    internal string Nino { get; set; }

    internal override void AddError(ValidationInfo validationInfo, string? value)
    {
        AddError(new EmployeeError
        {
            Info = validationInfo,
            Forenames = Forenames,
            Surname = Surname,
            Dob = Dob,
            Nino = Nino,
            Value = value
        });
    }
}