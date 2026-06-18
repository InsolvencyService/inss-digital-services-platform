using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;
using Inss.Common.IPUpload.Employee.Spreadsheet;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GovUk.Forms.HostApp.UI.Test.Builders;

public sealed class Rp14aSpreadsheetFixtureBuilder : IRp14aFixtureBuilder
{
    private const string Namespace = "http://www.ins.gsi.gov.uk/FileUpload/RP14A_Application";
    private const string NsPrefix = "ns1";

    private static readonly XmlSerializer _serializer = new(typeof(RP14A));
    private static readonly XmlSerializerNamespaces _serializerNamespaces = CreateSerializerNamespaces();

    private readonly Dictionary<string, string?> _mutations = [];
    private readonly Dictionary<int, Dictionary<string, string?>> _employeeMutations = [];

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
        return Set(RP14AElementNames.AOP1StartDate, FormatDate(startDate), nullMeansEmpty: false)
            .Set(RP14AElementNames.AOP1EndDate, FormatDate(endDate), nullMeansEmpty: false);
    }

    public IRp14aFixtureBuilder WithHolidayNotPaidDates(DateOnly? startDate, DateOnly? endDate)
    {
        return Set(RP14AElementNames.Holiday1StartDate, FormatDate(startDate), nullMeansEmpty: false)
            .Set(RP14AElementNames.Holiday1EndDate, FormatDate(endDate), nullMeansEmpty: false);
    }

    public IRp14aFixtureBuilder WithArrearsAmount(int periodNumber, string? amountOwed)
    {
        ValidatePositiveNumber(periodNumber, nameof(periodNumber));

        return Set(
            RP14AElementNames.AOPOwedPeriod(periodNumber),
            amountOwed);
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

        for (int employeeIndex = 0; employeeIndex < employeeCount; employeeIndex++)
        {
            AddEmployeeMutation(
                employeeIndex,
                RP14AElementNames.NoDaysHolidayOwed,
                invalidValues[employeeIndex % invalidValues.Length]);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithMissingSurnames(int employeeCount)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        for (int index = 0; index < employeeCount; index++)
        {
            AddEmployeeMutation(
                index,
                RP14AElementNames.Surname,
                string.Empty);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithSurnamesForEmployees(int employeeCount, string? surname)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        for (int employeeIndex = 0; employeeIndex < employeeCount; employeeIndex++)
        {
            AddEmployeeMutation(employeeIndex, RP14AElementNames.Surname, surname ?? string.Empty);
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
            throw new ArgumentException(
                "At least one case reference must be provided.",
                nameof(caseReferences));
        }

        for (int employeeIndex = 0; employeeIndex < caseReferences.Length; employeeIndex++)
        {
            AddEmployeeMutation(
                employeeIndex,
                RP14AElementNames.CaseReference,
                caseReferences[employeeIndex] ?? string.Empty);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithNinosForEmployees(params string?[] ninos)
    {
        if (ninos is null || ninos.Length == 0)
        {
            throw new ArgumentException(
                "At least one NINO must be provided.",
                nameof(ninos));
        }

        for (int employeeIndex = 0; employeeIndex < ninos.Length; employeeIndex++)
        {
            AddEmployeeMutation(
                employeeIndex,
                RP14AElementNames.NINO,
                ninos[employeeIndex] ?? string.Empty);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithEmployerNames(params string?[] employerNames)
    {
        if (employerNames is null || employerNames.Length == 0)
        {
            throw new ArgumentException(
                "At least one employer name must be provided.",
                nameof(employerNames));
        }

        for (int employeeIndex = 0; employeeIndex < employerNames.Length; employeeIndex++)
        {
            AddEmployeeMutation(
                employeeIndex,
                RP14AElementNames.EmployerName,
                employerNames[employeeIndex] ?? string.Empty);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithNationalInsuranceNumberForEmployees(int employeeCount, string? nationalInsuranceNumber)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        for (int employeeIndex = 0; employeeIndex < employeeCount; employeeIndex++)
        {
            AddEmployeeMutation(
                employeeIndex,
                RP14AElementNames.NINO,
                nationalInsuranceNumber ?? string.Empty);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithMoneyOwedToEmployerForEmployees(int employeeCount, string? moneyOwed)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        for (int employeeIndex = 0; employeeIndex < employeeCount; employeeIndex++)
        {
            AddEmployeeMutation(
                employeeIndex,
                RP14AElementNames.MoneyOwedToEmployer,
                moneyOwed ?? string.Empty);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithEmployeeBasicPayPerWeekForEmployees(int employeeCount, string? basicPayPerWeek)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        for (int employeeIndex = 0; employeeIndex < employeeCount; employeeIndex++)
        {
            AddEmployeeMutation(
                employeeIndex,
                RP14AElementNames.BasicPayPerWeek,
                basicPayPerWeek ?? string.Empty);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithHolidayDaysTakenForEmployees(int employeeCount, string? holidayDaysTaken)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        for (int employeeIndex = 0; employeeIndex < employeeCount; employeeIndex++)
        {
            AddEmployeeMutation(
                employeeIndex,
                RP14AElementNames.HolidayDaysTaken,
                holidayDaysTaken ?? string.Empty);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithHolidayDaysCarriedForwardForEmployees(int employeeCount, string? holidayDaysCarriedForward)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        for (int employeeIndex = 0; employeeIndex < employeeCount; employeeIndex++)
        {
            AddEmployeeMutation(
                employeeIndex,
                RP14AElementNames.HolidayDaysCarriedForward,
                holidayDaysCarriedForward ?? string.Empty);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithHolidayContractedEntitlementDaysForEmployees(int employeeCount, string? value)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        for (int employeeIndex = 0; employeeIndex < employeeCount; employeeIndex++)
        {
            AddEmployeeMutation(
                employeeIndex,
                RP14AElementNames.HolidayContractedEntitlementDays,
                value ?? string.Empty);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithHolidayNotPaidDatesForEmployees(int employeeCount, DateOnly? startDate, DateOnly? endDate)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        string? formattedStart = FormatDate(startDate);
        string? formattedEnd = FormatDate(endDate);

        for (int employeeIndex = 0; employeeIndex < employeeCount; employeeIndex++)
        {
            AddEmployeeMutation(employeeIndex, RP14AElementNames.Holiday1StartDate, formattedStart);
            AddEmployeeMutation(employeeIndex, RP14AElementNames.Holiday1EndDate, formattedEnd);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithEmploymentDatesForEmployees(int employeeCount, DateOnly? startDate, DateOnly? endDate)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        string? formattedStart = FormatDate(startDate);
        string? formattedEnd = FormatDate(endDate);

        for (int employeeIndex = 0; employeeIndex < employeeCount; employeeIndex++)
        {
            AddEmployeeMutation(employeeIndex, RP14AElementNames.StartDate, formattedStart);
            AddEmployeeMutation(employeeIndex, RP14AElementNames.EndDate, formattedEnd);
        }

        return this;
    }

    public IRp14aFixtureBuilder WithArrearsDatesForEmployees(int employeeCount, DateOnly? startDate, DateOnly? endDate)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        string? formattedStart = FormatDate(startDate);
        string? formattedEnd = FormatDate(endDate);

        for (int employeeIndex = 0; employeeIndex < employeeCount; employeeIndex++)
        {
            AddEmployeeMutation(employeeIndex, RP14AElementNames.AOP1StartDate, formattedStart);
            AddEmployeeMutation(employeeIndex, RP14AElementNames.AOP1EndDate, formattedEnd);
        }

        return this;
    }

    public Rp14aTestFile Build(TestArtifacts testArtifacts, string? baselineFilePath = null, string scenarioName = "Default")
    {
        ArgumentNullException.ThrowIfNull(testArtifacts);

        LogInfo($"Scenario '{scenarioName}': Building fixture with {_mutations.Count} global mutations " +
                $"and {_employeeMutations.Count} employee mutation sets.");

        string filePath = testArtifacts.FilePath($"rp14a-{Guid.NewGuid():N}.xml");

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

        SaveXmlDocument(document, filePath);

        Dictionary<string, string> appliedMutations = _mutations
            .Where(mutation => mutation.Value is not null)
            .ToDictionary(
                mutation => mutation.Key,
                mutation => mutation.Value!);

        foreach ((int employeeIndex, Dictionary<string, string?> mutations) in _employeeMutations)
        {
            foreach ((string elementName, string? value) in mutations.Where(m => m.Value is not null))
            {
                appliedMutations[$"[employee:{employeeIndex}] {elementName}"] = value!;
            }
        }

        LogInfo($"Scenario '{scenarioName}': Fixture created successfully at {filePath}");

        return new Rp14aTestFile(filePath, appliedMutations, _targetEmployeeIndex, DateTime.UtcNow);
    }

    private Rp14aSpreadsheetFixtureBuilder Set(
        string elementName,
        string? value,
        bool nullMeansEmpty = true)
    {
        _mutations[elementName] = nullMeansEmpty
            ? value ?? string.Empty
            : value;

        return this;
    }

    private void ApplyMutation(
        XDocument document,
        XNamespace ns,
        XElement targetEmployee,
        string elementName,
        string? value)
    {
        XElement? element = targetEmployee
            .Descendants(ns + elementName)
            .FirstOrDefault();

        element ??= document
            .Descendants(ns + elementName)
            .FirstOrDefault();

        if (element is null)
        {
            throw new InvalidOperationException(
                $"Element '{elementName}' not found in RP14A XML for employee index {_targetEmployeeIndex}.");
        }

        if (value is null)
        {
            LogInfo($"Removing element: {elementName}");
            element.Remove();
            return;
        }

        element.Value = value;
        LogInfo($"Applied mutation: {elementName} = '{value}'");
    }

    private void AddEmployeeMutation(
        int employeeIndex,
        string elementName,
        string? value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(employeeIndex);

        if (!_employeeMutations.TryGetValue(employeeIndex, out Dictionary<string, string?>? mutations))
        {
            mutations = [];
            _employeeMutations[employeeIndex] = mutations;
        }

        mutations[elementName] = value;
    }

    private static XElement GetEmployeeByIndex(
        XDocument document,
        XNamespace ns,
        int index)
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
        Employee =
        [
            CreateEmployee(
                caseReference: "CN70000537",
                employerName: "Employer Test",
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
                caseReference: "CN70000537",
                employerName: "Employer Test 2",
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
                caseReference: "CN70000537",
                employerName: "Employer Test 3",
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
                caseReference: "CN70000537",
                employerName: "Employer Test 4",
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
                caseReference: "CN70000537",
                employerName: "Employer Test 5",
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
        string caseReference,
        string employerName,
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
            Header = new RP14AEmployeeHeader { CaseReference = caseReference },
            EmployerName = employerName,
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
                ComponentPayPerWeek1 = new RP14AEmployeePayDetailsComponentPayPerWeek1
                {
                    ComponentType = ComponentType.HolidayPayAccrued,
                    ComponentRate = "1000",
                    ComponentRateSpecified = true,
                    ComponentRateStatus = ComponentRateStatusType.Default
                },
                ComponentPayPerWeek2 = new RP14AEmployeePayDetailsComponentPayPerWeek2
                {
                    ComponentType = ComponentType.HolidayPayTakenNotPaid,
                    ComponentRateStatus = ComponentRateStatusType.Fixedrateofpay
                },
                WeeklyPayDay = weeklyPayDay,
                WeeklyPayDaySpecified = true,
                ArrearsOfPay = new RP14AEmployeePayDetailsArrearsOfPay
                {
                    ArrearsOfPayPeriod1 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod1
                    {
                        AOP1StartDate = new DateTime(2020, 1, aopStartDay),
                        AOP1StartDateSpecified = true,
                        AOP1EndDate = new DateTime(2020, 1, aopStartDay + 1),
                        AOP1EndDateSpecified = true,
                        AOPOwed1 = 100m,
                        AOPOwed1Specified = true,
                        AOPPayType1 = RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod1AOPPayType1.wages,
                        AOPPayType1Specified = true
                    },
                    ArrearsOfPayPeriod2 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod2
                    {
                        AOP2StartDate = new DateTime(2020, 1, aopStartDay + 2),
                        AOP2StartDateSpecified = true,
                        AOP2EndDate = new DateTime(2020, 1, aopStartDay + 3),
                        AOP2EndDateSpecified = true,
                        AOPOwed2 = 100m,
                        AOPOwed2Specified = true,
                        AOPPayType2 = RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod2AOPPayType2.bouncedcheque,
                        AOPPayType2Specified = true
                    },
                    ArrearsOfPayPeriod3 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod3
                    {
                        AOP3StartDate = new DateTime(2020, 1, aopStartDay + 4),
                        AOP3StartDateSpecified = true,
                        AOP3EndDate = new DateTime(2020, 1, aopStartDay + 5),
                        AOP3EndDateSpecified = true,
                        AOPOwed3 = 100m,
                        AOPOwed3Specified = true,
                        AOPPayType3 = RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod3AOPPayType3.commission,
                        AOPPayType3Specified = true
                    },
                    ArrearsOfPayPeriod4 = new RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod4
                    {
                        AOP4StartDate = new DateTime(2020, 1, aopStartDay + 6),
                        AOP4StartDateSpecified = true,
                        AOP4EndDate = new DateTime(2020, 1, aopStartDay + 7),
                        AOP4EndDateSpecified = true,
                        AOPOwed4 = 100m,
                        AOPOwed4Specified = true,
                        AOPPayType4 = RP14AEmployeePayDetailsArrearsOfPayArrearsOfPayPeriod4AOPPayType4.overtime,
                        AOPPayType4Specified = true
                    }
                }
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
                HolidayNotPaid = new RP14AEmployeeHolidayHolidayNotPaid
                {
                    Holiday1 = new RP14AEmployeeHolidayHolidayNotPaidHoliday1
                    {
                        Holiday1StartDate = new DateTime(2020, 1, aopStartDay + 10),
                        Holiday1StartDateSpecified = true,
                        Holiday1EndDate = new DateTime(2020, 1, aopStartDay + 11),
                        Holiday1EndDateSpecified = true
                    },
                    Holiday2 = new RP14AEmployeeHolidayHolidayNotPaidHoliday2
                    {
                        Holiday2StartDate = new DateTime(2020, 1, aopStartDay + 12),
                        Holiday2StartDateSpecified = true,
                        Holiday2EndDate = new DateTime(2020, 1, aopStartDay + 13),
                        Holiday2EndDateSpecified = true
                    },
                    Holiday3 = new RP14AEmployeeHolidayHolidayNotPaidHoliday3
                    {
                        Holiday3StartDate = new DateTime(2020, 1, aopStartDay + 14),
                        Holiday3StartDateSpecified = true,
                        Holiday3EndDate = new DateTime(2020, 1, aopStartDay + 15),
                        Holiday3EndDateSpecified = true
                    }
                },
                NoDaysHolidayOwed = 10m,
                NoDaysHolidayOwedSpecified = true
            }
        };
    }

    private static void SaveXmlDocument(XDocument document, string filePath) =>
        XmlFixtureHelper.SaveXmlDocument(document, filePath);

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
        XmlFixtureHelper.Log("RP14A", "INFO", message);
}
