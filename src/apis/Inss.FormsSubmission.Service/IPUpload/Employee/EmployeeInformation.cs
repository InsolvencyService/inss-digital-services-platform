using System.Text.Json.Serialization;

namespace Inss.FormsSubmission.Service.IPUpload.Employee;

public sealed class EmployeeInformation
{
    [JsonPropertyName("correlation_id")]
    public Guid CorrelationId { get; init; }

    [JsonPropertyName("case_reference")]
    public string CaseReference { get; init; }

    [JsonPropertyName("employer_name")]
    public string EmployerName { get; init; }

    [JsonPropertyName("employee")]
    public Employee Employee { get; init; }
}