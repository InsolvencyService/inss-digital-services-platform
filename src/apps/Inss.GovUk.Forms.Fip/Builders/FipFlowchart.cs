using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Inss.GovUk.Forms.Fip.Builders;

public sealed class FipFlowchart : DefineFlowchartBuilder
{
    // TODO: Align this with your actual journey and pages defined
    
    public override void Construct(IServiceCollection services)
    {
        NodeId dateId = "Date";
        NodeId summaryId = "Summary";
        WebRoot webRoot = new();
        
        FormModel form = GetForm(services, webRoot.Root);
        SectionModel section = form.Sections["Find an Insolvency Practitioner"];
            
        DateModel bankruptcyDate = section.Pages.GetFirstOf<DateModel>();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
            
        FlowchartBuilder
            .ForSection(section, services)
            .AddTransitionNode(dateId, bankruptcyDate.Path, summaryId)
            .Next()
            .AddEndNode(summaryId, summary.Path)
            .WithLoader<SectionSummaryFlowNodeLoader>()
            .WithExecutor<SectionSummaryFlowNodeExecutor>()
            .BuildAndRegister();
    }
}