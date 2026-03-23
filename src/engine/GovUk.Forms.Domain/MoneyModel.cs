using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Domain.Attributes;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace GovUk.Forms.Domain;

public sealed class MoneyModel : PageModel
{
    [Copyable]
    [Summary]
    [Required(ErrorMessage = "You must enter a monetary value")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public int Amount { get; set; }
}