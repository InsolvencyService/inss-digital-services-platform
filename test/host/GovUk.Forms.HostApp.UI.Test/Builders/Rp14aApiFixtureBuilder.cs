using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;
using Inss.Common.IPUpload.Employee.Api;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GovUk.Forms.HostApp.UI.Test.Builders;

public sealed class Rp14aApiFixtureBuilder : IRp14aFixtureBuilder
{
    private const string Namespace = "www.inss.gsi.gov.uk/RP14A_Application";
    private const string NsPrefix = "ns1";

    private static readonly XmlSerializer _serializer = new(typeof(RP14A));
    private static readonly XmlSerializerNamespaces _serializerNamespaces = CreateSerializerNamespaces();

    private readonly Dictionary<string, string?> _mutations = [];
    private readonly Dictionary<int, Dictionary<string, string?>> _employeeMutations = [];
    private readonly Dictionary<int, string?> _aopAmountMutations = [];
    private readonly Dictionary<int, (string? StartDate, string? EndDate)> _holidayNotPaidDatesPerEmployee = [];
    private readonly Dictionary<int, (string? StartDate, string? EndDate)> _aopDatesPerEmployee = [];
    private string? _aopStartDate;
    private string? _aopEndDate;
    private string? _holidayNotPaidStartDate;
    private string? _holidayNotPaidEndDate;

    private int _targetEmployeeIndex;

    public IRp14aFixtureBuilder WithEmployeeIndex(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        _targetEmployeeIndex = index;
        return this;
    }

    public IRp14aFixtureBuilder WithCaseReference(string? value) =>
        Set(RP14AElementNames.CaseReference, value);

    public IRp14aFixtureBuilder WithEmployerName(string? value) =>
        Set(RP14AElementNames.EmployerName, value);

    public IRp14aFixtureBuilder WithEmployeeBasicPayPerWeek(string? value) =>
        Set(RP14AElementNames.BasicPayPerWeek, value);

    public IRp14aFixtureBuilder WithMoneyOwedToEmployer(string? value) =>
        Set(RP14AElementNames.MoneyOwedToEmployer, value);

    public IRp14aFixtureBuilder WithHolidayContractedEntitlementDays(string? value) =>
        Set(RP14AElementNames.HolidayContractedEntitlementDays, value, nullMeansEmpty: false);

    public IRp14aFixtureBuilder WithHolidayDaysCarriedForward(string? value) =>
        Set(RP14AElementNames.HolidayDaysCarriedForward, value, nullMeansEmpty: false);

    public IRp14aFixtureBuilder WithHolidayDaysTaken(string? value) =>
        Set(RP14AElementNames.HolidayDaysTaken, value, nullMeansEmpty: false);

    public IRp14aFixtureBuilder WithHolidayOwed(string? value) =>
        Set(RP14AElementNames.NoDaysHolidayOwed, value, nullMeansEmpty: false);

    public IRp14aFixtureBuilder WithEmployeeName(
        string? surname,
        string? forename,
        string? title)
    {
        return Set(RP14AElementNames.Surname, surname)
            .Set(RP14AElementNames.Forenames, forename)
            .Set(RP14AElementNames.Title, title);
    }

    public IRp14aFixtureBuilder WithEmploymentDates(DateOnly? startDate, DateOnly? endDate)
    {
        return Set(RP14AElementNames.StartDate, FormatDate(startDate), nullMeansEmpty: false)
            .Set(RP14AElementNames.EndDate, FormatDate(endDate), nullMeansEmpty: false);
    }

