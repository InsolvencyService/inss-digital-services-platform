using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Domain.Attributes;

namespace GovUk.Forms.Domain;

public sealed class AgeModel : PageModel
{
    [Copyable]
    [Summary]
    [Required(ErrorMessage = "You must enter an age")]
    [Range(16, 80, ErrorMessage = "The age is not between 16 and 80 inclusive")]
    public int Value { get; set; }
}