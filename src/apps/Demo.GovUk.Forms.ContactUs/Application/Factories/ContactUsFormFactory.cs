using Demo.GovUk.Forms.ContactUs.Domain;
using GovUk.Forms.Application.Factories;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Domain;

namespace Demo.GovUk.Forms.ContactUs.Application.Factories;

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
            .AddSearchPage<SearchModel>("Search", "search",description:"hello", submitButtonText: "Search")
            .AddSearchResultColumn("Column1")
            .AddSearchResultColumn("Column2")
            .AddFinalColumn("Column3")
            .EndSection<SummaryModel>("Find people summary", "summary", submitButtonText: "Continue")


            // TODO: Testing how to use the component so it is re-useable!!
            .AddSection("Finding Peoples Part Two", "find-people")
            .AddSearchPage<SearchModel>("Search", "search", submitButtonText: "Search")
            .AddSearchResultColumn("Column1")
            .AddSearchResultColumn("Column2")
            .AddFinalColumn("Column3")
            .EndSection<SummaryModel>("Find people summary", "summary", submitButtonText: "Continue")

            .ValidateAndComplete();
    }
}