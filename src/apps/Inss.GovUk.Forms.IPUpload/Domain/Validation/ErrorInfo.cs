using System.Globalization;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public class ErrorInfo : ErrorInfoHeader
{
    public string Id { get; init; } = Guid.NewGuid().ToString();

    public int RowCount => Rows.Length;
    
    public string[] Headers { get; private set; } = [];
    
    public string[][] Rows { get; private set; } = [];

    public string HeaderCaption { get; set; }

    public bool ShowErrorDetails { get; set; } = true;
    
    public int ColumnCount => Headers.Length;

    public void AddHeader(params string[] rowValues)
    {
        Headers = rowValues;
    }
    
    public void AddRow(params string[] rowValues)
    {
        List<string[]> helper = [..Rows, rowValues];
        Rows = helper.ToArray();
    }

    public bool IsMatch(ErrorInfo otherErrorInfo)
    {
        return Category == otherErrorInfo.Category && Property == otherErrorInfo.Property && Error == otherErrorInfo.Error;
    }

    public string[] GetValueRow()
    {
        return Rows.Last();
    }
}

public sealed class EmployeeSpreadsheetErrorInfo : ErrorInfo
{
    public EmployeeSpreadsheetErrorInfo(
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

public sealed class EmployerErrorInfo : ErrorInfo
{
    public EmployerErrorInfo(
        string category, 
        string property, 
        string error,
        string? hint,
        bool showErrorDetails,
        string[] headers,
        string?[] cellValues)
    {
        Category = category;
        Property = property;
        Error = error;
        Hint = hint;
        ShowErrorDetails = showErrorDetails;

        AddHeader(headers);
        AddRow(cellValues.Select(v => v ?? "Not entered").ToArray());
    }
}