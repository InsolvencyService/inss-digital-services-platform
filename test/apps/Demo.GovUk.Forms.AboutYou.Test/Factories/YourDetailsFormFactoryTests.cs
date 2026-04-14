using Demo.GovUk.Forms.AboutYou.Domain;
using Demo.GovUk.Forms.AboutYou.Factories;
using GovUk.Forms.Domain;
using Xunit;

namespace Demo.GovUk.Forms.AboutYou.Test.Factories;

public class YourDetailsFormFactoryTests
{
    [Fact]
    public void ForForm_Create_SetsFormPath()
    {
        AboutYouFormFactory factory = new();

        FormModel form = factory.Create();
        
        Assert.Equal("/about-you", form.Path);
    }
    
    [Fact]
    public void ForForm_Create_AddsAllFormSections()
    {
        AboutYouFormFactory factory = new();

        FormModel form = factory.Create();
        
        Assert.Single(form.Sections);
        Assert.Equal("Your Details", form.Sections[0].Title);
        Assert.Equal("/about-you/your-details", form.Sections[0].Path);
    }
    
    [Fact]
    public void ForForm_Create_AddsAllPagesToYourDetailsSection()
    {
        AboutYouFormFactory factory = new();

        FormModel form = factory.Create();

        SectionModel yourDetails = form.Sections["Your Details"];
        Assert.Equal(9, yourDetails.Pages.Count);
        AssertSectionPage<FullNameModel>(yourDetails, "Your name", "/about-you/your-details/your-name");
        AssertSectionPage<AddressModel>(yourDetails, "Your address", "/about-you/your-details/your-address");
        AssertSectionPage<AgeModel>(yourDetails, "Your age", "/about-you/your-details/your-age");
        AssertSectionPage<SalaryModel>(yourDetails, "Your salary", "/about-you/your-details/your-salary");
        AssertSectionPage<BankAccountModel>(yourDetails, "Your bank account", "/about-you/your-details/your-bank-account");
        AssertSectionPage<OwnHomeModel>(yourDetails, "Own your own home", "/about-you/your-details/your-home-ownership");
        AssertSectionPage<HomeValueModel>(yourDetails, "Your home value", "/about-you/your-details/your-home-value");
        AssertSectionPage<SummaryModel>(yourDetails, "Your summary", "/about-you/your-details/summary");
    }
    
    private static void AssertSectionPage<TPage>(SectionModel section, string title, string path) where TPage : PageModel
    {
        TPage? page = section.Pages.GetAllPathPages().FirstOrDefault(p => p.Path == path && p is TPage) as TPage;
        Assert.NotNull(page);
        Assert.Equal(title, page.Title);
    }
}