using System.Text.Json.Serialization;

namespace Inss.FormsSubmission.Service.IPUpload.Employer;

public class PreviousEmployer
{
    [JsonPropertyName("employer_name")]
    public string EmployerName { get; init; }

    [JsonPropertyName("address")]
    public Address Address { get; init; }

    [JsonPropertyName("date_trading_started")]
    public DateTime? DateTradingStarted { get; init; }

    [JsonPropertyName("should_claims_be_accepted")]
    public bool? ShouldClaimsBeAccepted { get; init; }

    [JsonPropertyName("strikes")]
    public bool? Strikes { get; init; }

    [JsonPropertyName("entitled_to_carry_over_holiday")]
    public bool? EntitledToCarryOverHoliday { get; init; }
}