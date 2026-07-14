using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace Inss.GovUk.Forms.IPUpload.Domain;

public sealed class PostSubmitModel : PageModel
{
    public string ReferenceNumber { get; set; }
    
    public ContentPath DeclarationPagePath { get; set; }
    
    public override void ClearValues()
    {
        base.ClearValues();
        DeclarationPagePath = "/";
    }
}