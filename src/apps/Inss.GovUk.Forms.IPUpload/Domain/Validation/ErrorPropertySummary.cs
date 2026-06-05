// ReSharper disable MemberCanBePrivate.Global - Cosmos won't serialize properly
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global - Cosmos won't serialize properly
using System.Globalization;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public sealed class ErrorPropertySummary
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    
    public EmployeeError[] EmployeeUploadErrors { get; set; } = [];
    
    public EmployerError[] EmployerUploadErrors { get; set; } = [];
    
    public ValidationInfo Info { get; init; }
    
    public bool HasEmployeeErrors => EmployeeUploadErrors.Length > 0;

    internal string GetFormattedMessage()
    {
        if (HasEmployeeErrors)
        {
            if (EmployeeUploadErrors.Length > 1)
            {
                return Info.PluralErrorPattern.Replace("[COUNT]", EmployeeUploadErrors.Length.ToString(CultureInfo.InvariantCulture));
            }

            return Info.SingularErrorPattern;
        }

        return EmployerUploadErrors.Length > 1 
            ? Info.PluralErrorPattern.Replace("[COUNT]", EmployerUploadErrors.Length.ToString(CultureInfo.InvariantCulture)) 
            : Info.SingularErrorPattern;
    }

    internal void AddError(Error error)
    {
        if (error is EmployeeError employeeError)
        {
            List<EmployeeError> errors = [..EmployeeUploadErrors, employeeError];
            EmployeeUploadErrors = errors.ToArray();
        }
        else if (error is EmployerError employerError)
        {
            List<EmployerError> errors = [..EmployerUploadErrors, employerError];
            EmployerUploadErrors = errors.ToArray();
        }
    }
}