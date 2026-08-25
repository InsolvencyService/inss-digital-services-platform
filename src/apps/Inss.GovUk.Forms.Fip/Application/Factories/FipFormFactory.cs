using GovUk.Forms.Application.Factories;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Enums;
using GovUk.Forms.Domain.Search;

namespace Inss.GovUk.Forms.Fip.Application.Factories;

public sealed class FipFormFactory : IFormFactory
{
    public FormModel Create()
    {
        // As FIP is not actually a form that we collect and process data for the user - just search we don't want the full form/section
        // paths defining so we can simply create a form and still benefit from the form engine
        
        return new FormModel
        {
            Sections =
            [
                new SectionModel
                {
                    Title = "Find an Insolvency Practitioner",
                    Pages =
                    [
                        new SearchTermModel
                        {
                            Title = "Search",
                            Path = "/search",
                            SubmitType = SubmitTypes.None,
                            MetaData =
                            {
                                Question = "Find an insolvency practitioner",
                                Description = "<p class=\"govuk-body\">Search using one or more of the following:</p>" +
                                              "<ul class=\"govuk-list govuk-list--bullet\">" +
                                              "<li>name</li>" +
                                              "<li>company</li>" +
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
                            ConfigKey = "FIPSearch",
                            MetaData =
                            {
                                Question = "Search results"
                            }
                        },
                        new SearchResultDetailModel
                        {
                            Title = "Search result detail",
                            Path = "/search-result-detail",
                            SubmitType = SubmitTypes.None,
                            MetaData =
                            {
                                Question = "",
                                SubmitButtonText = "Find another insolvency practitioner"
                            }
                        }
                    ]
                }
            ]
        };
    }
}