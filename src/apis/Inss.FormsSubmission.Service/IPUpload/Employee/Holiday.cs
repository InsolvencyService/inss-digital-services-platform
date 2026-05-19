using System.Text.Json.Serialization;

namespace Inss.FormsSubmission.Service.IPUpload.Employee;

public sealed class Holiday
{
    [JsonPropertyName("year_start")]
    public DateTime? YearStart { get; init; }

    [JsonPropertyName("days_owed")]
    public decimal? DaysOwed { get; init; }

    [JsonPropertyName("taken_and_not_paid")]
    public IList<TakenAndNotPaid> TakenAndNotPaid { get; init; }

    [JsonPropertyName("contracted_entitlement_days")]
    public decimal? HolidayContractedEntitlementDays { get; init; }

    [JsonPropertyName("days_carried_forward")]
    public decimal? HolidayDaysCarriedForward { get; init; }

    [JsonPropertyName("days_taken")]
    public decimal? HolidayDaysTaken { get; init; }
}