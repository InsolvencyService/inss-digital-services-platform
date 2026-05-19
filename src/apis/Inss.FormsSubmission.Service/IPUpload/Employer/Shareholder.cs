using System.Text.Json.Serialization;

namespace Inss.FormsSubmission.Service.IPUpload.Employer;

public class Shareholder
{
    [JsonPropertyName("fullname")]
    public string Fullname { get; init; }

    [JsonPropertyName("number_of_shares_held")]
    public int? NumberOfSharesHeld { get; init; }

    [JsonPropertyName("percentage")]
    public decimal? Percentage { get; init; }
}