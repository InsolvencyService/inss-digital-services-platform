using System.Text.Json.Serialization;

namespace Inss.FormsSubmission.Service.IPUpload.Employer;

public sealed class PayRecordsContact
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("pay_records_address")]
    public Address PayRecordsAddress { get; init; }

    [JsonPropertyName("telephone_number")]
    public string TelephoneNumber { get; init; }

    [JsonPropertyName("email_address")]
    public string EmailAddress { get; init; }
}