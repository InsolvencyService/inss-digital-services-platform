namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Lookups;

internal sealed class EmployeePayLookups : Dictionary<string, ErrorInfoHeader>
{
    internal EmployeePayLookups()
    {
        this["InvalidFormatEmployeeBasicPayPerWeek"] = new ErrorInfoHeader
        {
            Category = "Employee pay", Property = "Employee basic pay per week", Error = "[COUNT] invalid basic pay per week", Hint = "Expected format is 12.34 or 100"
        };
    }
}