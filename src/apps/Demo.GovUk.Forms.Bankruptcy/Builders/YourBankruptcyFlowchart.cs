using Demo.GovUk.Forms.Bankruptcy.Application.DataFlow;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.GovUk.Forms.Bankruptcy.Builders;

public sealed class YourBankruptcyFlowchart : DefineFlowchartBuilder
{
    public override void Construct(IServiceCollection services)
    {
        NodeId bankruptcyDateId = "BankruptcyDate";
        NodeId summaryId = "Summary";
        
        FormModel form = GetForm(services);
        SectionModel section = form.Sections["Bankruptcy"];
            
        DateModel bankruptcyDate = section.Pages.GetFirstOf<DateModel>();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
            
        FlowchartBuilder
            .ForSection(section, services)
            .AddTransitionNode(bankruptcyDateId, bankruptcyDate.Path, summaryId)
            .WithValidator<BankruptcyDateFlowNodeValidator>()
            .Next()
            .AddEndNode(summaryId, summary.Path)
            .WithLoader<BankruptcySummaryFlowNodeLoader>()
            .WithExecutor<SectionSummaryFlowNodeExecutor>()
            .BuildAndRegister();
    }
}