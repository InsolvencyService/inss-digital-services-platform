using GovUk.Forms.Application.Factories;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Enums;
using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.Factories;

public sealed class IPUploadFormFactory : IFormFactory
{
    public FormModel Create()
    {
        return FormModelBuilder
            .Create("ip-upload", SubmitTypes.Section)
            
            .AddSection("IP Upload", "redundancy-payment")
            .AddPage<IPUploadDeclarationModel>("Declaration", "declaration", submitButtonText: "Agree and continue")
            .AddPage<CheckCaseReferenceModel>("Enter the 10 character case reference number", "check-case-reference", submitButtonText: "Continue", question: "Whats the case reference number?", hint: "For example, �CN12345678�. This must match the case reference number in your form.")
            .AddPage<EmployerDetailsModel>("Employer details", "case-reference-match", submitButtonText: "Continue")
            .AddPage<XmlFileUploadModel>("Upload redundancy payment forms (RP14/A)", "upload-document", submitButtonText: "Continue")
            .AddPage<IPUploadXmlErrorsModel>("Your form has errors", "upload-errors", submitButtonText: "Continue")
            .AddPage<IPUploadXmlErrorDetailsModel>("IP upload error details", "upload-error-details")
            .AddPage<SummaryModel>("Redundancy payment summary", "summary", question: "Check your answers before submitting the form", submitButtonText: "Submit")
            .EndSection<PostSubmitModel>("What happens next", "submit-completed", submitButtonText: "Upload another form")

            .ValidateAndComplete();
    }
}