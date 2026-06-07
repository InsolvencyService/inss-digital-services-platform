using GovUk.Forms.Domain;
using Inss.GovUk.Forms.Fip.Factories;
using Xunit;

namespace Inss.GovUk.Forms.Fip.Test.Factories;

// TODO: Fix this to match your form

public class BankruptcyFormFactoryTests
{
    [Fact]
    public void ForForm_Create_SetsFormPath()
    {
        FipFormFactory factory = new();

        FormModel form = factory.Create();
        
        Assert.Equal("/fip", form.Path);
    }
    
    [Fact]
    public void ForForm_Create_AddsAllFormSections()
    {
        FipFormFactory factory = new();

        FormModel form = factory.Create();
        
        Assert.Single(form.Sections);
        Assert.Equal("Find an Insolvency Practitioner", form.Sections[0].Title);
        Assert.Equal("/fip/search", form.Sections[0].Path);
    }
    
    [Fact]
    public void ForForm_Create_AddsAllPagesToSection()
    {
        FipFormFactory factory = new();

        FormModel form = factory.Create();

        SectionModel bankruptcyDetails = form.Sections["Find an Insolvency Practitioner"];
        Assert.Equal(2, bankruptcyDetails.Pages.Count);
        AssertSectionPage<DateModel>(bankruptcyDetails, "Todo", "/fip/search/todo");
        AssertSectionPage<SummaryModel>(bankruptcyDetails, "TODO summary", "/fip/search/summary");
    }
    
    private static void AssertSectionPage<TPage>(SectionModel section, string title, string path) where TPage : PageModel
    {
        TPage? page = section.Pages.GetAllPathPages().FirstOrDefault(p => p.Path == path && p is TPage) as TPage;
        Assert.NotNull(page);
        Assert.Equal(title, page.Title);
    }
}