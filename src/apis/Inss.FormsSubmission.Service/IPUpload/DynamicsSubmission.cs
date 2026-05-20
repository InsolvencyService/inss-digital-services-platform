// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Inss.FormsSubmission.Service.IPUpload;

public sealed class DynamicsSubmission
{
    public string Id { get; init; }
    
    public string Reference { get; init; }
    
    public string Json { get; init; }
    
    public string UserId { get; init; } 
    
    public string PayloadType { get; init; }
    
    public string? StatusCode { get; set; }
    
    public string? ErrorInfo { get; set; }
    
    public DateTimeOffset SubmissionTimestamp { get; init; }
}