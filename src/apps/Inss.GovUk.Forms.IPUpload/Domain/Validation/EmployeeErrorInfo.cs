using System.Globalization;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public sealed class EmployeeErrorInfo : ErrorInfo
{
    public EmployeeErrorInfo(
        string category, 
        string property, 
        string error,
        string? hint,
        string forenames, 
        string surname, 
        DateOnly dob, 
        string niNumber, 
        string? cellValue)
    {
        Category = category;
        Property = property;
        Error = error;
        Hint = hint;
        
        AddHeader("Forenames", "Surname", "Date of birth", "NI number", "Cell value");
        AddRow(forenames, surname, dob.ToString(CultureInfo.CurrentCulture), niNumber, cellValue ?? "Not entered");
    }
}