using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Domain.Attributes;

namespace GovUk.Forms.Domain;

public sealed class SalaryModel : PageModel
{
    [Copyable]
    [Summary]
    [Required(ErrorMessage = "You must enter a salary")]
    [Range(1_000, 1_000_000, ErrorMessage = "The salary is not between £1,000 and £1,000,000 inclusive")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public int Value { get; set; }
}