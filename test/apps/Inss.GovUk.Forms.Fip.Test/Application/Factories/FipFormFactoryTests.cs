using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Search;
using Inss.GovUk.Forms.Fip.Application.Factories;
using Xunit;

namespace Inss.GovUk.Forms.Fip.Test.Application.Factories;

// TODO: Fix this to match your form

public class FipFormFactoryTests
{
    [Fact]
    public void ForForm_Create_SetsFormPath()
    {
        FipFormFactory factory = new();

        FormModel form = factory.Create();
        
        Assert.Equal("/", form.Path);
    }
    
    [Fact]
    public void ForForm_Create_AddsAllFormSections()
    {
        FipFormFactory factory = new();

        FormModel form = factory.Create();
        
        Assert.Single(form.Sections);
        Assert.Equal("Find an Insolvency Practitioner", form.Sections[0].Title);
        Assert.Equal("/", form.Sections[0].Path);
    }
    
    [Fact]
    public void ForForm_Create_AddsAllPagesToSection()
    {
        FipFormFactory factory = new();

        FormModel form = factory.Create();

        SectionModel section = form.Sections["Find an Insolvency Practitioner"];
        Assert.Equal(3, section.Pages.Count);
        AssertSectionPage<SearchTermModel>(section, "Search", "/search");
        AssertSectionPage<SearchResultModel>(section, "Search results", "/search-results");
        AssertSectionPage<SearchResultDetailModel>(section, "Search result detail", "/search-result-detail");
    }
    
    private static void AssertSectionPage<TPage>(SectionModel section, string title, string path) where TPage : PageModel
    {
        TPage? page = section.Pages.GetAllPathPages().FirstOrDefault(p => p.Path == path && p is TPage) as TPage;
        Assert.NotNull(page);
        Assert.Equal(title, page.Title);
    }
}