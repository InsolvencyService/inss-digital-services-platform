using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

public sealed class GrossIncomeModel : BaseModel
{
    public GrossIncomeModel()
    {
        PathName = "gross-income";
        Title = "Gross Income";
    }

    [Required(ErrorMessage = "Enter your gross income")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Gross income must be greater than 0")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal? Amount { get; init; }
}
