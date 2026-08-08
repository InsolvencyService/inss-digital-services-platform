// ReSharper disable UnusedAutoPropertyAccessor.Global - request
namespace Inss.Common.IPUpload;

public sealed class SubmitIPUploadRequest
{
    public required string SessionId { get; init; }
    
    public required string Email { get; init; }
    
    public required bool IsEmployeeUpload { get; init; }
    
    public required bool IsApiSource { get; init; }
}