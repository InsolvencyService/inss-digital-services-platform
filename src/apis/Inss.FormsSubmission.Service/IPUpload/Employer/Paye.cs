using System.Text.Json.Serialization;

namespace Inss.FormsSubmission.Service.IPUpload.Employer;

public class Paye
{
    [JsonPropertyName("district")]
    public string District { get; init; }

    [JsonPropertyName("reference")]
    public string Reference { get; init; }
}