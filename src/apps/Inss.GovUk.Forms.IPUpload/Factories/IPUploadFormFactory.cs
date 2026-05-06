using GovUk.Forms.Components.Builders;
using GovUk.Forms.Components.Factories;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Enums;
using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Factories;

public sealed class IPUploadFormFactory : IFormFactory
{
    public FormModel Create()
    {
        return FormModelBuilder
            .Create("ip-upload", SubmitTypes.Section)
            
            .AddSection("IP Upload", "redundancy-payment")
            .AddPage<IPUploadDeclarationModel>("Declaration", "declaration", submitButtonText: "Agree and continue")
            .AddPage<XmlFileUploadModel>("Upload document", "upload-document", submitButtonText: "Continue")
            .AddPage<IPUploadXmlErrorsModel>("IP upload errors", "upload-errors", submitButtonText: "Continue")
            .AddPage<IPUploadXmlErrorDetailsModel>("IP upload error details", "upload-error-details")
            .AddPage<SummaryModel>("Redundancy payment summary", "summary", question: "Check your answers before submitting the form", submitButtonText: "Submit", description: "Your form has passed initial validation.")
            .EndSection<PostSubmitModel>("Submitted", "submit-completed", submitButtonText: "Upload another form")

            .ValidateAndComplete();
    }
}