using GovUk.Forms.HostApp.UI.Test.Support;
using static GovUk.Forms.HostApp.UI.Test.Support.Rp14aEmployee;

namespace GovUk.Forms.HostApp.UI.Test.Builders;

public sealed class Rp14aScenarioBuilder
{
    private string? _caseReference;
    private string? _employerName;
    private int? _employerNameLength;
    private EmployeeNameData? _employeeName;
    private string? _moneyOwedToEmployer;
    private readonly List<ArrearsOfPayPeriodData> _arrearsOfPayPeriods = [];
    private EmploymentDatesData? _employmentDates;
    private string? _nationalInsuranceNumber;

    public static Rp14aScenarioBuilder Create() => new();

    public Rp14aScenarioBuilder WithCaseReference(string? caseReference)
    {
        _caseReference = caseReference;
        return this;
    }

    public Rp14aScenarioBuilder WithEmployerName(string? employerName)
    {
        _employerName = employerName;
        _employerNameLength = null;
        return this;
    }

    public Rp14aScenarioBuilder WithEmployerNameLength(int length)
    {
        _employerName = null;
        _employerNameLength = length;
        return this;
    }

    public Rp14aScenarioBuilder WithEmployeeName(
        string surname,
        string forename,
        string title = "Ms")
    {
        _employeeName = new EmployeeNameData(surname, forename, title);
        return this;
    }

    public Rp14aScenarioBuilder WithMoneyOwedToEmployer(string? amount)
    {
        _moneyOwedToEmployer = amount;
        return this;
    }

    public Rp14aScenarioBuilder WithNationalInsuranceNumber(string? niNumber)
    {
        _nationalInsuranceNumber = niNumber;
        return this;
    }

    public Rp14aScenarioBuilder WithEmploymentDates(
        string startDate,
        string endDate)
    {
        _employmentDates = new EmploymentDatesData(startDate, endDate);
        return this;
    }

    public Rp14aScenarioBuilder WithArrearsOfPayPeriod(
        int periodNumber,
        string startDate,
        string endDate,
        string amountOwed,
        string payType)
    {
        _arrearsOfPayPeriods.RemoveAll(x => x.PeriodNumber == periodNumber);

        _arrearsOfPayPeriods.Add(new ArrearsOfPayPeriodData(
            periodNumber,
            startDate,
            endDate,
            amountOwed,
            payType));

        return this;
    }

    public Rp14aEmployee BuildEmployeeData()
    {
        Rp14aEmployee employee = Default();

        if (_caseReference is not null)
        {
            employee = employee with { CaseReference = _caseReference };
        }

        if (_employerNameLength.HasValue)
        {
            employee = employee with
            {
                EmployerName = new string('A', _employerNameLength.Value)
            };
        }
        else if (_employerName is not null)
        {
            employee = employee with { EmployerName = _employerName };
        }

        if (_employeeName is not null)
        {
            employee = employee with
            {
                Surname = _employeeName.Surname,
                Forenames = _employeeName.Forename,
                Title = _employeeName.Title
            };
        }

        if (_moneyOwedToEmployer is not null)
        {
            employee = employee with
            {
                MoneyOwedToEmployer = _moneyOwedToEmployer
            };
        }

        if (_employmentDates is not null)
        {
            employee = employee with
            {
                StartDate = _employmentDates.StartDate,
                EndDate = _employmentDates.EndDate
            };
        }

        if (_nationalInsuranceNumber is not null)
        {
            employee = employee with
            {
                NationalInsuranceNumber = _nationalInsuranceNumber
            };
        }

        if (_arrearsOfPayPeriods.Count > 0)
        {
            employee = employee with
            {
                ArrearsOfPayPeriods = _arrearsOfPayPeriods
                    .OrderBy(x => x.PeriodNumber)
                    .Select(x => new ArrearsOfPayPeriod(
                        x.PeriodNumber,
                        x.StartDate,
                        x.EndDate,
                        x.AmountOwed,
                        x.PayType))
                    .ToList()
            };
        }

        return employee;
    }

    public string BuildXml()
    {
        return Rp14aXmlScenarioBuilder
            .Create()
            .WithEmployee(this)
            .BuildXml();
    }

    public string GetDescription()
    {
        List<string> parts = [];

        if (_caseReference is not null)
        {
            parts.Add($"Case Reference: '{_caseReference}'");
        }

        if (_employerName is not null)
        {
            parts.Add($"Employer: '{_employerName}'");
        }

        if (_employerNameLength.HasValue)
        {
            parts.Add($"Employer Name Length: {_employerNameLength.Value}");
        }

        if (_employeeName is not null)
        {
            parts.Add($"Employee: {_employeeName.Forename} {_employeeName.Surname}");
        }

        if (_moneyOwedToEmployer is not null)
        {
            parts.Add($"Money Owed To Employer: '{_moneyOwedToEmployer}'");
        }

        if (_arrearsOfPayPeriods.Count > 0)
        {
            parts.Add($"Arrears Periods: {_arrearsOfPayPeriods.Count}");
        }

        if (_employmentDates is not null)
        {
            parts.Add($"Employment: {_employmentDates.StartDate} to {_employmentDates.EndDate}");
        }

        if (_nationalInsuranceNumber is not null)
        {
            parts.Add($"NINO: '{_nationalInsuranceNumber}'");
        }

        return string.Join(" | ", parts);
    }
}
public sealed record EmployeeNameData(
    string Surname,
    string Forename,
    string Title);

public sealed record EmploymentDatesData(
    string StartDate,
    string EndDate);

public sealed record ArrearsOfPayPeriodData(
    int PeriodNumber,
    string StartDate,
    string EndDate,
    string AmountOwed,
    string PayType);
