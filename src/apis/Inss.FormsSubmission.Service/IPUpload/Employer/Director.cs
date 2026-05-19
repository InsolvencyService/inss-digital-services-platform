using System.Text.Json.Serialization;

namespace Inss.FormsSubmission.Service.IPUpload.Employer;

public class Director 
{
    [JsonPropertyName("surname")]
    public string Surname { get; init; }

    [JsonPropertyName("initials")]
    public string Initials { get; init; }

    [JsonPropertyName("national_insurance_number")]
    public string NationalInsuranceNumber { get; init; }
}