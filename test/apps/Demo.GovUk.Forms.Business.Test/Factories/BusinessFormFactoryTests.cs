using Demo.GovUk.Forms.Business.Factories;
using GovUk.Forms.Domain;
using Xunit;

namespace Demo.GovUk.Forms.Business.Test.Factories;

public class BusinessFormFactoryTests
{
    [Fact]
    public void ForForm_Create_SetsFormPath()
    {
        BusinessFormFactory factory = new();

        FormModel form = factory.Create();
        
        Assert.Equal("/business", form.Path);
    }
    
    [Fact]
    public void ForForm_Create_AddsAllFormSections()
    {
        BusinessFormFactory factory = new();

        FormModel form = factory.Create();
        
        Assert.Equal(2, form.Sections.Count);
        Assert.Equal("Employee Details", form.Sections[0].Title);
        Assert.Equal("/business/your-employee-details", form.Sections[0].Path);
        Assert.Equal("Creditors and Debtors", form.Sections[1].Title);
        Assert.Equal("/business/your-creditors-and-debtors", form.Sections[1].Path);
    }
    
    [Fact]
    public void ForForm_Create_AddsAllPagesToYourEmployeesSection()
    {
        BusinessFormFactory factory = new();

        FormModel form = factory.Create();

        SectionModel employeeDetails = form.Sections["Employee Details"];
        Assert.Equal(6, employeeDetails.Pages.GetAllPathPages().Count);
        AssertSectionPage<FullNameModel>(employeeDetails, "Employee name", "/business/your-employee-details/employee-name");
        AssertSectionPage<AgeModel>(employeeDetails, "Employee age", "/business/your-employee-details/employee-age");
        AssertSectionPage<CheckAnswersModel>(employeeDetails, "Check employee details", "/business/your-employee-details/check-employee-details");
        AssertSectionPage<RemoveModel>(employeeDetails, "Remove employee details", "/business/your-employee-details/remove-employee-details");
        AssertSectionPage<AddAnotherModel>(employeeDetails, "Employee details", "/business/your-employee-details/add-another-employee");
        AssertSectionPage<SummaryModel>(employeeDetails, "Employee summary", "/business/your-employee-details/summary");
    }

    [Fact]
    public void ForForm_Create_AddsAllPagesToYourCreditorsAndDebtorsSection()
    {
        BusinessFormFactory factory = new();

        FormModel form = factory.Create();

        SectionModel creditorDetails = form.Sections["Creditors and Debtors"];
        Assert.Equal(11, creditorDetails.Pages.GetAllPathPages().Count);
        AssertSectionPage<FullNameModel>(creditorDetails, "Creditor name", "/business/your-creditors-and-debtors/creditor-name");
        AssertSectionPage<MoneyModel>(creditorDetails, "Amount owned to creditor", "/business/your-creditors-and-debtors/amount-owned-to-creditor");
        AssertSectionPage<CheckAnswersModel>(creditorDetails, "Check creditor details", "/business/your-creditors-and-debtors/check-creditor-details");
        AssertSectionPage<RemoveModel>(creditorDetails, "Remove creditor details", "/business/your-creditors-and-debtors/remove-creditor-details");
        AssertSectionPage<AddAnotherModel>(creditorDetails, "Creditor details", "/business/your-creditors-and-debtors/add-another-creditor");
        AssertSectionPage<FullNameModel>(creditorDetails, "Debtor name", "/business/your-creditors-and-debtors/debtor-name");
        AssertSectionPage<MoneyModel>(creditorDetails, "Amount owned by the debtor", "/business/your-creditors-and-debtors/amount-owned-by-debtor");
        AssertSectionPage<CheckAnswersModel>(creditorDetails, "Check debtor details", "/business/your-creditors-and-debtors/check-debtor-details");
        AssertSectionPage<RemoveModel>(creditorDetails, "Remove debtor details", "/business/your-creditors-and-debtors/remove-debtor-details");
        AssertSectionPage<AddAnotherModel>(creditorDetails, "Debtor details", "/business/your-creditors-and-debtors/add-another-debtor");
        AssertSectionPage<SummaryModel>(creditorDetails, "Creditors and debtors summary", "/business/your-creditors-and-debtors/summary");
    }
    
    private static void AssertSectionPage<TPage>(SectionModel section, string title, string path) where TPage : PageModel
    {
        TPage? page = section.Pages.GetAllPathPages().FirstOrDefault(p => p.Path == path && p is TPage) as TPage;
        Assert.NotNull(page);
        Assert.Equal(title, page.Title);
    }
}