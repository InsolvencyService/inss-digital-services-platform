using System.Text.Json.Serialization;

namespace Inss.FormsSubmission.Service.IPUpload.Employer;

public sealed class Company
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("number")]
    public string Number { get; init; }

    [JsonPropertyName("address")]
    public Address Address { get; init; }

    [JsonPropertyName("incorporation_date")]
    public DateTime? IncorporationDate { get; init; }

    [JsonPropertyName("nature_of_business")]
    public string NatureOfBusiness { get; init; }
}