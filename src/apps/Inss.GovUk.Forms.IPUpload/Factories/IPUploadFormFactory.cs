using GovUk.Forms.Components.Builders;
using GovUk.Forms.Components.Factories;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Enums;
using Inss.GovUk.Forms.IPUpload.Domain;
using XmlFileUploadModel = Inss.GovUk.Forms.IPUpload.Domain.XmlFileUploadModel;

namespace Inss.GovUk.Forms.IPUpload.Factories;

public sealed class IPUploadFormFactory : IFormFactory
{
    public FormModel Create()
    {
        return FormModelBuilder
            .Create("ip-upload", SubmitTypes.Section)
            
            .AddSection("IP Upload", "redundancy-payment")
            .AddStaticPage<IPUploadDeclarationModel>("Declaration", "declaration", "ipupload-declaration", submitButtonText: "Agree and continue")
            .AddPage<XmlFileUploadModel>("Upload document", "upload-document", submitButtonText: "Continue")
            .AddPage<IPUploadXmlErrorsModel>("IP upload errors", "upload-errors", submitButtonText: "Continue")
            .AddPage<SummaryModel>("Redundancy payment summary", "summary", question: "Check your answers before sending the form", submitButtonText: "Send form")
            .EndSection<PostSubmitSuccessModel>("Submitted", "submit-completed")

            .ValidateAndComplete();
    }
}