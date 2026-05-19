namespace Inss.Common.IPUpload;

public sealed class SubmitIPUploadRequest
{
    public string UserId { get; init; }
    
    public bool IsEmployeeUpload { get; init; }
    
    public string Xml { get; init; }
}