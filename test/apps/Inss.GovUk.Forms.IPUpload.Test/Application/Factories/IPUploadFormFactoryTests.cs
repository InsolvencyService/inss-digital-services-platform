using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Application.Factories;
using Inss.GovUk.Forms.IPUpload.Domain;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Application.Factories;

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
        Assert.Equal(8, redundancyPayment.Pages.Count);
        AssertSectionPage<IPUploadDeclarationModel>(redundancyPayment, "Declaration", "/ip-upload/redundancy-payment/declaration");
        AssertSectionPage<CheckCaseReferenceModel>(redundancyPayment, "Enter the 10 character case reference number", "/ip-upload/redundancy-payment/check-case-reference");
        AssertSectionPage<EmployerDetailsModel>(redundancyPayment, "Employer details", "/ip-upload/redundancy-payment/case-reference-match");
        AssertSectionPage<XmlFileUploadModel>(redundancyPayment, "Upload redundancy payment forms (RP14/A)", "/ip-upload/redundancy-payment/upload-document");
        AssertSectionPage<IPUploadXmlErrorsModel>(redundancyPayment, "Your form has errors", "/ip-upload/redundancy-payment/upload-errors");
        AssertSectionPage<IPUploadXmlErrorDetailsModel>(redundancyPayment, "IP upload error details", "/ip-upload/redundancy-payment/upload-error-details");
        AssertSectionPage<SummaryModel>(redundancyPayment, "Redundancy payment summary", "/ip-upload/redundancy-payment/summary");
        AssertSectionPage<PostSubmitModel>(redundancyPayment, "What happens next", "/ip-upload/redundancy-payment/submit-completed");
    }
    
    private static void AssertSectionPage<TPage>(SectionModel section, string title, string path) where TPage : PageModel
    {
        TPage? page = section.Pages.GetAllPathPages().FirstOrDefault(p => p.Path == path && p is TPage) as TPage;
        Assert.NotNull(page);
        Assert.Equal(title, page.Title);
    }
}