using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using GovUk.Forms.Domain.Search;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace GovUk.Forms.Application.Test.DataFlow.Loading;

public class SearchResultFlowNodeLoaderTests
{
    private readonly SearchResultFlowNodeLoader _searchResultFlowNodeLoader;
    private readonly FormModel _form;
    private readonly SectionModel _section;
    private readonly IFlowchart _flowchart;
    private readonly ISearchService _searchService;
    private readonly ISearchConfigProvider _searchConfigProvider;
    private readonly ServiceCollection _services = [];
    private readonly NodeId _searchTermId = "SearchTerm";
    private readonly NodeId _searchResultId = "SearchResult";
    private readonly NodeId _searchDetailId = "SearchDetail";
    private readonly NodeId _summaryId = "Summary";
    
    public SearchResultFlowNodeLoaderTests()
    {
        _form = TestFormModels.CreateWithSearchSection();
        _section = _form.Sections["Find people"];
        _searchService = Substitute.For<ISearchService>();
        _searchService
            .SearchAsync(Arg.Is<SearchRequest>(r => r.SearchText == "Springfield*"))
            .Returns(new SearchResponse { Results = [new SearchResult()] });
        _searchConfigProvider = Substitute.For<ISearchConfigProvider>();
        _searchConfigProvider.LoadConfig().Returns(new SearchDefinition());
        
        RegisterFlowchart();
        
        IServiceProvider serviceProvider = _services.BuildServiceProvider();
        _flowchart = serviceProvider.GetRequiredKeyedService<IFlowchart>(_section.Path);
        _searchResultFlowNodeLoader = new SearchResultFlowNodeLoader(
            serviceProvider, Substitute.For<ILogger<SearchResultFlowNodeLoader>>());
    }

    [Fact]
    public async Task NoSearchKeyword_LoadAsync_ClearsExistingResults()
    {
        SearchResultModel searchResult = _section.Pages.GetFirstOf<SearchResultModel>();
        searchResult.Results = [new SearchResult()];
        FlowNode searchResultNode = _flowchart.Nodes.First(n => n.Id == _searchResultId);
        FlowNodeContext context = new()
        { 
            Nodes = _flowchart.Nodes,
            CurrentNode = searchResultNode,
            Form = _form,
            Section = _section, 
            CurrentPage = searchResult
        };

        await _searchResultFlowNodeLoader.LoadAsync(context);
        
        Assert.Empty(searchResult.Results);
    }
    
    [Fact]
    public async Task SearchKeywordExists_LoadAsync_SetsResults()
    {
        SearchResultModel searchResult = _section.Pages.GetFirstOf<SearchResultModel>();
        searchResult.Results = [new SearchResult()];
        FlowNode searchResultNode = _flowchart.Nodes.First(n => n.Id == _searchResultId);
        FlowNodeContext context = new()
        { 
            Nodes = _flowchart.Nodes,
            CurrentNode = searchResultNode,
            Form = _form,
            Section = _section, 
            CurrentPage = searchResult,
            QueryParams = new Dictionary<string, string?> { ["keyword"] = "Springfield", ["currentPageNumber"] = "1" }
        };

        await _searchResultFlowNodeLoader.LoadAsync(context);
        
        Assert.NotEmpty(searchResult.Results);
    }
    
    private void RegisterFlowchart()
    {
        SearchTermModel searchTerm = _section.Pages.GetFirstOf<SearchTermModel>();
        SearchResultModel searchResult = _section.Pages.GetFirstOf<SearchResultModel>();
        SearchResultDetailModel searchResultDetail = _section.Pages.GetFirstOf<SearchResultDetailModel>();
        SummaryModel summary = _section.Pages.GetFirstOf<SummaryModel>();
        
        _services.AddKeyedSingleton(searchResult.ConfigKey, _searchService);
        _services.AddKeyedSingleton(searchResult.ConfigKey, _searchConfigProvider);
        _services.AddSingleton(Substitute.For<ILogger<Flowchart>>());
        
        FlowchartBuilder
            .ForSection(_section, _services)
            .AddTransitionNode(_searchTermId, searchTerm.Path, _searchResultId)
            .WithExecutor<SearchTermFlowNodeExecutor>()
            .Next()
            .AddSpurNode(_searchResultId, searchResult.Path, _searchResultId, _searchDetailId)
            .WithLoader<SearchResultFlowNodeLoader>()
            .WithExecutor<SearchResultFlowNodeExecutor>()
            .Next()
            .AddTransitionNode(_searchDetailId, searchResultDetail.Path, _searchTermId)
            .WithLoader<SearchResultDetailFlowNodeLoader>()
            .Next()
            .AddEndNode(_summaryId, summary.Path)
            .WithLoader<FindPeopleSummaryFlowNodeLoader>()
            .WithExecutor<SectionSummaryFlowNodeExecutor>()
            .BuildAndRegister();
    }
    
    private sealed class FindPeopleSummaryFlowNodeLoader : SummaryFlowNodeLoader
    {
        public override ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
        {
            return ValueTask.FromResult<NodeId?>(null);
        }
    }
}