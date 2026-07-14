using Demo.GovUk.Forms.Business.Application.DataFlow;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.GovUk.Forms.Business.Builders;

public sealed class YourCreditorsAndDebtorsFlowchart : DefineFlowchartBuilder
{
    public override void Construct(IServiceCollection services)
    {
        NodeId creditorFullNameId = "CreditorFullName";
        NodeId creditorAmountId = "CreditorAmount";
        NodeId checkCreditorDetailsId = "CheckCreditorDetails";
        NodeId removeCreditorDetailsId = "RemoveCreditorDetails";
        NodeId addAnotherId = "AddAnotherCreditor";
        NodeId debtorFullNameId = "DebtorFullName";
        NodeId debtorAmountId = "DebtorAmount";
        NodeId checkDebtorDetailsId = "CheckDebtorDetails";
        NodeId removeDebtorDetailsId = "RemoveDebtorDetails";
        NodeId addAnotherDebtorId = "AddAnotherDebtor";
        NodeId summaryId = "EmployeeSummary";
        
        FormModel form = GetForm(services);
        SectionModel section = form.Sections["Creditors and Debtors"];
        
        FullNameModel creditorFullName = section.Pages.GetAtIndex<FullNameModel>(0);
        MoneyModel creditorAmount = section.Pages.GetAtIndex<MoneyModel>(0);
        CheckAnswersModel checkCreditorAnswers = section.Pages.GetAtIndex<CheckAnswersModel>(0);
        RemoveModel removeCreditorDetails = section.Pages.GetAtIndex<RemoveModel>(0);
        AddAnotherModel addAnotherCreditor = section.Pages.GetAtIndex<AddAnotherModel>(0);
        FullNameModel debtorFullName = section.Pages.GetAtIndex<FullNameModel>(1);
        MoneyModel debtorAmount = section.Pages.GetAtIndex<MoneyModel>(1);
        CheckAnswersModel checkDebtorDetails = section.Pages.GetAtIndex<CheckAnswersModel>(1);
        RemoveModel removeDebtorDetails = section.Pages.GetAtIndex<RemoveModel>(1);
        AddAnotherModel addAnotherDebtor = section.Pages.GetAtIndex<AddAnotherModel>(1);
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        
        FlowchartBuilder
            .ForSection(section, services)
            .AddTransitionNode(creditorFullNameId, creditorFullName.Path, creditorAmountId)
            .WithExecutor<AddAnotherWorkingPageFlowNodeExecutor>()
            .Next()
            .AddTransitionNode(creditorAmountId, creditorAmount.Path, checkCreditorDetailsId)
            .WithExecutor<AddAnotherWorkingPageFlowNodeExecutor>()
            .WithValidator<CreditorAmountFlowNodeValidator>()
            .Next()
            .AddTransitionNode(checkCreditorDetailsId, checkCreditorAnswers.Path, addAnotherId)
            .WithLoader<CheckAnswersFlowNodeLoader>()
            .Next()
            .AddDecisionNode(addAnotherId, addAnotherCreditor.Path, creditorFullNameId, debtorFullNameId)
            .WithLoader<AddAnotherFlowNodeLoader>()
            .WithExecutor<AddAnotherFlowNodeExecutor>()
            .Next()
            .AddTransitionNode(removeCreditorDetailsId, removeCreditorDetails.Path, addAnotherId)
            .WithLoader<AddAnotherRemoveFlowNodeLoader>()
            .WithExecutor<AddAnotherRemoveFlowNodeExecutor>()
            .Next()
            .AddTransitionNode(debtorFullNameId, debtorFullName.Path, debtorAmountId)
            .WithExecutor<AddAnotherWorkingPageFlowNodeExecutor>()
            .Next()
            .AddTransitionNode(debtorAmountId, debtorAmount.Path, checkDebtorDetailsId)
            .WithExecutor<AddAnotherWorkingPageFlowNodeExecutor>()
            .WithValidator<DebtorAmountFlowNodeValidator>()
            .Next()
            .AddTransitionNode(checkDebtorDetailsId, checkDebtorDetails.Path, addAnotherDebtorId)
            .WithLoader<CheckAnswersFlowNodeLoader>()
            .Next()
            .AddDecisionNode(addAnotherDebtorId, addAnotherDebtor.Path, debtorFullNameId, summaryId)
            .WithLoader<AddAnotherFlowNodeLoader>()
            .WithExecutor<AddAnotherFlowNodeExecutor>()
            .Next()
            .AddTransitionNode(removeDebtorDetailsId, removeDebtorDetails.Path, addAnotherDebtorId)
            .WithLoader<AddAnotherRemoveFlowNodeLoader>()
            .WithExecutor<AddAnotherRemoveFlowNodeExecutor>()
            .Next()
            .AddEndNode(summaryId, summary.Path)
            .WithLoader<CreditorsAndDebtorSummaryFlowNodeLoader>()
            .WithExecutor<SectionSummaryFlowNodeExecutor>()
            .BuildAndRegister();
    }
}