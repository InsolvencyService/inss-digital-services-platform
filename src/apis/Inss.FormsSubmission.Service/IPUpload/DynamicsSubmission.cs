// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Inss.FormsSubmission.Service.IPUpload;

public sealed class DynamicsSubmission
{
    public string Id { get; init; }
    
    public string Reference { get; init; }
    
    public string Json { get; init; }
    
    public string UserId { get; init; } 
    
    public PayloadTypes PayloadType { get; init; }
    
    public string? StatusCode { get; init; }
    
    public string? ErrorInfo { get; init; }
    
    public DateTimeOffset SubmissionTimestamp { get; init; }
}