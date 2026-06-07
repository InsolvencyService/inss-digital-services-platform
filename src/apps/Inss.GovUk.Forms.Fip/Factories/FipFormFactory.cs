using GovUk.Forms.Components.Builders;
using GovUk.Forms.Components.Factories;
using GovUk.Forms.Domain;

namespace Inss.GovUk.Forms.Fip.Factories;

public sealed class FipFormFactory : IFormFactory
{
    public FormModel Create()
    {
        // TODO: A placeholder form for the FIP which needs correct pages added
        // Note that we may not need the summary here as per IP upload
        
        return FormModelBuilder
            .Create("fip")
            
            .AddSection("Find an Insolvency Practitioner", "search")
            .AddPage<DateModel>("Todo", "todo", question: "Todo?", submitButtonText: "Continue")
            .EndSection<SummaryModel>("TODO summary", "summary", submitButtonText: "Continue")

            .ValidateAndComplete();
    }
}