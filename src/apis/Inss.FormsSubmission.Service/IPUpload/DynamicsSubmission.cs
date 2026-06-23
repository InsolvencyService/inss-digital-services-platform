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
    
    public ErrorInfo? ErrorInfo { get; set; }
    
    public DateTimeOffset SubmissionTimestamp { get; init; }
    
    public string? IPEmailReceipt { get; set; }
    
    public string? InternalEmailReceipt { get; set; }
}