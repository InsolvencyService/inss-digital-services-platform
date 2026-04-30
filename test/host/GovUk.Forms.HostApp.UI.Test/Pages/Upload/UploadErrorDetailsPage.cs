using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Extensions;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Upload;

public class UploadErrorDetailsPage : BasePage, IUploadErrorDetailsPage
{
    private readonly IPlaywrightDriver _playwrightDriver;

    public UploadErrorDetailsPage(IPlaywrightDriver playwrightDriver)
    {
        _playwrightDriver = playwrightDriver;
    }
    private new IPage Page => _playwrightDriver.Page;

    protected ILocator PageHeading => Page.GetByRole(AriaRole.Heading, new() { Name = "Case reference" });
    private ILocator AffectedEmployeesTable => Page.GetByRole(AriaRole.Table, new() { Name = "Affected employees" });
    protected ILocator EmployerNamePageTitle => Page.GetByRole(AriaRole.Heading, new() { Name = "Employer name" });
    private ILocator TableRows => AffectedEmployeesTable.GetByRole(AriaRole.Row);
    private ILocator GetByExactText(string text) => Page.GetByText(text, new PageGetByTextOptions { Exact = true });


    public async Task VerifyAffectedEmployeeAsync(AffectedEmployee employee)
    {
        ILocator row = TableRows.Filter(new()
        {
            HasTextString = employee.Forename
        });

        await row.GetByRole(AriaRole.Cell, new() { Name = employee.Forename }).ShouldBeVisibleAsync();
        await row.GetByRole(AriaRole.Cell, new() { Name = employee.Surname }).ShouldBeVisibleAsync();
        await row.GetByRole(AriaRole.Cell, new() { Name = employee.DateOfBirth }).ShouldBeVisibleAsync();
        await row.GetByRole(AriaRole.Cell, new() { Name = employee.NiNumber }).ShouldBeVisibleAsync();

        if (!string.IsNullOrWhiteSpace(employee.CellValue))
        {
            await row.GetByText(employee.CellValue, new() { Exact = true }).ShouldBeVisibleAsync();
        }
    }

    public async Task VerifyErrorMessageAsync(string expectedMessage)
    {
        await GetByExactText(expectedMessage).ShouldBeVisibleAsync();
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

    protected override async Task PageContentLoadedAsync()
    {
        await Page.WaitForLoadStateAsync(LoadState.Load,
            new() { Timeout = ScenarioConstant.ElementTimeout });
    }
    public async Task VerifyErrorDetailsDoesNotContainAsync(string text)
    {
        await Expect(Page.Locator("main")).Not.ToContainTextAsync(text);
    }

    public async Task EmployerErrorSummaryIsDisplayedAsync()
    {
        await EmployerNamePageTitle.ShouldBeVisibleAsync();
        await AffectedEmployeesTable.ShouldBeVisibleAsync();
    }
}
