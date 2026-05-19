using System.Text.Json.Serialization;

namespace Inss.FormsSubmission.Service.IPUpload.Employee;

public sealed class Pay
{
    [JsonPropertyName("pay_per_week")]
    public decimal? PayPerWeek { get; init; }

    [JsonPropertyName("weekly_pay_day")]
    public string WeeklyPayDay { get; init; }

    [JsonPropertyName("component_pay_per_week")]
    public IList<ComponentPayPerWeek> ComponentPayPerWeek { get; init; }

    [JsonPropertyName("arrears_of_pay")]
    public IList<ArrearsOfPay> ArrearsOfPay { get; init; }

    [JsonPropertyName("holiday")]
    public Holiday Holiday { get; init; }
}