using GovUk.Forms.Domain;

namespace Inss.GovUk.Forms.IPUpload.Domain;

public class IPUploadDeclarationModel : StaticHtmlModel
{
    public IPUploadDeclarationModel()
    {
        Key = "ipupload-declaration";
        ViewName = "_StaticHtml";
    }
    
    public bool Accepted { get; set; }

    public override void CopyTo(PageModel target)
    {
        IPUploadDeclarationModel ipUploadDeclaration = target.As<IPUploadDeclarationModel>();
        ipUploadDeclaration.Accepted = Accepted;
    }
}