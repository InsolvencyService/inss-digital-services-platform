using GovUk.Forms.Domain.Attributes;

namespace GovUk.Forms.Domain;

public class StaticHtmlModel : PageModel
{
    [Copyable]
    public string Key { get; init; }
    
    [Copyable]
    public string Html { get; set; }
}