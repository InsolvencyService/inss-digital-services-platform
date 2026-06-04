namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;

public sealed class EmployeeValidatorContext : ValidatorContext
{
    public string Forenames { get; set; }
    
    public string Surname { get; set; }
    
    public DateOnly Dob { get; set; }
    
    public string Nino { get; set; }

    public override void AddError(ValidationInfo validationInfo, string? value)
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