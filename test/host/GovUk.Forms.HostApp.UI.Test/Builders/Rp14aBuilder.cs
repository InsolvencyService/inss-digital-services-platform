using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Support;
using System.Text;
using System.Xml.Linq;
using static GovUk.Forms.HostApp.UI.Test.Models.TestData;

namespace GovUk.Forms.HostApp.UI.Test.Builders;

/// <summary>
/// Builds RP14A XML files for upload tests.
/// 
/// This builder creates a valid default RP14A document and allows
/// individual fields to be overridden for validation scenarios.
/// </summary>
public sealed class Rp14aBuilder
{
    private const string NamespaceUri = "http://www.ins.gsi.gov.uk/FileUpload/RP14A_Application";
    private static readonly XNamespace _ns = NamespaceUri;

    private string _caseReference = "CN12345678";
    private string _nationalInsuranceNumber = ScenarioConstant.NationalInsuranceNumber;
    private string _employerName = ScenarioConstant.EmployerName;
    private string _surname = ScenarioConstant.Surname;
    private string _forenames = ScenarioConstant.Forname;
    private string _title = ScenarioConstant.Title;
    private string _dateOfBirth = ScenarioConstant.DOB;
    private string _moneyOwedToEmployer = "1000";
    private string _startDate = "2019-01-01";
    private string _endDate = "2020-03-01";

    public Rp14aBuilder WithCaseReference(string caseReference)
    {
        _caseReference = caseReference;
        return this;
    }

    public Rp14aBuilder WithNationalInsuranceNumber(string nationalInsuranceNumber)
    {
        _nationalInsuranceNumber = nationalInsuranceNumber;
        return this;
    }

    public Rp14aBuilder WithEmployerName(string employerName)
    {
        _employerName = employerName;
        return this;
    }

    public Rp14aBuilder WithEmployerNameLength(int length)
    {
        _employerName = new string('A', length);
        return this;
    }

    public Rp14aBuilder WithSurname(string surname)
    {
        _surname = surname;
        return this;
    }

    public Rp14aBuilder WithForenames(string forenames)
    {
        _forenames = forenames;
        return this;
    }

    public Rp14aBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public Rp14aBuilder WithDateOfBirth(string dateOfBirth)
    {
        _dateOfBirth = dateOfBirth;
        return this;
    }
    public Rp14aBuilder WithMoneyOwedToEmployer(string value)
    {
        _moneyOwedToEmployer = value;
        return this;
    }

    public Rp14aBuilder WithEmploymentDates(string startDate, string endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
        return this;
    }

    public Rp14aBuilder WithEmployeeName(
    string surname,
    string forenames,
    string title)
    {
        _surname = surname;
        _forenames = forenames;
        _title = title;

        return this;
    }

    public string Build()
    {
        XDocument document = new(
            new XDeclaration("1.0", "UTF-8", "yes"),
            new XElement(_ns + "RP14A",
                new XAttribute(XNamespace.Xmlns + "ns1", _ns),
                BuildEmployee()
            )
        );

        return document.ToString(SaveOptions.None); ;
    }

