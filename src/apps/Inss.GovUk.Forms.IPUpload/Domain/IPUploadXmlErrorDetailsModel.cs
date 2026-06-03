using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;

namespace Inss.GovUk.Forms.IPUpload.Domain;

public sealed class IPUploadXmlErrorDetailsModel : PageModel
{
    internal ErrorPropertySummary CurrentErrorDetail { get; set; }
    
    public override string? GetButtonText()
    {
        return null;
    }
}