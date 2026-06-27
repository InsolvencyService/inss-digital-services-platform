using GovUk.Forms.Application.Factories;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Search;

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
            .AddGroupPage<FileUploadModel>("Upload file", "upload-file", submitButtonText: "Continue", 
                hint: "The uploaded file must be PDF ending with '.pdf'. (Other formats e.g. XML, XLS are NOT supported).")
            .AddGroupPage<RemoveModel>("Remove uploaded file", "remove-uploaded-file", submitButtonText: "Continue")
            .AddFinalGroupPage<AddAnotherModel>("Uploaded files", "add-another-file", submitButtonText: "Continue")
            .EndSection<SummaryModel>("Contact us summary", "summary", submitButtonText: "Continue")

            // TODO: Add a specific builder to help construct the search page with columns and display info
            .AddSection("Find People", "find-people")
            .AddSearchPage<SearchModel>("Search", "search", "Config1",description:"hello", submitButtonText: "Search")
            .EndSection<SummaryModel>("Find people summary", "summary", submitButtonText: "Continue")


            // TODO: Testing how to use the component so it is re-useable!!
            //.AddSection("Finding Peoples Part Two", "find-people")
            //.AddSearchPage<SearchModel>("Search", "search", "Config2", submitButtonText: "Search")
            //.EndSection<SummaryModel>("Find people summary", "summary", submitButtonText: "Continue")

            .ValidateAndComplete();
    }
}