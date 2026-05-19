using System.Text.Json.Serialization;

namespace Inss.FormsSubmission.Service.IPUpload.Employer;

public class Transfer
{
    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("to")]
    public TransferTo To { get; init; }
}