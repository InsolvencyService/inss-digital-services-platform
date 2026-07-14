using System.Text.Json.Serialization;

namespace Inss.FormsSubmission.Service.IPUpload;

public sealed class ErrorInfo
{
    [JsonPropertyName("error")]
    public ErrorDetails Error { get; init; }
}