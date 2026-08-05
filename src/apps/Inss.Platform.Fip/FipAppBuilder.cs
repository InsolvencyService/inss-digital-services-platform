using Inss.Platform.Application.Factories;
using Inss.Platform.Application.Loaders;
using Inss.Platform.Component;
using Inss.Platform.Component.Builders;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Fip;

public sealed class FipAppBuilder : AppBuilder
{
    public override PagePath[] Build(IServiceCollection services)
    {
        PageModel searchTermPage = PageModelBuilder
            .For("Search", "/search", new Content("Search"))
            .NextPagesIs("/search-results")
            .AddSearchTermComponent("SearchTerm", new Content("Find an insolvency practitioner"), new Content("Search"), new Content(
                "<p class=\"govuk-body\">Search using one or more of the following:</p>" +
                "<ul class=\"govuk-list govuk-list--bullet\">" +
                "<li>name</li>" +
                "<li>company</li>" +
                "<li>town or city</li>" +
                "<li>full or partial postcode</li>" +
                "<li>a combination of these</li>" +
                "</ul>"))
            .WithRequiredValidator("You must enter a search text")
            .ComponentAdded()
            .Build(services);
        
        PageModel searchResultPage = PageModelBuilder
            .For("Search results", "/search-results", displayFullWidth: true)
            .NextPagesIs("/search-results") // Reload ourselves with the new search
            .AddSearchResultComponent("LastName", new Content("Search results"), "FIPSearch")
            .WithLoader<SearchResultComponentLoader>()
            .WithRequiredValidator("You must enter a search text")
            .ComponentAdded()
            .Build(services);
        
        services.AddSingleton<IAppFactory>(_ => new AppFactory([searchTermPage, searchResultPage]));

        return [searchTermPage.Path, searchResultPage.Path];
    }
}