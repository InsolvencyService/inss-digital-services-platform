using GovUk.Forms.Application.Services;
using Inss.Platform.Application.Providers;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Components.Searching;
using Inss.Platform.Domain.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Inss.Platform.Application.Loaders;

public sealed class SearchResultDetailComponentLoader : IComponentLoader
{
    private readonly IServiceProvider _serviceProvider;

    public SearchResultDetailComponentLoader(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async ValueTask LoadAsync(LoaderContext context)
    {
        SearchResultDetailComponentModel searchResultDetail = context.Component.As<SearchResultDetailComponentModel>();
        
        ISearchConfigProvider searchConfigProvider = _serviceProvider.GetRequiredKeyedService<ISearchConfigProvider>(searchResultDetail.ConfigKey);
        searchResultDetail.Definition = searchConfigProvider.LoadConfig();
        
        // Update the result detail previous path so we can link back to the correct paged results
        PageModel searchResultPage = context.App.Pages.GetFirstPageAssociatedTo<SearchResultComponentModel>();
        SearchResultComponentModel searchResult = searchResultPage.Components.GetFirstOf<SearchResultComponentModel>();
        context.Page.PreviousPage = searchResult.CurrentPageNumber > 1
            ? $"{searchResult.AssociatedPagePath.Value}?keyword={searchResult.Value}&currentPageNumber={searchResult.CurrentPageNumber}"
            : $"{searchResult.AssociatedPagePath.Value}?keyword={searchResult.Value}";
        
        string key = context.QueryParams.GetQueryParam<string>("key") ?? throw new ComponentException("No key query param found.");
        string value = context.QueryParams.GetQueryParam<string>("value") ?? throw new ComponentException("No value query param found.");
        SearchDetailRequest request = new() { KeyField = key, KeyValue = value };

        ISearchService searchService = _serviceProvider.GetRequiredKeyedService<ISearchService>(searchResultDetail.ConfigKey);
        SearchDetailResponse? response = await searchService.SearchDetailAsync(request);

        if (response is null)
        {
            throw new ComponentException($"Unable to get details for key {key}.");
        }

        searchResultDetail.Result = response.Result;
    }
}