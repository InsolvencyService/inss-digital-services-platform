using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Domain;
using Inss.GovUk.Forms.IPUpload.Factories;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Factories;

public class IPUploadFormFactoryTests
{
    [Fact]
    public void ForForm_Create_SetsFormPath()
    {
        IPUploadFormFactory factory = new();

        FormModel form = factory.Create();
        
        Assert.Equal("/ip-upload", form.Path);
    }
    
    [Fact]
    public void ForForm_Create_AddsAllFormSections()
    {
        IPUploadFormFactory factory = new();

        FormModel form = factory.Create();
        
        Assert.Single(form.Sections);
        Assert.Equal("IP Upload", form.Sections[0].Title);
        Assert.Equal("/ip-upload/redundancy-payment", form.Sections[0].Path);
    }
    
    [Fact]
    public void ForForm_Create_AddsAllPagesToRedundancyPaymentSection()
    {
        IPUploadFormFactory factory = new();

        FormModel form = factory.Create();

        SectionModel redundancyPayment = form.Sections[0];
        Assert.Equal(4, redundancyPayment.Pages.Count);
        AssertSectionPage<IPUploadDeclarationModel>(redundancyPayment, "Declaration", "/ip-upload/redundancy-payment/declaration");
        AssertSectionPage<XmlFileUploadModel>(redundancyPayment, "Upload document", "/ip-upload/redundancy-payment/upload-document");
        AssertSectionPage<IPUploadXmlErrorsModel>(redundancyPayment, "IP upload errors", "/ip-upload/redundancy-payment/upload-errors");
        AssertSectionPage<SummaryModel>(redundancyPayment, "Redundancy payment summary", "/ip-upload/redundancy-payment/summary");
    }
    
    private static void AssertSectionPage<TPage>(SectionModel section, string title, string path) where TPage : PageModel
    {
        TPage? page = section.Pages.GetAllPathPages().FirstOrDefault(p => p.Path == path && p is TPage) as TPage;
        Assert.NotNull(page);
        Assert.Equal(title, page.Title);
    }
}