using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
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
            .AddTransitionNode(searchId, search.Path, summaryId)
            .WithExecutor<SearchFlowNodeExecutor>()
            .Next()
            .AddEndNode(summaryId, summary.Path)
            .WithLoader<SectionSummaryFlowNodeLoader>()
            .WithExecutor<SectionSummaryFlowNodeExecutor>()
            .BuildAndRegister();
    }
}