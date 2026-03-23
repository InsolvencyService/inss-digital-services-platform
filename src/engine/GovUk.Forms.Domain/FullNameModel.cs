using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Domain.Attributes;

namespace GovUk.Forms.Domain;

public sealed class FullNameModel : PageModel
{
    [Copyable]
    [Summary]
    [Required(ErrorMessage = "Enter your full name")]
    [RegularExpression(@"^[A-Za-z\s\-']+$", ErrorMessage = "Full name can only contain letters, spaces, hyphens and apostrophes")]
    public string Value { get; set; } = string.Empty;
}