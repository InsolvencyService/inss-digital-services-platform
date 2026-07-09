using Demo.GovUk.Forms.ContactUs.Application.DataFlow;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Application.DataFlow.Providing;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using GovUk.Forms.Domain.Search;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.GovUk.Forms.ContactUs.Builders;

public sealed class FindPeopleFlowchart : DefineFlowchartBuilder
{
    public override void Construct(IServiceCollection services)
    {
        NodeId searchTermId = "SearchTerm";
        NodeId searchResultId = "SearchResult";
        NodeId searchDetailId = "SearchDetail";
        NodeId summaryId = "Summary";
        
        FormModel form = GetForm(services);
        SectionModel section = form.Sections["Find People"];

        SearchTermModel searchTerm = section.Pages.GetFirstOf<SearchTermModel>();
        SearchResultModel searchResult = section.Pages.GetFirstOf<SearchResultModel>();
        SearchResultDetailModel searchResultDetail = section.Pages.GetFirstOf<SearchResultDetailModel>();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        
        services.AddKeyedTransient<IFlowNodePreviousPathProvider, FindPeopleFlowNodePreviousPathProvider>(section.Path);
        
        FlowchartBuilder
            .ForSection(section, services)
            .AddTransitionNode(searchTermId, searchTerm.Path, searchResultId)
            .WithExecutor<SearchTermFlowNodeExecutor>()
            .Next()
            .AddSpurNode(searchResultId, searchResult.Path, searchResultId, searchDetailId)
            .WithLoader<SearchResultFlowNodeLoader>()
            .WithExecutor<SearchResultFlowNodeExecutor>()
            .Next()
            .AddTransitionNode(searchDetailId, searchResultDetail.Path, searchTermId)
            .WithLoader<SearchResultDetailFlowNodeLoader>()
            .Next()
            .AddEndNode(summaryId, summary.Path)
            .WithLoader<ContactUsSummaryFlowNodeLoader>()
            .WithExecutor<SectionSummaryFlowNodeExecutor>()
            .BuildAndRegister();
    }
}