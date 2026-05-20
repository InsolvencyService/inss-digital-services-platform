using System.Text.Json.Serialization;

namespace Inss.FormsSubmission.Service.IPUpload.Employer;

public sealed class AssociatedCompany
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("number")]
    public string Number { get; init; }

    [JsonPropertyName("address")]
    public Address Address { get; init; }

    [JsonPropertyName("offer_made")]
    public bool OfferMade { get; init; }
}