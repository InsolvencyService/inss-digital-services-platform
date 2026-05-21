using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Upload;

public sealed class UploadErrorDetailsPage : BasePage, IUploadErrorDetailsPage
{
    private const int ForenameColumnIndex = 0;
    private const int SurnameColumnIndex = 1;
    private const int DateOfBirthColumnIndex = 2;
    private const int NiNumberColumnIndex = 3;
    private const int CellValueColumnIndex = 4;

    private readonly IPlaywrightDriver _playwrightDriver;

    public UploadErrorDetailsPage(IPlaywrightDriver playwrightDriver)
    {
        _playwrightDriver = playwrightDriver
            ?? throw new ArgumentNullException(nameof(playwrightDriver));
    }

    private new IPage Page => _playwrightDriver.Page;

    private ILocator MainContent => Page.Locator("main");

    private ILocator AffectedEmployeesTable => Page.Locator("table.govuk-table");

    private ILocator TableRows => AffectedEmployeesTable.GetByRole(AriaRole.Row);

    private ILocator BackButton =>
        Page.GetByRole(AriaRole.Link, new() { Name = SharedLocactors.BackButton, Exact = true });

    public async Task VerifyErrorMessageAsync(string expectedMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(expectedMessage);

        await Expect(Page.GetByText(expectedMessage, new() { Exact = true }))
            .ToBeVisibleAsync();
    }

    public async Task VerifyAffectedEmployeeTableHeadersAsync()
    {
        string[] expectedHeaders =
        [
            "Forenames",
            "Surname",
            "Date of birth",
            "NI number",
            "Cell value"
        ];

        await Expect(AffectedEmployeesTable).ToBeVisibleAsync();

        foreach (string expectedHeader in expectedHeaders)
        {
            await Expect(
                AffectedEmployeesTable.GetByRole(
                    AriaRole.Columnheader,
                    new() { Name = expectedHeader, Exact = true }))
                .ToBeVisibleAsync();
        }

        IReadOnlyList<string> actualHeaders =
            await AffectedEmployeesTable
                .GetByRole(AriaRole.Columnheader)
                .AllInnerTextsAsync();

        string[] normalizedHeaders = actualHeaders
            .Select(header => header.Trim())
            .ToArray();

        Assert.That(
            normalizedHeaders,
            Is.EqualTo(expectedHeaders),
            "The affected employee table headers do not match.");
    }

    public async Task VerifyErrorDetailsDoesNotContainAsync(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        await Expect(MainContent).Not.ToContainTextAsync(text);
    }

    public async Task VerifyErrorSummaryIsDisplayedAsync(ErrorCategory category)
    {
        ILocator heading = GetErrorCategoryHeading(category);

        await Expect(heading).ToBeVisibleAsync();
        await Expect(AffectedEmployeesTable).ToBeVisibleAsync();
    }

    public async Task ClickBackButtonAsync()
    {
        await Expect(BackButton).ToBeVisibleAsync();
        await BackButton.ClickAsync();
    }

    protected override async Task PageContentLoadedAsync()
    {
        await Page.WaitForLoadStateAsync(
            LoadState.Load,
            new() { Timeout = ScenarioConstant.ElementTimeout });
    }

    public async Task VerifyAffectedEmployeeAsync(AffectedEmployee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);

        ILocator row = await GetAffectedEmployeeRowAsync(employee);

