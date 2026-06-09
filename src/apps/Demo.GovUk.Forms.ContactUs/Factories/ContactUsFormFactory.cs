using Demo.GovUk.Forms.ContactUs.Domain;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Components.Factories;
using GovUk.Forms.Domain;

namespace Demo.GovUk.Forms.ContactUs.Factories;

public sealed class ContactUsFormFactory : IFormFactory
{
    public FormModel Create()
    {
        return FormModelBuilder
            .Create("contact-us")
            
            .AddSection("Send Us Files", "send-us-files")
            .AddPage<FullNameModel>("Your name", "your-name", submitButtonText: "Continue")
            .AddGroup<AddAnotherGroup>("Files")
            .AddGroupPage<FileUploadModel>("Upload file", "upload-file", submitButtonText: "Continue")
            .AddGroupPage<RemoveModel>("Remove uploaded file", "remove-uploaded-file", submitButtonText: "Continue")
            .AddFinalGroupPage<AddAnotherModel>("Uploaded files", "add-another-file", submitButtonText: "Continue")
            .EndSection<SummaryModel>("Contact us summary", "summary", submitButtonText: "Continue")

            // TODO: Add a specific builder to help construct the search page with columns and display info
            .AddSection("Find People", "find-people")
            .AddPage<SearchModel>("Search", "search", submitButtonText: "Search")
            .EndSection<SummaryModel>("Find people summary", "summary", submitButtonText: "Continue")
            
            .ValidateAndComplete();
    }
}