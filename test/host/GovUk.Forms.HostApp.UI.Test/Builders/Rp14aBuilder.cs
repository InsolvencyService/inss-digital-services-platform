using GovUk.Forms.HostApp.UI.Test.Support;
using System.Xml.Linq;

namespace GovUk.Forms.HostApp.UI.Test.Builders;

public sealed class Rp14aBuilder
{
    private const string NamespaceUri = "http://www.ins.gsi.gov.uk/FileUpload/RP14A_Application";

    private string _caseReference = "CN10000112";
    private string _nationalInsuranceNumber = ScenarioConstant.NationalInsuranceNumber;
    private string _employerName = ScenarioConstant.EmployerName;
    private string _surname = ScenarioConstant.Surname;
    private string _forenames = ScenarioConstant.Forname;
    private string _title = ScenarioConstant.Title;
    private string _dateOfBirth = ScenarioConstant.DOB;

    public Rp14aBuilder WithNationalInsuranceNumber(string nationalInsuranceNumber)
    {
        _nationalInsuranceNumber = nationalInsuranceNumber;
        return this;
    }

    public Rp14aBuilder WithCaseReference(string caseReference)
    {
        _caseReference = caseReference;
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

    public string Build()
    {
        XNamespace ns = NamespaceUri;

        XDocument document = new(
            new XElement(ns + "RP14A",
                new XElement(ns + "Employee",
                    new XElement(ns + "Header",
                        new XElement(ns + "CaseReference", _caseReference)
                    ),
                    new XElement(ns + "EmployerName", _employerName),
                    new XElement(ns + "EmployeeName",
                        new XElement(ns + "Surname", _surname),
                        new XElement(ns + "Forenames", _forenames),
                        new XElement(ns + "Title", _title)
                    ),
                    new XElement(ns + "NIClass", "A"),
                    new XElement(ns + "NINO", _nationalInsuranceNumber),
                    new XElement(ns + "DateOfBirth", _dateOfBirth),
                    new XElement(ns + "StartDate", "2019-01-01"),
                    new XElement(ns + "DateNoticeGiven", "2020-01-01"),
                    new XElement(ns + "EndDate", "2020-03-01"),
                    new XElement(ns + "IsDirector", "No"),
                    new XElement(ns + "AverageHoursWorked", "37"),
                    new XElement(ns + "MoneyOwedToEmployer", "1000"),
                    new XElement(ns + "EntitledToRedundancyPay", "Yes"),
                    new XElement(ns + "EntitledToNoticePay", "Yes"),
                    new XElement(ns + "PayDetails",
                        new XElement(ns + "BasicPayPerWeek", "1000"),
                        new XElement(ns + "ComponentPayPerWeek1",
                            new XElement(ns + "ComponentType", "Holiday Pay Accrued"),
                            new XElement(ns + "ComponentRate", "1000"),
                            new XElement(ns + "ComponentRateStatus", string.Empty)
                        ),
                        new XElement(ns + "ComponentPayPerWeek2",
                            new XElement(ns + "ComponentType", "Holiday Pay Taken Not Paid"),
                            new XElement(ns + "ComponentRateStatus", "Fixed rate of pay")
                        ),
                        new XElement(ns + "WeeklyPayDay", "Monday"),
                        new XElement(ns + "ArrearsOfPay",
                            BuildArrearsOfPayPeriod(ns, 1, "2020-01-10", "2020-01-11", "100", "attachmentofearnings"),
                            BuildArrearsOfPayPeriod(ns, 2, "2020-01-12", "2020-01-13", "100", "bouncedcheque"),
                            BuildArrearsOfPayPeriod(ns, 3, "2020-01-14", "2020-01-15", "100", "commission"),
                            BuildArrearsOfPayPeriod(ns, 4, "2020-01-16", "2020-01-17", "100", "overtime")
                        )
                    ),
                    new XElement(ns + "Holiday",
                        new XElement(ns + "HolidayYearStart", "2020-01-01"),
                        new XElement(ns + "HolidayContractedEntitlementDays", "30"),
                        new XElement(ns + "HolidayDaysCarriedForward", "10"),
                        new XElement(ns + "HolidayDaysTaken", "2"),
                        new XElement(ns + "HolidayNotPaid",
                            BuildHoliday(ns, 1, "2020-01-20", "2020-01-21"),
                            BuildHoliday(ns, 2, "2020-01-22", "2020-01-23"),
                            BuildHoliday(ns, 3, "2020-01-24", "2020-01-25")
                        ),
                        new XElement(ns + "NoDaysHolidayOwed", "10")
                    )
                )
            )
        );

        return document.ToString(SaveOptions.DisableFormatting);
    }

    public string BuildToFile(string directoryPath, string? fileName = null)
    {
        Directory.CreateDirectory(directoryPath);

        fileName ??= $"rp14a-{Guid.NewGuid():N}.xml";

        string filePath = Path.Combine(directoryPath, fileName);

        File.WriteAllText(filePath, Build());

        return filePath;
    }

    private static XElement BuildArrearsOfPayPeriod(
        XNamespace ns,
        int periodNumber,
        string startDate,
        string endDate,
        string amountOwed,
        string payType)
    {
        return new XElement(ns + $"ArrearsOfPayPeriod{periodNumber}",
            new XElement(ns + $"AOP{periodNumber}StartDate", startDate),
            new XElement(ns + $"AOP{periodNumber}EndDate", endDate),
            new XElement(ns + $"AOPOwed{periodNumber}", amountOwed),
            new XElement(ns + $"AOPPayType{periodNumber}", payType)
        );
    }

    private static XElement BuildHoliday(
        XNamespace ns,
        int holidayNumber,
        string startDate,
        string endDate)
    {
        return new XElement(ns + $"Holiday{holidayNumber}",
            new XElement(ns + $"Holiday{holidayNumber}StartDate", startDate),
            new XElement(ns + $"Holiday{holidayNumber}EndDate", endDate)
        );
    }
}
