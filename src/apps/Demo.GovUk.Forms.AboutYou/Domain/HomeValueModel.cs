using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Attributes;

namespace Demo.GovUk.Forms.AboutYou.Domain;

public class HomeValueModel : PageModel
{
    [Copyable]
    [Summary]
    [Required(ErrorMessage = "Enter your home value")]
    [Range(100, 1_000_000, ErrorMessage = "The value must between £100 and £1,000,000")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public int Value { get; set; }
}