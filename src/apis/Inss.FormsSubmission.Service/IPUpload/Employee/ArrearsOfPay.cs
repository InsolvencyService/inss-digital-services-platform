// ReSharper disable UnusedAutoPropertyAccessor.Global - Dynamics object
using System.Text.Json.Serialization;

namespace Inss.FormsSubmission.Service.IPUpload.Employee;

public sealed class ArrearsOfPay
{
    [JsonPropertyName("start_date")]
    public DateTime StartDate { get; init; }

    [JsonPropertyName("end_date")]
    public DateTime EndDate { get; init; }

    [JsonPropertyName("amount_owed")]
    public decimal? AmountOwed { get; init; }

    [JsonPropertyName("pay_type")]
    public string Paytype { get; init; }
}