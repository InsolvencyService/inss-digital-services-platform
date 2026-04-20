namespace Inss.GovUk.Forms.IPUpload.Domain;

public sealed class ErrorInfo
{
    public string Category { get; init; }
    
    public string Property { get; init; }
    
    public string SubCategory { get; init; }
    
    public int Count { get; set; }
}