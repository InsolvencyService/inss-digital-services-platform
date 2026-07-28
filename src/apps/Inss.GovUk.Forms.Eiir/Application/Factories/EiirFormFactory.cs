using GovUk.Forms.Application.Factories;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Enums;
using GovUk.Forms.Domain.Search;

namespace Inss.GovUk.Forms.Eiir.Application.Factories;

public sealed class EiirFormFactory : IFormFactory
{
    public FormModel Create()
    {
        return new FormModel
        {
            Sections =
            [
                new SectionModel
                {
                    Title = "Individual Insolvency Register",
                    Pages =
                    [
                        new SearchTermModel
                        {
                            Title = "Search",
                            Path = "/search",
                            SubmitType = SubmitTypes.None,
                            MetaData =
                            {
                                Question = "Name of individual or trading name",
                                Description = "<p class=\"govuk-body\">Search using one or more of the following:</p>" +
                                              "<ul class=\"govuk-list govuk-list--bullet\">" +
                                              "<li>name</li>" +
                                              "<li>town or city</li>" +
                                              "<li>full or partial postcode</li>" +
                                              "<li>a combination of these</li>" +
                                              "</ul>",
                                SubmitButtonText = "Search"
                            }
                        },
                        new SearchResultModel
                        {
                            Title = "Search results",
                            Path = "/search-results",
                            SubmitType = SubmitTypes.None,
                            ConfigKey = "EIIRPersonSearch",
                            MetaData =
                            {
                                Question = "Search results",
                                Description = "<p class=\"govuk-body\">Your search returned 10000 records. These may include individuals with an alias or a previous name which matches your search criteria. If you expected to see a name in the register and it is not there, tell <a href=\"https://www.insolvencydirect.bis.gov.uk/ExternalOnlineForms/GeneralEnquiry.aspx\">the Insolvency Service</a>.</p>" +
                                              "<p class=\"govuk-body\">Select a <span class=\"govuk-!-font-weight-bold\">Name</span> link to view the relevant case details.</p>"
                            }
                        },
                        new SearchResultDetailModel
                        {
                            Title = "Search result detail",
                            Path = "/search-result-detail",
                            SubmitType = SubmitTypes.None,
                            MetaData =
                            {
                                Question = "Search result detail",
                                SubmitButtonText = "Search again"
                            }
                        }
                    ]
                }
            ]
        };
    }
}