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
            .For("Search", "/search", "Search")
            .NextPageIs("/search-results")
            .AddSearchTermComponent(
                "SearchTerm", "Find an insolvency practitioner", "Search",
                "<p class=\"govuk-body\">Search using one or more of the following:</p>" +
                "<ul class=\"govuk-list govuk-list--bullet\">" +
                "<li>name</li>" +
                "<li>company</li>" +
                "<li>town or city</li>" +
                "<li>full or partial postcode</li>" +
                "<li>a combination of these</li>" +
                "</ul>")
            .WithLoader<SearchTermComponentLoader>()
            .WithRequiredValidator("You must enter a search text")
            .ComponentAdded()
            .Build(services);
        
        PageModel searchResultPage = PageModelBuilder
            .For("Search results", "/search-results", displayFullWidth: true)
            .NextPageIs("/search-results") // Reload ourselves with the new search
            .PreviousPageIs("/search") // We know it will always be the search page
            .AddSearchResultComponent("SearchResult", "Search results", "FIPSearch", "/search-result-detail")
            .WithLoader<SearchResultComponentLoader>()
            .WithRequiredValidator("You must enter a search text")
            .ComponentAdded()
            .Build(services);
        
        PageModel searchResultDetailPage = PageModelBuilder
            .For("Search result detail", "/search-result-detail", "Find another insolvency practitioner", displayFullWidth: true)
            .NextPageIs("/search") // Return to the search term page for a fresh search
            .AddSearchResultDetailComponent("SearchResultDetail", "FIPSearch")
            .WithLoader<SearchResultDetailComponentLoader>()
            .ComponentAdded()
            .Build(services);
        
        services.AddSingleton<IAppFactory>(_ => new AppFactory([searchTermPage, searchResultPage, searchResultDetailPage]));

        return [searchTermPage.Path, searchResultPage.Path, searchResultDetailPage.Path];
    }
}