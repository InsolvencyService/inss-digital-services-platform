using GovUk.Forms.Application.Factories;
using GovUk.Forms.Components.Builders;
using GovUk.Forms.Domain;

namespace Demo.GovUk.Forms.Bankruptcy.Application.Factories;

public sealed class BankruptcyFormFactory : IFormFactory
{
    public FormModel Create()
    {
        return FormModelBuilder
            .Create("bankruptcy")
            
            .AddSection("Bankruptcy", "your-bankruptcy")
            .AddPage<DateModel>("Date you went bankrupt", "date-bankrupt", question: "When did you go bankrupt?", submitButtonText: "Continue")
            .EndSection<SummaryModel>("Bankruptcy summary", "summary", submitButtonText: "Continue")

            .ValidateAndComplete();
    }
}