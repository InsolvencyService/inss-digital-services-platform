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
            .AddPage<FullNameModel>("Your name", "your-name")
            .AddPage<AddressModel>("Your address", "your-address")
            .AddPage<ContactDetailsModel>("Your contact details", "Your-contact-details")
            .AddPage<AgeModel>("Your age", "your-age")
            .AddPage<SalaryModel>("Your salary", "your-salary")
            .AddPage<BankAccountModel>("Your bank account", "your-bank-account")
            .AddPage<OwnHomeModel>("Own your own home", "your-home-ownership")
            .AddPage<HomeValueModel>("Your home value", "your-home-value")
            .EndSection<SummaryModel>("Your summary", "summary")

            .ValidateAndComplete();
    }
}