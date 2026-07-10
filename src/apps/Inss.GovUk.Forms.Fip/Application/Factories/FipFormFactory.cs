using GovUk.Forms.Application.Factories;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Search;

namespace Inss.GovUk.Forms.Fip.Application.Factories;

public sealed class FipFormFactory : IFormFactory
{
    public FormModel Create()
    {
        return FormModelBuilder
            .Create("fip")
            
            .AddSection("Find an Insolvency Practitioner", "search")
            .AddPage<SearchTermModel>("Enter search", "search", 
                question: "Find an insolvency practitioner",
                description: "<p class=\"govuk-body\">Search using one or more of the following:</p>" +
                             "<ul class=\"govuk-list govuk-list--bullet\">" +
                             "<li>name</li>" +
                             "<li>company</li>" +
                             "<li>town or city</li>" +
                             "<li>full or partial postcode</li>" +
                             "<li>a combination of these</li>" +
                             "</ul>",
                submitButtonText: "Search")
            .AddSearchPage<SearchResultModel>("Search", "search-results", "FIPSearch", question: "Search results", submitButtonText: null)
            .EndSection<SearchResultDetailModel>("Search", "search-result-detail", question: "Search result detail", submitButtonText: "Find another insolvency practitioner")

            .ValidateAndComplete();
    }
}