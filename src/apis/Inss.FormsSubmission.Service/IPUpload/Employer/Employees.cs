using System.Text.Json.Serialization;

namespace Inss.FormsSubmission.Service.IPUpload.Employer;

public class Employees
{
    [JsonPropertyName("number_of_employees")]
    public int? NumberOfEmployees { get; init; }

    [JsonPropertyName("claiming_continuity")]
    public bool? Continuity { get; init; }

    [JsonPropertyName("previous_employer")]
    public PreviousEmployer PreviousEmployer { get; init; }
}