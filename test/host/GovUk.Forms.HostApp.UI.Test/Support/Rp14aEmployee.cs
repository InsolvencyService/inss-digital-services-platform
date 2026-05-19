namespace GovUk.Forms.HostApp.UI.Test.Support;

public sealed record Rp14aEmployee
{
    public string CaseReference { get; init; } = "CN12445678";
    public string EmployerName { get; init; } = "Employer Test";
    public string Surname { get; init; } = "Surname Test";
    public string Forenames { get; init; } = "Forname Test";
    public string Title { get; init; } = "Mr";
    public string NIClass { get; init; } = "A";
    public string NationalInsuranceNumber { get; init; } = "BP011752C";
    public string DateOfBirth { get; init; } = "1990-01-01";
    public DateOnly? StartDate { get; init; } = new(2019, 1, 1);
    public DateOnly? EndDate { get; init; } = new(2020, 3, 1);
    public string DateNoticeGiven { get; init; } = "2020-01-01";
    public string IsDirector { get; init; } = "No";
    public string AverageHoursWorked { get; init; } = "37";
    public string MoneyOwedToEmployer { get; init; } = "1000";
    public string EntitledToRedundancyPay { get; init; } = "Yes";
    public string EntitledToNoticePay { get; init; } = "Yes";

    public string BasicPayPerWeek { get; init; } = "1000";
    public string Component1Type { get; init; } = "Holiday Pay Accrued";
    public string Component1Rate { get; init; } = "1000";
    public string Component1RateStatus { get; init; } = "";
    public string Component2Type { get; init; } = "Holiday Pay Taken Not Paid";
    public string Component2RateStatus { get; init; } = "Fixed rate of pay";
    public string WeeklyPayDay { get; init; } = "Monday";

    public string HolidayYearStart { get; init; } = "2020-01-01";
    public string HolidayContractedEntitlementDays { get; init; } = "30";
    public string HolidayDaysCarriedForward { get; init; } = "10";
    public string HolidayDaysTaken { get; init; } = "2";
    public string NoDaysHolidayOwed { get; init; } = "10";

    public List<ArrearsOfPayPeriod> ArrearsOfPayPeriods { get; init; } =
    [
        new(1, "2020-01-10", "2020-01-11", "100", "attachmentofearnings"),
        new(2, "2020-01-12", "2020-01-13", "100", "bouncedcheque"),
        new(3, "2020-01-14", "2020-01-15", "100", "commission"),
        new(4, "2020-01-16", "2020-01-17", "100", "overtime")
    ];

    public List<HolidayPeriod> HolidaysNotPaid { get; init; } =
    [
        new(1, "2020-01-20", "2020-01-21"),
        new(2, "2020-01-22", "2020-01-23"),
        new(3, "2020-01-24", "2020-01-25")
    ];

    public static Rp14aEmployee Default() => new();

    public sealed record ArrearsOfPayPeriod(
    int PeriodNumber,
    string StartDate,
    string EndDate,
    string AmountOwed,
    string PayType);

    public sealed record HolidayPeriod(
        int HolidayNumber,
        string StartDate,
        string EndDate);
}
