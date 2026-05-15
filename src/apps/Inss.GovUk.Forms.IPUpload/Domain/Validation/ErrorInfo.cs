// ReSharper disable MemberCanBePrivate.Global - Cosmos won't serialize properly
namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public class ErrorInfo : ErrorInfoHeader
{
    public string Id { get; init; } = Guid.NewGuid().ToString();

    public int RowCount => Rows.Length;
    
    public string[] Headers { get; set; } = [];
    
    public string[][] Rows { get; set; } = [];

    public string HeaderCaption { get; init; }

    public bool ShowErrorDetails { get; init; } = true;
    
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