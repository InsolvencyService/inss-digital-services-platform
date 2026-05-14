namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public sealed class EmployerErrorInfo : ErrorInfo
{
    public EmployerErrorInfo(
        string category, 
        string property, 
        string error,
        string? hint,
        string[] headers,
        string?[] cellValues)
    {
        Category = category;
        Property = property;
        Error = error;
        Hint = hint;
        ShowErrorDetails = false;

        AddHeader(headers);
        AddRow(cellValues.Select(v => v ?? "Not entered").ToArray());
    }
}