    public IRp14aFixtureBuilder WithArrearsDates(DateOnly? startDate, DateOnly? endDate)
    {
        if (startDate.HasValue)
        {
            _aopStartDate = FormatDate(startDate);
        }

        if (endDate.HasValue)
        {
            _aopEndDate = FormatDate(endDate);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithHolidayNotPaidDates(DateOnly? startDate, DateOnly? endDate)
    {
        if (startDate.HasValue)
        {
            _holidayNotPaidStartDate = FormatDate(startDate);
        }

        if (endDate.HasValue)
        {
            _holidayNotPaidEndDate = FormatDate(endDate);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithArrearsAmount(int periodNumber, string? amountOwed)
    {
        ValidatePositiveNumber(periodNumber, nameof(periodNumber));
        _aopAmountMutations[periodNumber] = amountOwed ?? string.Empty;
        return this;
    }

    public IRp14aFixtureBuilder WithInvalidHolidayOwedForEmployees(
        int employeeCount,
        params string[] invalidValues)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        if (invalidValues.Length == 0)
        {
            throw new ArgumentException("At least one invalid value must be provided.", nameof(invalidValues));
        }

        for (int i = 0; i < employeeCount; i++)
        {
            AddEmployeeMutation(i, RP14AElementNames.NoDaysHolidayOwed, invalidValues[i % invalidValues.Length]);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithMissingSurnames(int employeeCount)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        for (int i = 0; i < employeeCount; i++)
        {
            AddEmployeeMutation(i, RP14AElementNames.Surname, string.Empty);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithSurnamesForEmployees(int employeeCount, string? surname)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        for (int i = 0; i < employeeCount; i++)
        {
            AddEmployeeMutation(i, RP14AElementNames.Surname, surname ?? string.Empty);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithCustomMutation(string elementName, string? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(elementName);
        _mutations[elementName] = value;
        return this;
    }

    public IRp14aFixtureBuilder WithCaseReferences(params string?[] caseReferences)
    {
        if (caseReferences is null || caseReferences.Length == 0)
        {
            throw new ArgumentException("At least one case reference must be provided.", nameof(caseReferences));
        }

        // API model has a single global CaseReference at the root; set it to the first value.
        _mutations[RP14AElementNames.CaseReference] = caseReferences[0] ?? string.Empty;

        return this;
    }

    public IRp14aFixtureBuilder WithNinosForEmployees(params string?[] ninos)
    {
        if (ninos is null || ninos.Length == 0)
        {
            throw new ArgumentException("At least one NINO must be provided.", nameof(ninos));
        }

        for (int i = 0; i < ninos.Length; i++)
        {
            AddEmployeeMutation(i, RP14AElementNames.NINO, ninos[i] ?? string.Empty);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithEmployerNames(params string?[] employerNames)
    {
        if (employerNames is null || employerNames.Length == 0)
        {
            throw new ArgumentException("At least one employer name must be provided.", nameof(employerNames));
        }

        // API model has a single global EmployerName at the root; set it to the first value.
        _mutations[RP14AElementNames.EmployerName] = employerNames[0] ?? string.Empty;

        return this;
    }

    public IRp14aFixtureBuilder WithNationalInsuranceNumberForEmployees(
        int employeeCount,
        string? nationalInsuranceNumber)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        for (int i = 0; i < employeeCount; i++)
        {
            AddEmployeeMutation(i, RP14AElementNames.NINO, nationalInsuranceNumber ?? string.Empty);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithMoneyOwedToEmployerForEmployees(int employeeCount, string? moneyOwed)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        for (int i = 0; i < employeeCount; i++)
        {
            AddEmployeeMutation(i, RP14AElementNames.MoneyOwedToEmployer, moneyOwed ?? string.Empty);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithEmployeeBasicPayPerWeekForEmployees(int employeeCount, string? basicPayPerWeek)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        for (int i = 0; i < employeeCount; i++)
        {
            AddEmployeeMutation(i, RP14AElementNames.BasicPayPerWeek, basicPayPerWeek ?? string.Empty);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithHolidayDaysTakenForEmployees(int employeeCount, string? holidayDaysTaken)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        for (int i = 0; i < employeeCount; i++)
        {
            AddEmployeeMutation(i, RP14AElementNames.HolidayDaysTaken, holidayDaysTaken ?? string.Empty);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithHolidayDaysCarriedForwardForEmployees(
        int employeeCount,
        string? holidayDaysCarriedForward)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        for (int i = 0; i < employeeCount; i++)
        {
            AddEmployeeMutation(i, RP14AElementNames.HolidayDaysCarriedForward, holidayDaysCarriedForward ?? string.Empty);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithHolidayContractedEntitlementDaysForEmployees(int employeeCount, string? value)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        for (int i = 0; i < employeeCount; i++)
        {
            AddEmployeeMutation(i, RP14AElementNames.HolidayContractedEntitlementDays, value ?? string.Empty);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithHolidayNotPaidDatesForEmployees(int employeeCount, DateOnly? startDate, DateOnly? endDate)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        string? formattedStart = startDate.HasValue ? FormatDate(startDate) : null;
        string? formattedEnd = endDate.HasValue ? FormatDate(endDate) : null;

        for (int i = 0; i < employeeCount; i++)
        {
            _holidayNotPaidDatesPerEmployee[i] = (formattedStart, formattedEnd);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithEmploymentDatesForEmployees(int employeeCount, DateOnly? startDate, DateOnly? endDate)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        string? formattedStart = startDate.HasValue ? FormatDate(startDate) : null;
        string? formattedEnd = endDate.HasValue ? FormatDate(endDate) : null;

        for (int i = 0; i < employeeCount; i++)
        {
            AddEmployeeMutation(i, RP14AElementNames.StartDate, formattedStart);
            AddEmployeeMutation(i, RP14AElementNames.EndDate, formattedEnd);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithArrearsDatesForEmployees(int employeeCount, DateOnly? startDate, DateOnly? endDate)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        string? formattedStart = startDate.HasValue ? FormatDate(startDate) : null;
        string? formattedEnd = endDate.HasValue ? FormatDate(endDate) : null;

        for (int i = 0; i < employeeCount; i++)
        {
            _aopDatesPerEmployee[i] = (formattedStart, formattedEnd);
        }

        return this;
    }

    public Rp14aTestFile Build(TestArtifacts testArtifacts, string? baselineFilePath = null, string scenarioName = "Default")
    {
        ArgumentNullException.ThrowIfNull(testArtifacts);

        string filePath = testArtifacts.FilePath($"rp14a-api-{Guid.NewGuid():N}.xml");

        XDocument document = SerializeToDocument();
        XNamespace ns = Namespace;

        XElement targetEmployee = GetEmployeeByIndex(document, ns, _targetEmployeeIndex);

        foreach ((string elementName, string? value) in _mutations)
        {
            ApplyMutation(document, ns, targetEmployee, elementName, value);
        }

        foreach ((int employeeIndex, Dictionary<string, string?> mutations) in _employeeMutations)
        {
            XElement employee = GetEmployeeByIndex(document, ns, employeeIndex);
            foreach ((string elementName, string? value) in mutations)
            {
                ApplyMutation(document, ns, employee, elementName, value);
            }
        }

        ApplyAopAmountMutations(targetEmployee, ns);
        ApplyAopDateMutations(targetEmployee, ns);
        ApplyHolidayNotPaidDateMutations(targetEmployee, ns);

        foreach ((int empIndex, (string? StartDate, string? EndDate) dates) in _holidayNotPaidDatesPerEmployee)
        {
            XElement employee = GetEmployeeByIndex(document, ns, empIndex);
            ApplyHolidayNotPaidDatesToEmployee(employee, ns, dates.StartDate, dates.EndDate);
        }

        foreach ((int empIndex, (string? StartDate, string? EndDate) dates) in _aopDatesPerEmployee)
        {
            XElement employee = GetEmployeeByIndex(document, ns, empIndex);
            ApplyAopDatesToEmployee(employee, ns, dates.StartDate, dates.EndDate);
        }

        XmlFixtureHelper.SaveXmlDocument(document, filePath);

        Dictionary<string, string> appliedMutations = _mutations
            .Where(m => m.Value is not null)
            .ToDictionary(m => m.Key, m => m.Value!);

        foreach ((int employeeIndex, Dictionary<string, string?> mutations) in _employeeMutations)
        {
            foreach ((string elementName, string? value) in mutations.Where(m => m.Value is not null))
            {
                appliedMutations[$"[employee:{employeeIndex}] {elementName}"] = value!;
            }
        }

        foreach ((int period, string? amount) in _aopAmountMutations.Where(m => m.Value is not null))
        {
            appliedMutations[$"AOPOwed[period:{period}]"] = amount!;
        }

        if (_aopStartDate is not null)
        {
            appliedMutations["AopPeriodStartDate"] = _aopStartDate;
        }

        if (_aopEndDate is not null)
        {
            appliedMutations["AopPeriodEndDate"] = _aopEndDate;
        }

        if (_holidayNotPaidStartDate is not null)
        {
            appliedMutations["HolidayNotPaidStartDate"] = _holidayNotPaidStartDate;
        }

        if (_holidayNotPaidEndDate is not null)
        {
            appliedMutations["HolidayNotPaidEndDate"] = _holidayNotPaidEndDate;
        }

        LogInfo($"Scenario '{scenarioName}': Fixture created at {filePath}");

        return new Rp14aTestFile(filePath, appliedMutations, _targetEmployeeIndex, DateTime.UtcNow);
    }

    private Rp14aApiFixtureBuilder Set(string elementName, string? value, bool nullMeansEmpty = true)
    {
        _mutations[elementName] = nullMeansEmpty ? value ?? string.Empty : value;
        return this;
    }

    private void AddEmployeeMutation(int employeeIndex, string elementName, string? value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(employeeIndex);

        if (!_employeeMutations.TryGetValue(employeeIndex, out Dictionary<string, string?>? mutations))
        {
            mutations = [];
            _employeeMutations[employeeIndex] = mutations;
        }

        mutations[elementName] = value;
    }

    private void ApplyAopAmountMutations(XElement employee, XNamespace ns)
    {
        List<XElement> periods = employee
            .Descendants(ns + "ArrearsOfPayPeriod")
            .ToList();

        foreach ((int periodNumber, string? amount) in _aopAmountMutations)
        {
            int periodIndex = periodNumber - 1;
            if (periodIndex < 0 || periodIndex >= periods.Count) { continue; }

            XElement? aopOwed = periods[periodIndex].Element(ns + "AOPOwed");
            if (aopOwed is null) { continue; }

            if (amount is null)
            {
                aopOwed.Remove();
            }
            else
            {
                aopOwed.Value = amount;
            }
        }
    }

    private void ApplyAopDateMutations(XElement employee, XNamespace ns)
    {
        ApplyAopDatesToEmployee(employee, ns, _aopStartDate, _aopEndDate);
    }

    private static void ApplyAopDatesToEmployee(XElement employee, XNamespace ns, string? startDate, string? endDate)
    {
        if (startDate is null && endDate is null)
        {
            return;
        }

        XElement? period = employee
            .Descendants(ns + "ArrearsOfPayPeriod")
            .FirstOrDefault()
            ?.Element(ns + "Period");

        if (period is null)
        {
            return;
        }

        if (startDate is not null)
        {
            period.Element(ns + "StartDate")?.SetValue(startDate);
        }

        if (endDate is not null)
        {
            period.Element(ns + "EndDate")?.SetValue(endDate);
        }
    }

    private void ApplyHolidayNotPaidDateMutations(XElement employee, XNamespace ns)
    {
        ApplyHolidayNotPaidDatesToEmployee(employee, ns, _holidayNotPaidStartDate, _holidayNotPaidEndDate);
    }

    private static void ApplyHolidayNotPaidDatesToEmployee(XElement employee, XNamespace ns, string? startDate, string? endDate)
    {
        if (startDate is null && endDate is null)
        {
            return;
        }

        XElement? holiday = employee
            .Descendants(ns + "HolidayNotPaid")
            .FirstOrDefault()
            ?.Element(ns + "Holiday");

        if (holiday is null)
        {
            return;
        }

        if (startDate is not null)
        {
            holiday.Element(ns + "StartDate")?.SetValue(startDate);
        }

        if (endDate is not null)
        {
            holiday.Element(ns + "EndDate")?.SetValue(endDate);
        }
    }

    private void ApplyMutation(
        XDocument document,
        XNamespace ns,
        XElement targetEmployee,
        string elementName,
        string? value)
    {
        XElement element = targetEmployee.Descendants(ns + elementName).FirstOrDefault()
            ?? document.Descendants(ns + elementName).FirstOrDefault()
            ?? throw new InvalidOperationException(
                $"Element '{elementName}' not found in RP14A XML for employee index {_targetEmployeeIndex}.");

        if (value is null)
        {
            LogInfo($"Removing element: {elementName}");
            element.Remove();
            return;
        }

        element.Value = value;
        LogInfo($"Applied mutation: {elementName} = '{value}'");
    }

    private static XElement GetEmployeeByIndex(XDocument document, XNamespace ns, int index)
    {
        List<XElement> employees = document
            .Descendants(ns + RP14AElementNames.Employee)
            .ToList();

        if (employees.Count == 0)
        {
            throw new InvalidOperationException("No Employee elements found in RP14A XML.");
        }

        if (index < 0 || index >= employees.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(index),
                $"Employee index {index} is out of range. Found {employees.Count} employees.");
        }

        return employees[index];
    }

    private static XDocument SerializeToDocument()
    {
        RP14A model = CreateDefault();

        XmlWriterSettings settings = new()
        {
            Indent = true,
            Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)
        };

        using StringWriter sw = new();
        using XmlWriter writer = XmlWriter.Create(sw, settings);
        _serializer.Serialize(writer, model, _serializerNamespaces);

        return XDocument.Parse(sw.ToString());
    }

    private static XmlSerializerNamespaces CreateSerializerNamespaces()
    {
        XmlSerializerNamespaces ns = new();
        ns.Add(NsPrefix, Namespace);
        return ns;
    }

    private static RP14A CreateDefault() => new()
    {
        Header = new RP14AHeader { CaseReference = "CN00345678" },
        EmployerName = "Employer Test",
        Employee =
        [
            CreateEmployee(
                surname: "Surname Test",
                forenames: "Forname Test",
                title: "Mr",
                niClass: "A",
                nino: "BP011752C",
                dateOfBirth: new DateTime(1990, 1, 1),
                startDate: new DateTime(2000, 1, 1),
                dateNoticeGiven: new DateTime(2020, 1, 1),
                endDate: new DateTime(2020, 3, 1),
                isDirector: YesNoType.No,
                averageHoursWorked: 37m,
                moneyOwedToEmployer: 5000m,
                entitledToRedundancyPay: YesNoType.Yes,
                entitledToNoticePay: YesNoType.Yes,
                basicPayPerWeek: 1000m,
                weeklyPayDay: RP14AEmployeePayDetailsWeeklyPayDay.Monday,
                holidayYearStart: new DateTime(2020, 1, 1),
                aopStartDay: 10),
            CreateEmployee(
                surname: "Surname Test 2",
                forenames: "Forname Test 2",
                title: "Doctor",
                niClass: "A",
                nino: "JH787823C",
                dateOfBirth: new DateTime(1959, 8, 2),
                startDate: new DateTime(2010, 10, 2),
                dateNoticeGiven: new DateTime(2020, 1, 2),
                endDate: new DateTime(2020, 3, 2),
                isDirector: YesNoType.No,
                averageHoursWorked: 37m,
                moneyOwedToEmployer: 10000m,
                entitledToRedundancyPay: YesNoType.No,
                entitledToNoticePay: YesNoType.No,
                basicPayPerWeek: 1500m,
                weeklyPayDay: RP14AEmployeePayDetailsWeeklyPayDay.Tuesday,
                holidayYearStart: new DateTime(2020, 1, 2),
                aopStartDay: 11),
            CreateEmployee(
                surname: "Surname Test 3",
                forenames: "Forname Test 3",
                title: "Miss",
                niClass: "A",
                nino: "JH787823C",
                dateOfBirth: new DateTime(1980, 11, 3),
                startDate: new DateTime(2019, 1, 3),
                dateNoticeGiven: new DateTime(2020, 1, 3),
                endDate: new DateTime(2020, 3, 3),
                isDirector: YesNoType.Yes,
                averageHoursWorked: 37m,
                moneyOwedToEmployer: 2000m,
                entitledToRedundancyPay: YesNoType.No,
                entitledToNoticePay: YesNoType.No,
                basicPayPerWeek: 2500m,
                weeklyPayDay: RP14AEmployeePayDetailsWeeklyPayDay.Wednesday,
                holidayYearStart: new DateTime(2020, 1, 3),
                aopStartDay: 12),
            CreateEmployee(
                surname: "Surname Test 4",
                forenames: "Forname Test 4",
                title: "Mrs",
                niClass: "A",
                nino: "JH787823C",
                dateOfBirth: new DateTime(2000, 1, 14),
                startDate: new DateTime(2019, 10, 24),
                dateNoticeGiven: new DateTime(2020, 1, 4),
                endDate: new DateTime(2020, 3, 4),
                isDirector: YesNoType.Yes,
                averageHoursWorked: 37m,
                moneyOwedToEmployer: 5000m,
                entitledToRedundancyPay: YesNoType.Yes,
                entitledToNoticePay: YesNoType.Yes,
                basicPayPerWeek: 800m,
                weeklyPayDay: RP14AEmployeePayDetailsWeeklyPayDay.Thursday,
                holidayYearStart: new DateTime(2020, 1, 4),
                aopStartDay: 13),
            CreateEmployee(
                surname: "Surname Test 5",
                forenames: "Forname Test 5",
                title: "Other",
                niClass: "A",
                nino: "JH787823C",
                dateOfBirth: new DateTime(1991, 1, 15),
                startDate: new DateTime(2016, 1, 5),
                dateNoticeGiven: new DateTime(2020, 1, 5),
                endDate: new DateTime(2020, 3, 5),
                isDirector: YesNoType.Yes,
                averageHoursWorked: 37m,
                moneyOwedToEmployer: 1000m,
                entitledToRedundancyPay: YesNoType.Yes,
                entitledToNoticePay: YesNoType.Yes,
                basicPayPerWeek: 1200m,
                weeklyPayDay: RP14AEmployeePayDetailsWeeklyPayDay.Friday,
                holidayYearStart: new DateTime(2020, 1, 5),
                aopStartDay: 14),
        ]
    };

    private static RP14AEmployee CreateEmployee(
        string surname,
        string forenames,
        string title,
        string niClass,
        string nino,
        DateTime dateOfBirth,
        DateTime startDate,
        DateTime dateNoticeGiven,
        DateTime endDate,
        YesNoType isDirector,
        decimal averageHoursWorked,
        decimal moneyOwedToEmployer,
        YesNoType entitledToRedundancyPay,
        YesNoType entitledToNoticePay,
        decimal basicPayPerWeek,
        RP14AEmployeePayDetailsWeeklyPayDay weeklyPayDay,
        DateTime holidayYearStart,
        int aopStartDay)
    {
        return new RP14AEmployee
        {
            EmployeeName = new NameType { Surname = surname, Forenames = forenames, Title = title },
            NIClass = niClass,
            NINO = nino,
            DateOfBirth = dateOfBirth,
            DateOfBirthSpecified = true,
            StartDate = startDate,
            StartDateSpecified = true,
            DateNoticeGiven = dateNoticeGiven,
            DateNoticeGivenSpecified = true,
            EndDate = endDate,
            EndDateSpecified = true,
            IsDirector = isDirector,
            IsDirectorSpecified = true,
            AverageHoursWorked = averageHoursWorked,
            AverageHoursWorkedSpecified = true,
            MoneyOwedToEmployer = moneyOwedToEmployer,
            MoneyOwedToEmployerSpecified = true,
            EntitledToRedundancyPay = entitledToRedundancyPay,
            EntitledToRedundancyPaySpecified = true,
            EntitledToNoticePay = entitledToNoticePay,
            EntitledToNoticePaySpecified = true,
            PayDetails = new RP14AEmployeePayDetails
            {
                BasicPayPerWeek = basicPayPerWeek,
                BasicPayPerWeekSpecified = true,
                ComponentPayPerWeek =
                [
                    new RP14AEmployeePayDetailsComponentPayPerWeek
                    {
                        ComponentType = RP14AEmployeePayDetailsComponentPayPerWeekComponentType.HolidayPayAccrued,
                        ComponentRate = "1000",
                        ComponentRateSpecified = true,
                        ComponentRateStatus = RP14AEmployeePayDetailsComponentPayPerWeekComponentRateStatus.Default
                    },
                    new RP14AEmployeePayDetailsComponentPayPerWeek
                    {
                        ComponentType = RP14AEmployeePayDetailsComponentPayPerWeekComponentType.HolidayPayTakenNotPaid,
                        ComponentRateStatus = RP14AEmployeePayDetailsComponentPayPerWeekComponentRateStatus.Fixedrateofpay
                    }
                ],
                WeeklyPayDay = weeklyPayDay,
                WeeklyPayDaySpecified = true,
                ArrearsOfPay =
                [
                    new RP14AEmployeePayDetailsArrearsOfPayPeriod
                    {
                        Period = new PeriodType
                        {
                            StartDate = new DateTime(2020, 1, aopStartDay),
                            EndDate = new DateTime(2020, 1, aopStartDay + 1)
                        },
                        AOPOwed = 100m,
                        AOPOwedSpecified = true,
                        PayType = RP14AEmployeePayDetailsArrearsOfPayPeriodPayType.wages,
                        PayTypeSpecified = true
                    },
                    new RP14AEmployeePayDetailsArrearsOfPayPeriod
                    {
                        Period = new PeriodType
                        {
                            StartDate = new DateTime(2020, 1, aopStartDay + 2),
                            EndDate = new DateTime(2020, 1, aopStartDay + 3)
                        },
                        AOPOwed = 100m,
                        AOPOwedSpecified = true,
                        PayType = RP14AEmployeePayDetailsArrearsOfPayPeriodPayType.bouncedcheque,
                        PayTypeSpecified = true
                    },
                    new RP14AEmployeePayDetailsArrearsOfPayPeriod
                    {
                        Period = new PeriodType
                        {
                            StartDate = new DateTime(2020, 1, aopStartDay + 4),
                            EndDate = new DateTime(2020, 1, aopStartDay + 5)
                        },
                        AOPOwed = 100m,
                        AOPOwedSpecified = true,
                        PayType = RP14AEmployeePayDetailsArrearsOfPayPeriodPayType.commission,
                        PayTypeSpecified = true
                    },
                    new RP14AEmployeePayDetailsArrearsOfPayPeriod
                    {
                        Period = new PeriodType
                        {
                            StartDate = new DateTime(2020, 1, aopStartDay + 6),
                            EndDate = new DateTime(2020, 1, aopStartDay + 7)
                        },
                        AOPOwed = 100m,
                        AOPOwedSpecified = true,
                        PayType = RP14AEmployeePayDetailsArrearsOfPayPeriodPayType.overtime,
                        PayTypeSpecified = true
                    }
                ]
            },
            Holiday = new RP14AEmployeeHoliday
            {
                HolidayYearStart = holidayYearStart,
                HolidayYearStartSpecified = true,
                HolidayContractedEntitlementDays = 30m,
                HolidayContractedEntitlementDaysSpecified = true,
                HolidayDaysCarriedForward = 10m,
                HolidayDaysCarriedForwardSpecified = true,
                HolidayDaysTaken = 2m,
                HolidayDaysTakenSpecified = true,
                HolidayNotPaid =
                [
                    new PeriodType
                    {
                        StartDate = new DateTime(2020, 1, aopStartDay + 10),
                        EndDate = new DateTime(2020, 1, aopStartDay + 11)
                    },
                    new PeriodType
                    {
                        StartDate = new DateTime(2020, 1, aopStartDay + 12),
                        EndDate = new DateTime(2020, 1, aopStartDay + 13)
                    },
                    new PeriodType
                    {
                        StartDate = new DateTime(2020, 1, aopStartDay + 14),
                        EndDate = new DateTime(2020, 1, aopStartDay + 15)
                    }
                ],
                NoDaysHolidayOwed = 10m,
                NoDaysHolidayOwedSpecified = true
            }
        };
    }

    private static string FormatDate(DateOnly? date) =>
        XmlFixtureHelper.FormatDate(date);

    private static void ValidatePositiveNumber(int value, string parameterName)
    {
        if (value <= 0)
        {
            throw new ArgumentException(
                $"'{parameterName}' must be greater than 0. Received: {value}",
                parameterName);
        }
    }

    private static void LogInfo(string message) =>
        XmlFixtureHelper.Log("RP14A-API", "INFO", message);
}
