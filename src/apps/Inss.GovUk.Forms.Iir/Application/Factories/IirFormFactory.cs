using GovUk.Forms.Application.Factories;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Enums;
using GovUk.Forms.Domain.Search;

namespace Inss.GovUk.Forms.Iir.Application.Factories;

public sealed class IirFormFactory : IFormFactory
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
                              Question = "Search the individual insolvency register",
                              SearchTitle = "Name of individual or trading name",
                              Description = "<p class=\"govuk-body\">Search using one or more of the following:</p>" +
                                              "<ul class=\"govuk-list govuk-list--bullet\">" +
                                              "<li>name</li>" +
                                              "<li>trading name</li>" +
                                              "</ul>",
                              SubmitButtonText = "Search"
                            }
                        },
                        new SearchResultModel
                        {
                            Title = "Search results",
                            Path = "/search-results",
                            SubmitType = SubmitTypes.None,
                            ConfigKey = "IIRSearch",
                            MetaData =
                            {
                                Question = "Search results",
                                Description = "<p class=\"govuk-body\">Your results may include individuals with an alias or a previous name which matches your search criteria.</p>" +
                                              "<p class=\"govuk-body\"> If you expected to see a name in the register and it is not there, <a href=\"\">tell the Insolvency Service.</a></p>\r\n"
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
                                SubmitButtonText = "New search"
                            }
                        }
                    ]
                }
            ]
        };
    }
}