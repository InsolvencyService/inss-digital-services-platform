using System.Text.Json.Serialization;

namespace Inss.FormsSubmission.Service.IPUpload.Employer;

public sealed class TransferTo
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("address")]
    public Address Address { get; init; }

    [JsonPropertyName("transfer_date")]
    public DateTime? TransferDate { get; init; }

    [JsonPropertyName("date_negotiation_began")]
    public DateTime? DateNegotiationBegan { get; init; }
}