using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Extensions;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Upload;

public sealed class UploadErrorDetailsPage : BasePage, IUploadErrorDetailsPage
{
    private readonly IPlaywrightDriver _playwrightDriver;

    public UploadErrorDetailsPage(IPlaywrightDriver playwrightDriver)
    {
        _playwrightDriver = playwrightDriver
            ?? throw new ArgumentNullException(nameof(playwrightDriver));
    }

    private new IPage Page => _playwrightDriver.Page;

    private ILocator MainContent => Page.Locator("main");

    private ILocator EmployerNameHeading =>
        Page.GetByRole(AriaRole.Heading, new() { Name = "Employer name" });

    private ILocator EmployeeSurnameHeading =>
        Page.GetByRole(AriaRole.Heading, new() { Name = "Employee surname" });

    private ILocator AffectedEmployeesTable =>
        Page.GetByRole(AriaRole.Table, new() { Name = "Affected employees" });
    private ILocator EmployeeArrearsOfPaymentOwedHeader =>
        Page.GetByRole(AriaRole.Heading, new() { Name = "Employee arrears of payment owed" });
    private ILocator EmployeeNationalInsuranceNumberHeader =>
    Page.GetByRole(AriaRole.Heading, new() { Name = "Employee national insurance number" });
    private ILocator MoneyOwedToEmployerHeading =>
    Page.GetByRole(AriaRole.Heading, new() { Name = "Money owed to employer" });
    private ILocator EmploymentDatesHeader =>
    Page.GetByRole(AriaRole.Heading, new() { Name = "Employee employment dates" });
    private ILocator ArrearsOfPayDatesHeader =>
    Page.GetByRole(AriaRole.Heading, new() { Name = "Employee arrears of payment dates" });

    private ILocator BackButton => Page.GetByRole(AriaRole.Link, new() { Name = SharedLoactors.BackButton, Exact = true });
    private ILocator TableRows =>
        AffectedEmployeesTable.GetByRole(AriaRole.Row);

    private ILocator ExactText(string text) =>
        Page.GetByText(text, new() { Exact = true });


    public async Task VerifyErrorMessageAsync(string expectedMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(expectedMessage);

        await ExactText(expectedMessage).ShouldBeVisibleAsync();
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

        foreach (string header in expectedHeaders)
        {
            await AffectedEmployeesTable
                .GetByRole(AriaRole.Columnheader, new() { Name = header })
                .ShouldBeVisibleAsync();
        }
    }

    public async Task VerifyErrorDetailsDoesNotContainAsync(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        await Expect(MainContent).Not.ToContainTextAsync(text);
    }

    public async Task VerifyEmployerErrorSummaryIsDisplayedAsync()
    {
        await EmployerNameHeading.ShouldBeVisibleAsync();
        await AffectedEmployeesTable.ShouldBeVisibleAsync();
    }

    public async Task VerifyEmployeeErrorSummaryIsDisplayedAsync()
    {
        await EmployeeSurnameHeading.ShouldBeVisibleAsync();
        await AffectedEmployeesTable.ShouldBeVisibleAsync();
    }

    public async Task VerifyEmployeeArrearsOfPaymentOwedErrorSummaryIsDisplayedAsync()
    {
        await EmployeeArrearsOfPaymentOwedHeader.ShouldBeVisibleAsync();
        await AffectedEmployeesTable.ShouldBeVisibleAsync();
    }
    public async Task VerifyEmployeeNationalInsuranceNumberHeaderIsDisplayedAsync()
    {
        await EmployeeNationalInsuranceNumberHeader.ShouldBeVisibleAsync();
        await AffectedEmployeesTable.ShouldBeVisibleAsync();
    }

    public async Task VerifyEmploymentDatesHeaderIsDisplayedAsync()
    {
        await EmploymentDatesHeader.ShouldBeVisibleAsync();
        await AffectedEmployeesTable.ShouldBeVisibleAsync();
    }

    protected override async Task PageContentLoadedAsync()
    {
        await Page.WaitForLoadStateAsync(LoadState.Load,
           new() { Timeout = ScenarioConstant.ElementTimeout });
    }

