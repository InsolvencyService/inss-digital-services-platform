using Demo.GovUk.Forms.ContactUs.Application.DataFlow;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
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
        NodeId searchId = "Search";
        NodeId summaryId = "Summary";
        
        FormModel form = GetForm(services);
        SectionModel section = form.Sections["Find People"];

        SearchModel search = section.Pages.GetFirstOf<SearchModel>();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        
        FlowchartBuilder
            .ForSection(section, services)
            .AddDecisionNode(searchId, search.Path, searchId, summaryId)
            .WithLoader<SearchFlowNodeLoader>()
            .WithExecutor<SearchFlowNodeExecutor>()
            .Next()
            .AddEndNode(summaryId, summary.Path)
            .WithLoader<ContactUsSummaryFlowNodeLoader>()
            .WithExecutor<SectionSummaryFlowNodeExecutor>()
            .BuildAndRegister();
    }
}

public sealed class FindOtherPeopleFlowchart : DefineFlowchartBuilder
{
    public override void Construct(IServiceCollection services)
    {
        NodeId searchId = "Search";
        NodeId summaryId = "Summary";
        
        FormModel form = GetForm(services);
        SectionModel section = form.Sections["Find Other People"];

        SearchModel search = section.Pages.GetFirstOf<SearchModel>();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        
        FlowchartBuilder
            .ForSection(section, services)
            .AddDecisionNode(searchId, search.Path, searchId, summaryId)
            .WithLoader<SearchFlowNodeLoader>()
            .WithExecutor<SearchFlowNodeExecutor>()
            .Next()
            .AddEndNode(summaryId, summary.Path)
            .WithLoader<ContactUsSummaryFlowNodeLoader>()
            .WithExecutor<SectionSummaryFlowNodeExecutor>()
            .BuildAndRegister();
    }
}