using System.Text.Json.Serialization;

namespace Inss.FormsSubmission.Service.IPUpload.Employer;

public sealed class Insolvency
{
    [JsonPropertyName("insolvency_date")]
    public DateTime? InsolvencyDate { get; init; }

    [JsonPropertyName("insolvency_type")]
    public string InsolvencyType { get; init; }
}