using System.Text.Json.Serialization;

namespace Inss.FormsSubmission.Service.IPUpload.Employer;

public sealed class Address
{
    [JsonPropertyName("line_1")]
    public string Line1 { get; init; }

    [JsonPropertyName("line_2")]
    public string Line2 { get; init; }

    [JsonPropertyName("line_3")]
    public string Line3 { get; init; }

    [JsonPropertyName("town")]
    public string Town { get; init; }

    [JsonPropertyName("county")]
    public string County { get; init; }

    [JsonPropertyName("postcode")]
    public string Postcode { get; init; }

    [JsonPropertyName("country")]
    public string Country { get; init; }
}