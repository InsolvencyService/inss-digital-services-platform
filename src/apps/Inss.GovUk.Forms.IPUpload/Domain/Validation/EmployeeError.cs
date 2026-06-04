namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public sealed class EmployeeError : Error
{
    public string Forenames { get; init; }
    
    public string Surname { get; init; }
    
    public DateOnly Dob { get; init; }
    
    public string Nino { get; init; }
    
    public string? Value { get; init; }
}