using Demo.GovUk.Forms.ContactUs.Domain;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Components.Factories;
using GovUk.Forms.Domain;

namespace Demo.GovUk.Forms.ContactUs.Factories;

public sealed class ContactFormFactory : IFormFactory
{
    public FormModel Create()
    {
        return FormModelBuilder
            .Create("contact")
            
            .AddSection("Contact Us", "contact-us")
            .AddPage<FullNameModel>("Your name", "your-name", submitButtonText: "Continue")
            .AddGroup<AddAnotherGroup>("Files")
            .AddGroupPage<FileUploadModel>("Upload file", "upload-file", submitButtonText: "Continue")
            .AddGroupPage<CheckAnswersModel>("Check employee details", "check-employee-details", submitButtonText: "Continue")
            .AddGroupPage<RemoveModel>("Remove uploaded file", "remove-uploaded-file", submitButtonText: "Continue")
            .AddFinalGroupPage<AddAnotherModel>("Uploaded files", "add-another-file", submitButtonText: "Continue")
            .EndSection<SummaryModel>("Contact us summary", "summary", submitButtonText: "Continue")

            .ValidateAndComplete();
    }
}