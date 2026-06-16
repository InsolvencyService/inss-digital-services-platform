using Demo.GovUk.Forms.ContactUs.Application.Factories;
using GovUk.Forms.Domain;
using Xunit;

namespace Demo.GovUk.Forms.ContactUs.Test.Application.Factories;

public class BusinessFormFactoryTests
{
    [Fact]
    public void ForForm_Create_SetsFormPath()
    {
        ContactUsFormFactory factory = new();

        FormModel form = factory.Create();
        
        Assert.Equal("/contact-us", form.Path);
    }
    
    [Fact]
    public void ForForm_Create_AddsAllFormSections()
    {
        ContactUsFormFactory factory = new();

        FormModel form = factory.Create();
        
        Assert.Single(form.Sections);
        Assert.Equal("Send Us Files", form.Sections[0].Title);
        Assert.Equal("/contact-us/send-us-files", form.Sections[0].Path);
    }
    
    [Fact]
    public void ForForm_Create_AddsAllPagesToSendUsFilesSection()
    {
        ContactUsFormFactory factory = new();

        FormModel form = factory.Create();

        SectionModel employeeDetails = form.Sections["Send Us Files"];
        Assert.Equal(5, employeeDetails.Pages.GetAllPathPages().Count);
        AssertSectionPage<FileUploadModel>(employeeDetails, "Upload file", "/contact-us/send-us-files/upload-file");
        AssertSectionPage<RemoveModel>(employeeDetails, "Remove uploaded file", "/contact-us/send-us-files/remove-uploaded-file");
        AssertSectionPage<AddAnotherModel>(employeeDetails, "Uploaded files", "/contact-us/send-us-files/add-another-file");
        AssertSectionPage<SummaryModel>(employeeDetails, "Contact us summary", "/contact-us/send-us-files/summary");
    }
    
    private static void AssertSectionPage<TPage>(SectionModel section, string title, string path) where TPage : PageModel
    {
        TPage? page = section.Pages.GetAllPathPages().FirstOrDefault(p => p.Path == path && p is TPage) as TPage;
        Assert.NotNull(page);
        Assert.Equal(title, page.Title);
    }
}