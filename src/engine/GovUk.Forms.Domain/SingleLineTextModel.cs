using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Domain.Attributes;

namespace GovUk.Forms.Domain;

public sealed class SingleLineTextModel : PageModel
{
    [Copyable]
    [Summary]
    [Required(ErrorMessage = "Enter text")]
    public string Value { get; set; } = string.Empty;
}