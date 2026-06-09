using System.Text.Json.Serialization;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Inss.FormsSubmission.Service.IPUpload;

public sealed class ErrorDetails
{
    [JsonPropertyName("code")]
    public string Code { get; init; }
    
    [JsonPropertyName("message")]
    public string Message { get; init; }    
}