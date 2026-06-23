using Demo.GovUk.Forms.Business.Application.DataFlow;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.GovUk.Forms.Business.Builders;

public sealed class YourEmployeesFlowchart : DefineFlowchartBuilder
{
    public override void Construct(IServiceCollection services)
    {
        NodeId fullNameId = "EmployeeFullName";
        NodeId ageId = "EmployeeAge";
        NodeId checkDetailsId = "CheckEmployeeDetails";
        NodeId addAnotherId = "AddAnotherEmployee";
        NodeId removeId = "RemoveEmployee";
        NodeId summaryId = "CreditorAndDebtorSummary";
            
        FormModel form = GetForm(services);
        SectionModel section = form.Sections["Employee Details"];
            
        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        AgeModel age = section.Pages.GetFirstOf<AgeModel>();
        CheckAnswersModel checkAnswers = section.Pages.GetFirstOf<CheckAnswersModel>();
        AddAnotherModel addAnother = section.Pages.GetFirstOf<AddAnotherModel>();
        RemoveModel remove = section.Pages.GetFirstOf<RemoveModel>();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
            
        FlowchartBuilder
            .ForSection(section, services)
            .AddTransitionNode(fullNameId, fullName.Path, ageId)
            .WithExecutor<AddAnotherWorkingPageFlowNodeExecutor>()
            .Next()
            .AddTransitionNode(ageId, age.Path, checkDetailsId)
            .WithExecutor<AddAnotherWorkingPageFlowNodeExecutor>()
            .Next()
            .AddTransitionNode(checkDetailsId, checkAnswers.Path, addAnotherId)
            .WithLoader<CheckAnswersFlowNodeLoader>()
            .Next()
            .AddDecisionNode(addAnotherId, addAnother.Path, fullNameId, summaryId)
            .WithLoader<AddAnotherFlowNodeLoader>()
            .WithExecutor<AddAnotherFlowNodeExecutor>()
            .Next()
            .AddTransitionNode(removeId, remove.Path, addAnotherId)
            .WithLoader<AddAnotherRemoveFlowNodeLoader>()
            .WithExecutor<AddAnotherRemoveFlowNodeExecutor>()
            .Next()
            .AddEndNode(summaryId, summary.Path)
            .WithLoader<EmployeeDetailsSummaryFlowNodeLoader>()
            .WithExecutor<SectionSummaryFlowNodeExecutor>()
            .BuildAndRegister();
    }
}