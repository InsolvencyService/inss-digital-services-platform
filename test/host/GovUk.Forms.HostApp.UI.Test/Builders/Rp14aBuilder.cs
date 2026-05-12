using GovUk.Forms.HostApp.UI.Test.Support;
using System.Xml.Linq;
using static GovUk.Forms.HostApp.UI.Test.Support.Rp14aEmployee;

namespace GovUk.Forms.HostApp.UI.Test.Builders;

/// <summary>
/// Builds RP14A XML files for upload tests.
/// 
/// This builder creates a valid default RP14A document and allows
/// individual fields to be overridden for validation scenarios.
/// </summary>
public sealed class Rp14aBuilder
{
    private const string NamespaceUri =
        "http://www.ins.gsi.gov.uk/FileUpload/RP14A_Application";

    private static readonly XNamespace _ns = NamespaceUri;

    private readonly List<Rp14aEmployee> _employees = [];

    public Rp14aBuilder()
    {
        _employees.Add(Default());
    }

    public Rp14aBuilder WithNoEmployees()
    {
        _employees.Clear();
        return this;
    }

    public Rp14aBuilder WithEmployee(Rp14aEmployee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);

        _employees.Add(employee);
        return this;
    }

    public Rp14aBuilder WithEmployees(params Rp14aEmployee[] employees)
    {
        ArgumentNullException.ThrowIfNull(employees);

        _employees.Clear();
        _employees.AddRange(employees);

        return this;
    }

    public string Build()
    {
        XDocument document = new(
            new XDeclaration("1.0", "UTF-8", "yes"),
            new XElement(_ns + "RP14A",
                new XAttribute(XNamespace.Xmlns + "ns1", _ns),
                _employees.Select(BuildEmployee)
            )
        );

        return document.ToString(SaveOptions.None);
    }

    private static XElement BuildEmployee(Rp14aEmployee employee)
    {
        return new XElement(_ns + "Employee",
            new XElement(_ns + "Header",
                new XElement(_ns + "CaseReference", employee.CaseReference)
            ),
            new XElement(_ns + "EmployerName", employee.EmployerName),
            new XElement(_ns + "EmployeeName",
                new XElement(_ns + "Surname", employee.Surname),
                new XElement(_ns + "Forenames", employee.Forenames),
                new XElement(_ns + "Title", employee.Title)
            ),
            new XElement(_ns + "NIClass", employee.NIClass),
            new XElement(_ns + "NINO", employee.NationalInsuranceNumber),
            new XElement(_ns + "DateOfBirth", employee.DateOfBirth),
            new XElement(_ns + "StartDate", employee.StartDate),
            new XElement(_ns + "DateNoticeGiven", employee.DateNoticeGiven),
            new XElement(_ns + "EndDate", employee.EndDate),
            new XElement(_ns + "IsDirector", employee.IsDirector),
            new XElement(_ns + "AverageHoursWorked", employee.AverageHoursWorked),
            new XElement(_ns + "MoneyOwedToEmployer", employee.MoneyOwedToEmployer),
            new XElement(_ns + "EntitledToRedundancyPay", employee.EntitledToRedundancyPay),
            new XElement(_ns + "EntitledToNoticePay", employee.EntitledToNoticePay),
            BuildPayDetails(employee),
            BuildHolidayDetails(employee)
        );
    }

    private static XElement BuildPayDetails(Rp14aEmployee employee)
    {
        return new XElement(_ns + "PayDetails",
            new XElement(_ns + "BasicPayPerWeek", employee.BasicPayPerWeek),
            new XElement(_ns + "ComponentPayPerWeek1",
                new XElement(_ns + "ComponentType", employee.Component1Type),
                new XElement(_ns + "ComponentRate", employee.Component1Rate),
                new XElement(_ns + "ComponentRateStatus", employee.Component1RateStatus)
            ),
            new XElement(_ns + "ComponentPayPerWeek2",
                new XElement(_ns + "ComponentType", employee.Component2Type),
                new XElement(_ns + "ComponentRateStatus", employee.Component2RateStatus)
            ),
            new XElement(_ns + "WeeklyPayDay", employee.WeeklyPayDay),
            new XElement(_ns + "ArrearsOfPay",
                employee.ArrearsOfPayPeriods
                    .OrderBy(x => x.PeriodNumber)
                    .Select(BuildArrearsOfPayPeriod)
            )
        );
    }

    private static XElement BuildArrearsOfPayPeriod(
        ArrearsOfPayPeriod period)
    {
        return new XElement(_ns + $"ArrearsOfPayPeriod{period.PeriodNumber}",
            new XElement(_ns + $"AOP{period.PeriodNumber}StartDate", period.StartDate),
            new XElement(_ns + $"AOP{period.PeriodNumber}EndDate", period.EndDate),
            new XElement(_ns + $"AOPOwed{period.PeriodNumber}", period.AmountOwed),
            new XElement(_ns + $"AOPPayType{period.PeriodNumber}", period.PayType)
        );
    }

    private static XElement BuildHolidayDetails(Rp14aEmployee employee)
    {
        return new XElement(_ns + "Holiday",
            new XElement(_ns + "HolidayYearStart", employee.HolidayYearStart),
            new XElement(_ns + "HolidayContractedEntitlementDays", employee.HolidayContractedEntitlementDays),
            new XElement(_ns + "HolidayDaysCarriedForward", employee.HolidayDaysCarriedForward),
            new XElement(_ns + "HolidayDaysTaken", employee.HolidayDaysTaken),
            new XElement(_ns + "HolidayNotPaid",
                employee.HolidaysNotPaid
                    .OrderBy(x => x.HolidayNumber)
                    .Select(BuildHoliday)
            ),
            new XElement(_ns + "NoDaysHolidayOwed", employee.NoDaysHolidayOwed)
        );
    }

    private static XElement BuildHoliday(HolidayPeriod holiday)
    {
        return new XElement(_ns + $"Holiday{holiday.HolidayNumber}",
            new XElement(_ns + $"Holiday{holiday.HolidayNumber}StartDate", holiday.StartDate),
            new XElement(_ns + $"Holiday{holiday.HolidayNumber}EndDate", holiday.EndDate)
        );
    }
}