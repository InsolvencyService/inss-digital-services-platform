using INSS.Platform.Portal.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

public sealed class SourceOfIncomeModel : BaseModel
{
    public SourceOfIncomeModel()
    {
        PathName = "source-of-income";
        Title = "Source of income";
    }

    [Required(ErrorMessage = "Enter your source of income")]
    public IncomeSource? SourceOfIncome { get; init; }
}
