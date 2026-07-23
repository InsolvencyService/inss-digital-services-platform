using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Application.Exceptions;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using GovUk.Forms.Domain.Search;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace GovUk.Forms.Application.Test.DataFlow.Loading;

public class SearchResultDetailFlowNodeLoaderTests
{
    private readonly SearchResultDetailFlowNodeLoader _searchResultDetailFlowNodeLoader;
    private readonly FormModel _form;
    private readonly SectionModel _section;
    private readonly IFlowchart _flowchart;
    private readonly ISearchService _searchService;
    private readonly ServiceCollection _services = [];
    private readonly NodeId _searchTermId = "SearchTerm";
    private readonly NodeId _searchResultId = "SearchResult";
    private readonly NodeId _searchDetailId = "SearchDetail";
    private readonly NodeId _summaryId = "Summary";
    
    public SearchResultDetailFlowNodeLoaderTests()
    {
        _form = TestFormModels.CreateWithSearchSection();
        _section = _form.Sections["Find people"];
        _searchService = Substitute.For<ISearchService>();
        
        RegisterFlowchart();
        
        IServiceProvider serviceProvider = _services.BuildServiceProvider();
        _flowchart = serviceProvider.GetRequiredKeyedService<IFlowchart>(_section.Path);
        _searchResultDetailFlowNodeLoader = new SearchResultDetailFlowNodeLoader(serviceProvider);
    }

    [Fact]
    public async Task KeyQueryParamNotDefined_LoadAsync_ThrowsException()
    {
        SearchResultDetailModel searchResultDetail = _section.Pages.GetFirstOf<SearchResultDetailModel>();
        FlowNode searchResultNode = _flowchart.Nodes.First(n => n.Id == _searchResultId);
        FlowNodeContext context = new()
        { 
            Nodes = _flowchart.Nodes,
            CurrentNode = searchResultNode,
            Form = _form,
            Section = _section, 
            CurrentPage = searchResultDetail,
            QueryParams = new Dictionary<string, string?> { ["value"] = "Springfield" }
        };

        FlowchartException exception = await Assert.ThrowsAsync<FlowchartException>(
            async () => await _searchResultDetailFlowNodeLoader.LoadAsync(context));
        
        Assert.Equal("No key query param found.", exception.Message);
    }
    
    [Fact]
    public async Task ValueQueryParamNotDefined_LoadAsync_ThrowsException()
    {
        SearchResultDetailModel searchResultDetail = _section.Pages.GetFirstOf<SearchResultDetailModel>();
        FlowNode searchResultNode = _flowchart.Nodes.First(n => n.Id == _searchResultId);
        FlowNodeContext context = new()
        { 
            Nodes = _flowchart.Nodes,
            CurrentNode = searchResultNode,
            Form = _form,
            Section = _section, 
            CurrentPage = searchResultDetail,
            QueryParams = new Dictionary<string, string?> { ["key"] = "Town" }
        };

        FlowchartException exception = await Assert.ThrowsAsync<FlowchartException>(
            async () => await _searchResultDetailFlowNodeLoader.LoadAsync(context));
        
        Assert.Equal("No value query param found.", exception.Message);
    }
    
    [Fact]
    public async Task NullSearchResults_LoadAsync_ThrowsException()
    {
        SearchResultDetailModel searchResultDetail = _section.Pages.GetFirstOf<SearchResultDetailModel>();
        FlowNode searchResultNode = _flowchart.Nodes.First(n => n.Id == _searchResultId);
        FlowNodeContext context = new()
        { 
            Nodes = _flowchart.Nodes,
            CurrentNode = searchResultNode,
            Form = _form,
            Section = _section, 
            CurrentPage = searchResultDetail,
            QueryParams = new Dictionary<string, string?> { ["key"] = "Town", ["value"] = "Springfield" }
        };

        _searchService
            .SearchDetailAsync(Arg.Is<SearchDetailRequest>(r => r.KeyField == "Town" && r.KeyValue == "Springfield"))
            .Returns((SearchDetailResponse?)null);
        
        FlowchartException exception = await Assert.ThrowsAsync<FlowchartException>(
            async () => await _searchResultDetailFlowNodeLoader.LoadAsync(context));
        
        Assert.Equal("Unable to get details for key Town.", exception.Message);
    }
    
    [Fact]
    public async Task PerformSearchDetailLookup_LoadAsync_SetsResultDetail()
    {
        SearchResultDetailModel searchResultDetail = _section.Pages.GetFirstOf<SearchResultDetailModel>();
        searchResultDetail.Result = null!;
        FlowNode searchResultNode = _flowchart.Nodes.First(n => n.Id == _searchResultId);
        FlowNodeContext context = new()
        { 
            Nodes = _flowchart.Nodes,
            CurrentNode = searchResultNode,
            Form = _form,
            Section = _section, 
            CurrentPage = searchResultDetail,
            QueryParams = new Dictionary<string, string?> { ["key"] = "Town", ["value"] = "Springfield" }
        };

        _searchService
            .SearchDetailAsync(Arg.Is<SearchDetailRequest>(r => r.KeyField == "Town" && r.KeyValue == "Springfield"))
            .Returns(new SearchDetailResponse { Result = new SearchResult() });
        
        await _searchResultDetailFlowNodeLoader.LoadAsync(context);
        
        Assert.NotNull(searchResultDetail.Result);
    }
    
    private void RegisterFlowchart()
    {
        SearchTermModel searchTerm = _section.Pages.GetFirstOf<SearchTermModel>();
        SearchResultModel searchResult = _section.Pages.GetFirstOf<SearchResultModel>();
        SearchResultDetailModel searchResultDetail = _section.Pages.GetFirstOf<SearchResultDetailModel>();
        SummaryModel summary = _section.Pages.GetFirstOf<SummaryModel>();
        
        _services.AddKeyedSingleton(searchResult.ConfigKey, _searchService);
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