    public async Task ClickBackButtonAsync()
    {
        await Expect(BackButton).ToBeVisibleAsync();
        await BackButton.ClickAsync();
    }

    public async Task VerifyArrearsOfPayDatesHeaderIsDisplayedAsync()
    {
        await ArrearsOfPayDatesHeader.ShouldBeVisibleAsync();
        await AffectedEmployeesTable.ShouldBeVisibleAsync();
    }

    public async Task VerifyMoneyOwedToEmployerHeaderIsDisplayedAsync()
    {

        await MoneyOwedToEmployerHeading.ShouldBeVisibleAsync();
        await AffectedEmployeesTable.ShouldBeVisibleAsync();
    }

    public async Task VerifyAffectedEmployeeAsync(AffectedEmployee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);

        ILocator row = TableRows;

        row = ApplyFilterIfNotEmpty(row, employee.NiNumber);
        row = ApplyFilterIfNotEmpty(row, employee.DateOfBirth);
        row = ApplyFilterIfNotEmpty(row, employee.Surname);
        row = ApplyFilterIfNotEmpty(row, employee.Forename);
        row = ApplyFilterIfNotEmpty(row, employee.CellValue);

        await Expect(row).ToHaveCountAsync(1);

        await VerifyCellAsync(row, 0, employee.Forename);
        await VerifyCellAsync(row, 1, employee.Surname);
        await VerifyCellAsync(row, 2, employee.DateOfBirth);
        await VerifyCellAsync(row, 3, employee.NiNumber);
        await VerifyCellAsync(row, 4, employee.CellValue);
    }
    public async Task<int> GetColumnIndexAsync(string columnName)
    {
        ILocator headers = AffectedEmployeesTable.GetByRole(AriaRole.Columnheader);
        int count = await headers.CountAsync();

        for (int i = 0; i < count; i++)
        {
            if ((await headers.Nth(i).InnerTextAsync()).Trim() == columnName)
            {
                return i;
            }
        }

        throw new InvalidOperationException($"Column '{columnName}' not found");
    }
    private static ILocator ApplyFilterIfNotEmpty(ILocator locator, string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? locator
            : locator.Filter(new() { HasTextString = value });
    }

    private static async Task VerifyCellAsync(
    ILocator row,
    int columnIndex,
    string? expectedValue)
    {
        ILocator cell = row.GetByRole(AriaRole.Cell).Nth(columnIndex);

        await cell.ShouldBeVisibleAsync();

        if (string.IsNullOrWhiteSpace(expectedValue))
        {
            await Expect(cell).ToHaveTextAsync(string.Empty);
            return;
        }

        await Expect(cell).ToHaveTextAsync(expectedValue);
    }

    public async Task VerifyMultipleAffectedEmployeeAsync(AffectedEmployee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);

        ILocator row = await GetAffectedEmployeeRowAsync(employee);

        await VerifyCellAsync(row, 0, employee.Forename);
        await VerifyCellAsync(row, 1, employee.Surname);
        await VerifyCellAsync(row, 2, employee.DateOfBirth);
        await VerifyCellAsync(row, 3, employee.NiNumber);
        await VerifyCellAsync(row, 4, employee.CellValue);
    }

    private async Task<ILocator> GetAffectedEmployeeRowAsync(AffectedEmployee employee)
    {
        ILocator rows = TableRows;

        rows = ApplyFilterIfNotEmpty(rows, employee.NiNumber);
        rows = ApplyFilterIfNotEmpty(rows, employee.DateOfBirth);
        rows = ApplyFilterIfNotEmpty(rows, employee.Surname);
        rows = ApplyFilterIfNotEmpty(rows, employee.Forename);

        int rowCount = await rows.CountAsync();

        for (int i = 0; i < rowCount; i++)
        {
            ILocator row = rows.Nth(i);
            string actualCellValue = (await row.GetByRole(AriaRole.Cell).Nth(4).InnerTextAsync()).Trim();

            if (actualCellValue == employee.CellValue)
            {
                return row;
            }
        }

        throw new InvalidOperationException(
            $"Affected employee row was not found for cell value '{employee.CellValue}'.");
    }
}