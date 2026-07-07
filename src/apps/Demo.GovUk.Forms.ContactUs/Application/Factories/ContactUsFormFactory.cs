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
            
            .AddSection("Find People", "find-people")
            .AddPage<SearchTermModel>("Enter search", "search", 
                question: "Find people",
                description: "<p class=\"govuk-body\">Search using one or more of the following:</p>" +
                             "<ul class=\"govuk-list govuk-list--bullet\">" +
                             "<li>surname</li>" +
                             "<li>forename</li>" +
                             "<li>a combination of these</li>" +
                             "</ul>",
                submitButtonText: "Search")
            .AddSearchPage<SearchModel>("Search", "search-results", "Config1", question: "Search results", submitButtonText: null)
            .EndSection<SummaryModel>("Find people summary", "summary", submitButtonText: "Continue")

            .AddSection("Find Other People", "find-other-people")
            .AddPage<SearchTermModel>("Enter search", "search", 
                question: "Find other people",
                description:"<p class=\"govuk-body\">Search using one or more of the following:</p>" +
                            "<ul class=\"govuk-list govuk-list--bullet\">" +
                            "<li>surname</li>" +
                            "<li>forename</li>" +
                            "<li>a combination of these</li>" +
                            "</ul>",
                submitButtonText: "Search")
            .AddSearchPage<SearchModel>("Search", "search-results", "Config2", question: "Search results", submitButtonText: null)
            .EndSection<SummaryModel>("Find other people summary", "summary", submitButtonText: "Continue")

            .ValidateAndComplete();
    }
}