        await VerifyCellAsync(row, ForenameColumnIndex, employee.Forename);
        await VerifyCellAsync(row, SurnameColumnIndex, employee.Surname);
        await VerifyCellAsync(row, DateOfBirthColumnIndex, employee.DateOfBirth);
        await VerifyCellAsync(row, NiNumberColumnIndex, employee.NiNumber);
        await VerifyCellAsync(row, CellValueColumnIndex, employee.CellValue);
    }

    public async Task<int> GetColumnIndexAsync(string columnName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

        ILocator headers = AffectedEmployeesTable.GetByRole(AriaRole.Columnheader);
        int count = await headers.CountAsync();

        for (int i = 0; i < count; i++)
        {
            string headerText = (await headers.Nth(i).InnerTextAsync()).Trim();

            if (headerText.Equals(columnName, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        throw new InvalidOperationException(
            $"Column '{columnName}' was not found in the affected employees table.");
    }

    private async Task<ILocator> GetAffectedEmployeeRowAsync(AffectedEmployee employee)
    {
        ILocator rows = ApplyEmployeeFilters(TableRows, employee);
        int matchCount = await rows.CountAsync();

        if (matchCount == 0)
        {
            throw new InvalidOperationException(
                $"No affected employee row found matching: " +
                $"Forename='{employee.Forename}', " +
                $"Surname='{employee.Surname}', " +
                $"DOB='{employee.DateOfBirth}', " +
                $"NI number='{employee.NiNumber}', " +
                $"Cell value='{employee.CellValue}'.");
        }

        if (matchCount == 1)
        {
            return rows.First;
        }

        return await FindRowByCellValueAsync(rows, employee.CellValue);
    }

    private static async Task<ILocator> FindRowByCellValueAsync(
        ILocator rows,
        string? cellValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cellValue);

        int rowCount = await rows.CountAsync();

        for (int i = 0; i < rowCount; i++)
        {
            ILocator row = rows.Nth(i);
            string actualCellValue =
                (await GetCellValueAsync(row, CellValueColumnIndex)).Trim();

            if (actualCellValue.Equals(cellValue, StringComparison.Ordinal))
            {
                return row;
            }
        }

        throw new InvalidOperationException(
            $"No row found with cell value '{cellValue}' among {rowCount} matching rows.");
    }

    private static ILocator ApplyEmployeeFilters(
        ILocator rows,
        AffectedEmployee employee)
    {
        ILocator filteredRows = rows;

        if (!string.IsNullOrWhiteSpace(employee.Forename))
        {
            filteredRows = filteredRows.Filter(new() { HasTextString = employee.Forename });
        }

        if (!string.IsNullOrWhiteSpace(employee.Surname))
        {
            filteredRows = filteredRows.Filter(new() { HasTextString = employee.Surname });
        }

        if (!string.IsNullOrWhiteSpace(employee.DateOfBirth))
        {
            filteredRows = filteredRows.Filter(new() { HasTextString = employee.DateOfBirth });
        }

        if (!string.IsNullOrWhiteSpace(employee.NiNumber))
        {
            filteredRows = filteredRows.Filter(new() { HasTextString = employee.NiNumber });
        }

        if (!string.IsNullOrWhiteSpace(employee.CellValue))
        {
            filteredRows = filteredRows.Filter(new() { HasTextString = employee.CellValue });
        }

        return filteredRows;
    }

    private static async Task VerifyCellAsync(
        ILocator row,
        int columnIndex,
        string? expectedValue)
    {
        if (columnIndex < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(columnIndex),
                "Column index cannot be negative.");
        }

        ILocator cell = row.GetByRole(AriaRole.Cell).Nth(columnIndex);

        await Expect(cell).ToBeVisibleAsync();

        string expectedText = string.IsNullOrWhiteSpace(expectedValue)
            ? string.Empty
            : expectedValue;

        await Expect(cell).ToHaveTextAsync(expectedText);
    }

    private static async Task<string> GetCellValueAsync(
        ILocator row,
        int columnIndex)
    {
        if (columnIndex < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(columnIndex),
                "Column index cannot be negative.");
        }

        ILocator cell = row.GetByRole(AriaRole.Cell).Nth(columnIndex);

        return await cell.InnerTextAsync() ?? string.Empty;
    }

    private ILocator GetErrorCategoryHeading(ErrorCategory category)
    {
        string headingText = category switch
        {
            ErrorCategory.EmployerName => "Employer name",
            ErrorCategory.EmployeeSurname => "Employee surname",
            ErrorCategory.EmployeeArrearsOfPaymentOwed => "Employee arrears of payment owed",
            ErrorCategory.EmployeeNationalInsuranceNumber => "Employee national insurance number",
            ErrorCategory.MoneyOwedToEmployer => "Money owed to employer",
            ErrorCategory.EmploymentDates => "Employee employment dates",
            ErrorCategory.ArrearsOfPayDates => "Employee arrears of payment dates",
            ErrorCategory.CaseReference => "Case reference",
            ErrorCategory.BasicPayPerWeek => "Employee basic pay per week",
            ErrorCategory.HolidayContractedEntitlementDays => "Contracted holiday entitlement",
            ErrorCategory.HolidayDaysCarriedForward => "Holiday carried forward",
            ErrorCategory.HolidayDaysTaken => "Holiday days taken",
            ErrorCategory.NoDaysHolidayOwed => "Holiday owed",
            _ => throw new ArgumentOutOfRangeException(
                                                                            nameof(category),
                                                                            category,
                                                                            "Unknown error category.")
        };

        return Page.GetByRole(
            AriaRole.Heading,
            new() { Name = headingText, Exact = true });
    }

    public async Task VerifyEmployerErrorSummaryIsDisplayedAsync() =>
        await VerifyErrorSummaryIsDisplayedAsync(ErrorCategory.EmployerName);

    public async Task VerifyEmployeeErrorSummaryIsDisplayedAsync() =>
       await VerifyErrorSummaryIsDisplayedAsync(ErrorCategory.EmployeeSurname);

    public async Task VerifyEmployeeArrearsOfPaymentOwedErrorSummaryIsDisplayedAsync() =>
        await VerifyErrorSummaryIsDisplayedAsync(ErrorCategory.EmployeeArrearsOfPaymentOwed);

    public async Task VerifyEmployeeNationalInsuranceNumberHeaderIsDisplayedAsync() =>
        await VerifyErrorSummaryIsDisplayedAsync(ErrorCategory.EmployeeNationalInsuranceNumber);

    public async Task VerifyEmploymentDatesHeaderIsDisplayedAsync() =>
        await VerifyErrorSummaryIsDisplayedAsync(ErrorCategory.EmploymentDates);

    public async Task VerifyArrearsOfPayDatesHeaderIsDisplayedAsync() =>
        await VerifyErrorSummaryIsDisplayedAsync(ErrorCategory.ArrearsOfPayDates);

    public async Task VerifyMoneyOwedToEmployerHeaderIsDisplayedAsync() =>
        await VerifyErrorSummaryIsDisplayedAsync(ErrorCategory.MoneyOwedToEmployer);

    public async Task VerifyCaseReferenceHeaderIsDisplayedAsync() =>
      await VerifyErrorSummaryIsDisplayedAsync(ErrorCategory.CaseReference);

    public async Task VerifyBasicPayPerWeekHeaderIsDisplayedAsync() =>
        await VerifyErrorSummaryIsDisplayedAsync(ErrorCategory.BasicPayPerWeek);

    public async Task VerifyContractedHolidayEntitlementHeaderIsDisplayedAsync() =>
        await VerifyErrorSummaryIsDisplayedAsync(ErrorCategory.HolidayContractedEntitlementDays);

    public async Task VerifyHolidayDaysCarriedForwardHeaderIsDisplayedAsync() =>
        await VerifyErrorSummaryIsDisplayedAsync(ErrorCategory.HolidayDaysCarriedForward);

    public async Task VerifyHolidayDaysTakenHeaderIsDisplayedAsync() =>
        await VerifyErrorSummaryIsDisplayedAsync(ErrorCategory.HolidayDaysTaken);

    public async Task VerifyNoDaysHolidayOwedHeaderIsDisplayedAsync() =>
        await VerifyErrorSummaryIsDisplayedAsync(ErrorCategory.NoDaysHolidayOwed);
}

/// <summary>
/// Enum defining the different error categories that can be displayed on the error details page.
/// </summary>
public enum ErrorCategory
{
    EmployerName,
    EmployeeSurname,
    EmployeeArrearsOfPaymentOwed,
    EmployeeNationalInsuranceNumber,
    MoneyOwedToEmployer,
    EmploymentDates,
    ArrearsOfPayDates,
    CaseReference,
    BasicPayPerWeek,
    HolidayContractedEntitlementDays,
    HolidayDaysCarriedForward,
    HolidayDaysTaken,
    NoDaysHolidayOwed
}