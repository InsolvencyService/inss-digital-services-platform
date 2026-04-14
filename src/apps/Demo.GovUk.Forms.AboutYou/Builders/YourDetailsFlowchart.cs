using Demo.GovUk.Forms.AboutYou.Application.DataFlow;
using Demo.GovUk.Forms.AboutYou.Domain;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.GovUk.Forms.AboutYou.Builders;

public sealed class YourDetailsFlowchart : DefineFlowchartBuilder
{
    public override void Construct(IServiceCollection services)
    {
        NodeId fullNameId = NodeId.New();
        NodeId addressId = NodeId.New();
        NodeId contactDetailsId = NodeId.New();
        NodeId ageId = NodeId.New();
        NodeId salaryId = NodeId.New();
        NodeId bankAccountId = NodeId.New();
        NodeId ownHomeId = NodeId.New();
        NodeId homeValueId = NodeId.New();
        NodeId summaryId = NodeId.New();
        WebRoot webRoot = new();
        
        FormModel form = GetForm(services, webRoot.Root);
        SectionModel section = form.Sections["Your Details"];
        
        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        AddressModel address = section.Pages.GetFirstOf<AddressModel>();
        ContactDetailsModel contactDetails = section.Pages.GetFirstOf<ContactDetailsModel>();
        AgeModel age = section.Pages.GetFirstOf<AgeModel>();
        SalaryModel salary = section.Pages.GetFirstOf<SalaryModel>();
        BankAccountModel bankAccount = section.Pages.GetFirstOf<BankAccountModel>();
        OwnHomeModel ownHome = section.Pages.GetFirstOf<OwnHomeModel>();
        HomeValueModel homeValue = section.Pages.GetFirstOf<HomeValueModel>();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        
        FlowchartBuilder
            .ForSection(section, services)
            .AddTransitionNode(fullNameId, fullName.Path, addressId)
            .Next()
            .AddTransitionNode(addressId, address.Path, contactDetailsId)
            .Next()
            .AddTransitionNode(contactDetailsId, contactDetails.Path, ageId)
            .Next()
            .AddDecisionNode(ageId, age.Path, salaryId, summaryId)
            .WithExecutor<YourAgeFlowNodeExecutor>()
            .Next()
            .AddDecisionNode(salaryId, salary.Path, bankAccountId, summaryId)
            .WithExecutor<YourSalaryFlowNodeExecutor>()
            .Next()
            .AddTransitionNode(bankAccountId, bankAccount.Path, ownHomeId)
            .WithValidator<BankAccountFlowNodeValidator>()
            .Next()
            .AddDecisionNode(ownHomeId, ownHome.Path, homeValueId, summaryId)
            .WithExecutor<OwnHomeFlowNodeExecutor>()
            .Next()
            .AddTransitionNode(homeValueId, homeValue.Path, summaryId)
            .Next()
            .AddEndNode(summaryId, summary.Path)
            .WithLoader<SectionSummaryFlowNodeLoader>()
            .WithExecutor<SectionSummaryFlowNodeExecutor>()
            .BuildAndRegister();
    }
}