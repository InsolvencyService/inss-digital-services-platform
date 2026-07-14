using GovUk.Forms.Domain;

namespace Inss.GovUk.Forms.IPUpload.Domain;

public class IPUploadDeclarationModel : PageModel
{
    public bool Accepted { get; set; }

    public override void CopyTo(PageModel target)
    {
        IPUploadDeclarationModel ipUploadDeclaration = target.As<IPUploadDeclarationModel>();
        ipUploadDeclaration.Accepted = Accepted;
    }

    public override void ClearValues()
    {
        base.ClearValues();
        Accepted = false;
    }
}