    public string BuildToFile(TestArtifacts artifacts, string? fileName = null)
    {
        ArgumentNullException.ThrowIfNull(artifacts);

        fileName ??= $"rp14a-{Guid.NewGuid():N}.xml";

        if (!fileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
        {
            fileName += ".xml";
        }

        string filePath = artifacts.FilePath(fileName);

        string xml = Build();

        File.WriteAllText(filePath, xml, Encoding.UTF8);

        return filePath;
    }

    private XElement BuildEmployee()
    {
        return new XElement(_ns + "Employee",
            new XElement(_ns + "Header",
                new XElement(_ns + "CaseReference", _caseReference)
            ),
            new XElement(_ns + "EmployerName", _employerName),
            new XElement(_ns + "EmployeeName",
                new XElement(_ns + "Surname", _surname),
                new XElement(_ns + "Forenames", _forenames),
                new XElement(_ns + "Title", _title)
            ),
            new XElement(_ns + "NIClass", "A"),
            new XElement(_ns + "NINO", _nationalInsuranceNumber),
            new XElement(_ns + "DateOfBirth", _dateOfBirth),
            new XElement(_ns + "StartDate", _startDate),
            new XElement(_ns + "DateNoticeGiven", "2020-01-01"),
            new XElement(_ns + "EndDate", _endDate),
            new XElement(_ns + "IsDirector", "No"),
            new XElement(_ns + "AverageHoursWorked", "37"),
            new XElement(_ns + "MoneyOwedToEmployer", _moneyOwedToEmployer),
            new XElement(_ns + "EntitledToRedundancyPay", "Yes"),
            new XElement(_ns + "EntitledToNoticePay", "Yes"),
            BuildPayDetails(),
            BuildHolidayDetails()
        );
    }

    private XElement BuildPayDetails()
    {
        return new XElement(_ns + "PayDetails",
            new XElement(_ns + "BasicPayPerWeek", "1000"),
            new XElement(_ns + "ComponentPayPerWeek1",
                new XElement(_ns + "ComponentType", "Holiday Pay Accrued"),
                new XElement(_ns + "ComponentRate", "1000"),
                new XElement(_ns + "ComponentRateStatus")
            ),
            new XElement(_ns + "ComponentPayPerWeek2",
                new XElement(_ns + "ComponentType", "Holiday Pay Taken Not Paid"),
                new XElement(_ns + "ComponentRateStatus", "Fixed rate of pay")
            ),
            new XElement(_ns + "WeeklyPayDay", "Monday"),
            new XElement(_ns + "ArrearsOfPay",
                _arrearsOfPayPeriods
                    .OrderBy(x => x.PeriodNumber)
                    .Select(x => BuildArrearsOfPayPeriod(
                        x.PeriodNumber,
                        x.StartDate,
                        x.EndDate,
                        x.AmountOwed,
                        x.PayType))
            )
        );
    }

    private static XElement BuildHolidayDetails()
    {
        return new XElement(_ns + "Holiday",
            new XElement(_ns + "HolidayYearStart", "2020-01-01"),
            new XElement(_ns + "HolidayContractedEntitlementDays", "30"),
            new XElement(_ns + "HolidayDaysCarriedForward", "10"),
            new XElement(_ns + "HolidayDaysTaken", "2"),
            new XElement(_ns + "HolidayNotPaid",
                BuildHoliday(1, "2020-01-20", "2020-01-21"),
                BuildHoliday(2, "2020-01-22", "2020-01-23"),
                BuildHoliday(3, "2020-01-24", "2020-01-25")
            ),
            new XElement(_ns + "NoDaysHolidayOwed", "10")
        );
    }

    private static XElement BuildArrearsOfPayPeriod(
        int periodNumber,
        string startDate,
        string endDate,
        string amountOwed,
        string payType)
    {
        return new XElement(_ns + $"ArrearsOfPayPeriod{periodNumber}",
            new XElement(_ns + $"AOP{periodNumber}StartDate", startDate),
            new XElement(_ns + $"AOP{periodNumber}EndDate", endDate),
            new XElement(_ns + $"AOPOwed{periodNumber}", amountOwed),
            new XElement(_ns + $"AOPPayType{periodNumber}", payType)
        );
    }

    private static XElement BuildHoliday(
        int holidayNumber,
        string startDate,
        string endDate)
    {
        return new XElement(_ns + $"Holiday{holidayNumber}",
            new XElement(_ns + $"Holiday{holidayNumber}StartDate", startDate),
            new XElement(_ns + $"Holiday{holidayNumber}EndDate", endDate)
        );
    }

    private readonly List<ArrearsOfPayPeriod> _arrearsOfPayPeriods =
[
           new(1, "2020-01-10", "2020-01-11", "100", "attachmentofearnings"),
           new(2, "2020-01-12", "2020-01-13", "100", "bouncedcheque"),
           new(3, "2020-01-14", "2020-01-15", "100", "commission"),
           new(4, "2020-01-16", "2020-01-17", "100", "overtime")
];

    public Rp14aBuilder WithArrearsOfPayPeriod(
    int periodNumber,
    string startDate,
    string endDate,
    string amountOwed,
    string payType)
    {
        _arrearsOfPayPeriods.RemoveAll(x => x.PeriodNumber == periodNumber);

        _arrearsOfPayPeriods.Add(new ArrearsOfPayPeriod(
            periodNumber,
            startDate,
            endDate,
            amountOwed,
            payType));

        return this;
    }

    public Rp14aBuilder WithNoArrearsOfPay()
    {
        _arrearsOfPayPeriods.Clear();
        return this;
    }
}