using GovUk.Forms.Components.Builders;
using GovUk.Forms.Components.Factories;
using GovUk.Forms.Domain;

namespace Demo.GovUk.Forms.Bankruptcy.Factories;

public sealed class BankruptcyFormFactory : IFormFactory
{
    public FormModel Create()
    {
        return FormModelBuilder
            .Create("bankruptcy")
            
            .AddSection("Bankruptcy", "your-bankruptcy")
            .AddPage<DateModel>("Date you went bankrupt", "date-bankrupt", question: "When did you go bankrupt?")
            .AddSummary("Bankruptcy summary", "summary")

            .ValidateAndComplete();
    }
}