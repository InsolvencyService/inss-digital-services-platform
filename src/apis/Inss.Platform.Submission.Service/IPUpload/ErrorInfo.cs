using System.Text.Json.Serialization;

namespace Inss.Platform.Submission.Service.IPUpload;

public sealed class ErrorInfo
{
    [JsonPropertyName("error")]
    public ErrorDetails Error { get; init; }
}