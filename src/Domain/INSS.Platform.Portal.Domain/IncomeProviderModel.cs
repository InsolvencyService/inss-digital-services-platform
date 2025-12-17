using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

public sealed class IncomeProviderModel : BaseModel
{
    public IncomeProviderModel()
    {
        PathName = "income-provider";
        Title = "Income Provider";
    }
    
    [Required(ErrorMessage = "Enter the income provider")]
    public string IncomeProvider { get; init; } = string.Empty;
}