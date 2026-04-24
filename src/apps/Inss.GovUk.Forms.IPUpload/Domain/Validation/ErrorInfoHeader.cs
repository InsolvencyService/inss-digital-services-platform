namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public class ErrorInfoHeader
{
    public string Category { get; init; }
    
    public string Property { get; init; }
    
    public string Error { get; init; }
    
    public string? Hint { get; init; }
}