using Demo.GovUk.Forms.Bankruptcy.Factories;
using GovUk.Forms.Domain;
using Xunit;

namespace Demo.GovUk.Forms.Bankruptcy.Test.Factories;

public class BankruptcyFormFactoryTests
{
    [Fact]
    public void ForForm_Create_SetsFormPath()
    {
        BankruptcyFormFactory factory = new();

        FormModel form = factory.Create();
        
        Assert.Equal("/bankruptcy", form.Path);
    }
    
    [Fact]
    public void ForForm_Create_AddsAllFormSections()
    {
        BankruptcyFormFactory factory = new();

        FormModel form = factory.Create();
        
        Assert.Single(form.Sections);
        Assert.Equal("Bankruptcy", form.Sections[0].Title);
        Assert.Equal("/bankruptcy/your-bankruptcy", form.Sections[0].Path);
    }
    
    [Fact]
    public void ForForm_Create_AddsAllPagesToYourBankruptcySection()
    {
        BankruptcyFormFactory factory = new();

        FormModel form = factory.Create();

        SectionModel bankruptcyDetails = form.Sections["Bankruptcy"];
        Assert.Equal(2, bankruptcyDetails.Pages.Count);
        AssertSectionPage<DateModel>(bankruptcyDetails, "Date you went bankrupt", "/bankruptcy/your-bankruptcy/date-bankrupt");
        AssertSectionPage<SummaryModel>(bankruptcyDetails, "Bankruptcy summary", "/bankruptcy/your-bankruptcy/summary");
    }
    
    private static void AssertSectionPage<TPage>(SectionModel section, string title, string path) where TPage : PageModel
    {
        TPage? page = section.Pages.GetAllPathPages().FirstOrDefault(p => p.Path == path && p is TPage) as TPage;
        Assert.NotNull(page);
        Assert.Equal(title, page.Title);
    }
}