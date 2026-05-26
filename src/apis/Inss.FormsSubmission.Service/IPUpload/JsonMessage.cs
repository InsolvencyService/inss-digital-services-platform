// ReSharper disable UnusedAutoPropertyAccessor.Global - Dynamics object
namespace Inss.FormsSubmission.Service.IPUpload;

public sealed class JsonMessage
{
    public string CorrelationId { get; init; }
    
    public string Json { get; init; }
    
    public string Entity { get; init; }
    
    public string MessageName { get; init; }
}