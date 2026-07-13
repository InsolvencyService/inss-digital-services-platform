using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using GovUk.Forms.Domain.Search;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace GovUk.Forms.Application.Test.DataFlow.Executing;

public class SearchResultFlowNodeExecutorTests
{
    private readonly SearchResultFlowNodeExecutor _searchResultFlowNodeExecutor = new();
    private readonly FormModel _form;
    private readonly SectionModel _section;
    private readonly IFlowchart _flowchart;
    private readonly ServiceCollection _services = [];
    private readonly NodeId _searchTermId = "SearchTerm";
    private readonly NodeId _searchResultId = "SearchResult";
    private readonly NodeId _searchDetailId = "SearchDetail";
    private readonly NodeId _summaryId = "Summary";

    public SearchResultFlowNodeExecutorTests()
    {
        _form = TestFormModels.CreateWithSearchSection();
        _section = _form.Sections["Find people"];
        
        RegisterFlowchart();
        
        IServiceProvider serviceProvider = _services.BuildServiceProvider();
        _flowchart = serviceProvider.GetRequiredKeyedService<IFlowchart>(_section.Path);
    }
    
    [Fact]
    public async Task PerformingAnotherSearching_ExecuteAsync_ReturnsSameNode()
    {
        FlowNode searchResultNode = _flowchart.Nodes.First(n => n.Id == _searchResultId);
        SearchResultModel searchResult = _section.Pages.GetFirstOf<SearchResultModel>();
        searchResult.SearchText = "Test";
        FlowNodeContext context = new()
        { 
            Nodes = _flowchart.Nodes,
            CurrentNode = searchResultNode,
            Form = _form,
            Section = _section, 
            CurrentPage = searchResult
        };

        NodeId? nextNodeId = await _searchResultFlowNodeExecutor.ExecuteAsync(context);

        Assert.NotNull(nextNodeId);
        Assert.Equal(_searchResultId, nextNodeId);
    }

    [Fact]
    public async Task PerformingSearchWithSingleWord_ExecuteAsync_AddsNewSearchKeywordQueryParam()
    {
        FlowNode searchResultNode = _flowchart.Nodes.First(n => n.Id == _searchResultId);
        SearchResultModel searchResult = _section.Pages.GetFirstOf<SearchResultModel>();
        searchResult.SearchText = "Test";
        FlowNodeContext context = new()
        { 
            Nodes = _flowchart.Nodes,
            CurrentNode = searchResultNode,
            Form = _form,
            Section = _section, 
            CurrentPage = searchResult
        };

        await _searchResultFlowNodeExecutor.ExecuteAsync(context);

        string? searchTerm = context.GetQueryParam<string>("keyword");
        Assert.NotNull(searchTerm);
        Assert.Equal("Test", searchTerm);
    }
    
    [Fact]
    public async Task PerformingSearchWithMultipleWords_ExecuteAsync_AddsNewEncodedSearchKeywordQueryParam()
    {
        FlowNode searchResultNode = _flowchart.Nodes.First(n => n.Id == _searchResultId);
        SearchResultModel searchResult = _section.Pages.GetFirstOf<SearchResultModel>();
        searchResult.SearchText = "Test Team";
        FlowNodeContext context = new()
        { 
            Nodes = _flowchart.Nodes,
            CurrentNode = searchResultNode,
            Form = _form,
            Section = _section, 
            CurrentPage = searchResult
        };

        await _searchResultFlowNodeExecutor.ExecuteAsync(context);

        Assert.Equal("Test+Team", context.QueryParams["keyword"]);
    }
    
    private void RegisterFlowchart()
    {
        SearchTermModel searchTerm = _section.Pages.GetFirstOf<SearchTermModel>();
        SearchResultModel searchResult = _section.Pages.GetFirstOf<SearchResultModel>();
        SearchResultDetailModel searchResultDetail = _section.Pages.GetFirstOf<SearchResultDetailModel>();
        SummaryModel summary = _section.Pages.GetFirstOf<SummaryModel>();
        
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