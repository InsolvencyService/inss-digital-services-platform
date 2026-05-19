// ReSharper disable UnusedAutoPropertyAccessor.Global - Dynamics object
using System.Text.Json.Serialization;

namespace Inss.FormsSubmission.Service.IPUpload.Employee;

public sealed class Employee
{
    [JsonPropertyName("title")] 
    public string Title { get; init; }

    [JsonPropertyName("first_names")] 
    public string FirstNames { get; init; }

    [JsonPropertyName("last_name")] 
    public string LastName { get; init; }

    [JsonPropertyName("national_insurance_number")]
    public string NationalInsuranceNumber { get; init; }

    [JsonPropertyName("date_of_birth")] 
    public DateTime? DateOfBirth { get; init; }

    [JsonPropertyName("start_date")] 
    public DateTime? StartDate { get; init; }

    [JsonPropertyName("end_date")] 
    public DateTime? EndDate { get; init; }

    [JsonPropertyName("date_notice_given")]
    public DateTime? DateNoticeGiven { get; init; }

    [JsonPropertyName("is_director")] 
    public string IsDirector { get; init; }

    [JsonPropertyName("average_hours_worked")]
    public decimal? AverageHoursWorked { get; init; }

    [JsonPropertyName("money_owed_to_employer")]
    public decimal? MoneyOwedToEmployer { get; init; }

    [JsonPropertyName("entitled_to_redundancy_pay")]
    public string EntitledToRedundancyPay { get; init; }

    [JsonPropertyName("entitled_to_notice_pay")]
    public string EntitledToNoticePay { get; init; }

    [JsonPropertyName("pay")] 
    public Pay Pay { get; init; }
}