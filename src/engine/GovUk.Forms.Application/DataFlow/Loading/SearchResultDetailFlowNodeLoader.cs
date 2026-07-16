using GovUk.Forms.Application.Exceptions;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain.Primitives;
using GovUk.Forms.Domain.Search;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.Application.DataFlow.Loading;

public sealed class SearchResultDetailFlowNodeLoader : IFlowNodeLoader
{
    private readonly IServiceProvider _serviceProvider;

    public SearchResultDetailFlowNodeLoader(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        SearchResultDetailModel searchResultDetail = context.CurrentPage.As<SearchResultDetailModel>();
        SearchResultModel searchResult = context.Section.Pages.GetFirstOf<SearchResultModel>();
        searchResultDetail.Definition = searchResult.Definition;
        
        string key = context.GetQueryParam<string>("key") ?? throw new FlowchartException("No key query param found.");
        string value = context.GetQueryParam<string>("value") ?? throw new FlowchartException("No key query param found.");
        SearchDetailRequest request = new() { KeyField = key, KeyValue = value };

        ISearchService searchService = _serviceProvider.GetRequiredKeyedService<ISearchService>(searchResult.ConfigKey);
        SearchDetailResponse? response = await searchService.SearchDetailAsync(request);

        if (response is null)
        {
            throw new FlowchartException($"Unable to get details for key {key}.");
        }

        searchResultDetail.Result = response.Result;
        return null;
    }
}