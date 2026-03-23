using Demo.GovUk.Forms.Bankruptcy.Application.DataFlow;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.GovUk.Forms.Bankruptcy.Builders;

public sealed class YourBankruptcyFlowchart : DefineFlowchartBuilder
{
    public override void Construct(IServiceCollection services)
    {
        NodeId bankruptcyDateId = NodeId.New();
        NodeId summaryId = NodeId.New();
        WebRoot webRoot = new();
        
        FormModel form = GetForm(services, webRoot.Root);
        SectionModel section = form.Sections["Bankruptcy"];
            
        DateModel bankruptcyDate = section.Pages.GetFirstOf<DateModel>();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
            
        FlowchartBuilder
            .ForSection(section, services)
            .AddTransitionNode(bankruptcyDateId, bankruptcyDate.Path, summaryId)
            .WithValidator<BankruptcyDateFlowNodeValidator>()
            .Next()
            .AddEndNode(summaryId, summary.Path)
            .WithLoader<SectionSummaryFlowNodeLoader>()
            .WithExecutor<SectionSummaryFlowNodeExecutor>()
            .BuildAndRegister();
    }
}