using Demo.GovUk.Forms.AboutYou.Application.DataFlow;
using Demo.GovUk.Forms.AboutYou.Domain;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.MetaData;
using GovUk.Forms.Domain.Primitives;
using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Demo.GovUk.Forms.AboutYou.StartupConfiguration))]

namespace Demo.GovUk.Forms.AboutYou;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            NodeId fullNameId = "FullName";
            NodeId addressId = "Address";
            NodeId contactDetailsId = "ContactDetails";
            NodeId ageId = "Age";
            NodeId salaryId = "Salary";
            NodeId bankAccountId = "BankAccount";
            NodeId ownHomeId = "OwnsHome";
            NodeId homeValueId = "HomeValue";
            NodeId summaryId = "Summary";
            
            FormBuilder
                .Create(services, "about-you")
                .AddSection("Your Details", "your-details")
                .AddNode<FullNameModel>(fullNameId, "your-name", [addressId])
                .WithMetaData(new QuestionPageMetaData("What is your full name?"))
                .WithMetaData(new HintPageMetaData("Enter your first and last name"))
                .WithMetaData(new ButtonPageMetaData("Continue"))
                .NextNode()
                .AddNode<AddressModel>(addressId, "your-address", [contactDetailsId])
                .WithMetaData(new QuestionPageMetaData("What is your address?"))
                .WithMetaData(new ButtonPageMetaData("Continue"))
                .NextNode()
                .AddNode<ContactDetailsModel>(contactDetailsId, "your-contact-details", [ageId])
                .WithMetaData(new ButtonPageMetaData("Continue"))
                .NextNode()
                .AddNode<AgeModel>(ageId, "your-age", [salaryId, summaryId])
                .WithMetaData(new QuestionPageMetaData("What is your current age?"))
                .WithMetaData(new HintPageMetaData("Enter value between 16 and 80 inclusive"))
                .WithMetaData(new ButtonPageMetaData("Continue"))
                .WithExecutor<YourAgeFlowNodeExecutor>()
                .NextNode()
                .AddNode<SalaryModel>(salaryId, "your-salary", [bankAccountId, summaryId])
                .WithMetaData(new QuestionPageMetaData("What is your annual salary?"))
                .WithMetaData(new HintPageMetaData("Enter value in pounds only"))
                .WithMetaData(new ButtonPageMetaData("Continue"))
                .WithExecutor<YourSalaryFlowNodeExecutor>()
                .NextNode()
                .AddNode<BankAccountModel>(bankAccountId, "your-bank-account", [ownHomeId])
                .WithMetaData(new QuestionPageMetaData("What are your bank or building society account details?"))
                .WithMetaData(new ButtonPageMetaData("Continue"))
                .WithValidator<BankAccountFlowNodeValidator>()
                .NextNode()
                .AddNode<OwnHomeModel>(ownHomeId, "your-home-ownership", [homeValueId, summaryId])
                .WithMetaData(new QuestionPageMetaData("Do you own your own home?"))
                .WithMetaData(new HintPageMetaData("If you do, choose yes"))
                .WithMetaData(new ButtonPageMetaData("Continue"))
                .WithExecutor<OwnHomeFlowNodeExecutor>()
                .NextNode()
                .AddNode<HomeValueModel>(homeValueId, "your-home-value", [summaryId])
                .WithMetaData(new QuestionPageMetaData("What is your home worth?"))
                .WithMetaData(new HintPageMetaData("Enter value in pounds"))
                .WithMetaData(new ButtonPageMetaData("Continue"))
                .NextNode()
                .AddNode<SummaryModel>(summaryId, "summary", [])
                .WithLoader<AboutYouSummaryFlowNodeLoader>()
                .WithExecutor<SectionSummaryFlowNodeExecutor>()
                .NodesDone()
                .RegisterSection()
                .FinalizeForm();
        });
    }
}