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