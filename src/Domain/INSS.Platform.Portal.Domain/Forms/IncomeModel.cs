using INSS.Platform.Portal.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain.Forms;

public class IncomeModel : FormBase
{
    [Required(ErrorMessage = "Enter source of income")]
    public IncomeSource? SourceOfIncome { get; set; }

    [Required(ErrorMessage = "Enter your gross income")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Gross income must be greater than 0")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal? GrossIncome { get; set; }

    [Required(ErrorMessage = "Enter payment frequency")]
    public PaymentFrequency? PaymentFrequency { get; set; }

    [Required(ErrorMessage = "Enter income provider")]
    public string IncomeProvider { get; set; }
}
