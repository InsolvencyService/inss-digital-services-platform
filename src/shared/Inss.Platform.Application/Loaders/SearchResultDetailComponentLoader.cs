using GovUk.Forms.Application.Services;
using Inss.Platform.Application.Providers;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Components.Searching;
using Inss.Platform.Domain.Components.Searching.Support;
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
        ConfigureSearchResultDetailDefinition(context);
        ConfigurePreviousPage(context);
        await PerformSearchAsync(context);
    }

    private void ConfigureSearchResultDetailDefinition(LoaderContext context)
    {
        SearchResultDetailComponentModel searchResultDetail = context.Component.As<SearchResultDetailComponentModel>();
        ISearchConfigProvider searchConfigProvider = _serviceProvider.GetRequiredKeyedService<ISearchConfigProvider>(searchResultDetail.ConfigKey);
        searchResultDetail.Definition = searchConfigProvider.LoadConfig();
    }
    
    private async ValueTask PerformSearchAsync(LoaderContext context)
    {
        SearchResultDetailComponentModel searchResultDetail = context.Component.As<SearchResultDetailComponentModel>();
        string key = context.QueryParams.GetQueryParam<string>("key");
        string value = context.QueryParams.GetQueryParam<string>("value");
        SearchDetailRequest request = new() { KeyField = key, KeyValue = value };
        ISearchService searchService = _serviceProvider.GetRequiredKeyedService<ISearchService>(searchResultDetail.ConfigKey);
        SearchDetailResponse? response = await searchService.SearchDetailAsync(request);

        if (response is null)
        {
            throw new ComponentException($"Unable to get details for key {key}.");
        }

        searchResultDetail.Result = response.Result;
    }
    
    private static void ConfigurePreviousPage(LoaderContext context)
    {
        PageModel searchResultPage = context.App.Pages.GetFirstPageAssociatedTo<SearchResultComponentModel>();
        SearchResultComponentModel searchResult = searchResultPage.Components.GetFirstOf<SearchResultComponentModel>();
        string keyword = context.QueryParams.GetQueryParam<string>("keyword");
        int currentPageNumber = context.QueryParams.GetQueryParam<int>("currentPageNumber");
        context.Page.PreviousPage =  $"{searchResult.AssociatedPagePath.Value}?keyword={keyword}&currentPageNumber={currentPageNumber}";
    }
}