using Demo.GovUk.Forms.AboutYou.Domain;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Components.Factories;
using GovUk.Forms.Domain;

namespace Demo.GovUk.Forms.AboutYou.Factories;

public sealed class AboutYouFormFactory : IFormFactory
{
    public FormModel Create()
    {
        return FormModelBuilder
            .Create("about-you")
            
            .AddSection("Your Details", "your-details")
            .AddPage<FullNameModel>("Your name", "your-name", submitButtonText: "Continue")
            .AddPage<AddressModel>("Your address", "your-address", submitButtonText: "Continue")
            .AddPage<ContactDetailsModel>("Your contact details", "Your-contact-details", submitButtonText: "Continue")
            .AddPage<AgeModel>("Your age", "your-age", submitButtonText: "Continue")
            .AddPage<SalaryModel>("Your salary", "your-salary", submitButtonText: "Continue")
            .AddPage<BankAccountModel>("Your bank account", "your-bank-account", submitButtonText: "Continue")
            .AddPage<OwnHomeModel>("Own your own home", "your-home-ownership", submitButtonText: "Continue")
            .AddPage<HomeValueModel>("Your home value", "your-home-value", submitButtonText: "Continue")
            .EndSection<SummaryModel>("Your summary", "summary", submitButtonText: "Continue")

            .ValidateAndComplete();
    }
}