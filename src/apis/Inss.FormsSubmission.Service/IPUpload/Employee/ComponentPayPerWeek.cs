// ReSharper disable UnusedAutoPropertyAccessor.Global - Dynamics object
using System.Text.Json.Serialization;

namespace Inss.FormsSubmission.Service.IPUpload.Employee;

public sealed class ComponentPayPerWeek
{
    [JsonPropertyName("component_type")]
    public string ComponentType { get; init; }

    [JsonPropertyName("component_rate")]
    public string ComponentRate { get; init; }

    [JsonPropertyName("component_rate_status")]
    public string ComponentRateStatus { get; init; }
}