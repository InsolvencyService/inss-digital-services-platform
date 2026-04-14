using System.Text;
using System.Xml.Linq;
using GovUk.Forms.Domain;

namespace Inss.GovUk.Forms.IPUpload.Domain;

public sealed class XmlFileUploadModel : PageModel
{
    public XmlFileUploadModel()
    {
        EncodingType = "multipart/form-data";
    }
    
    public string Filename { get; set; }
    
    public string Contents { get; set; }
    
    public int Length { get; set; }
    
    public int SizeInMb => Length / (1024 * 1024);
    
    public XDocument GetXml()
    {
        byte[] xmlBytes = Convert.FromBase64String(Contents);
        string xml = Encoding.UTF8.GetString(xmlBytes);
        return XDocument.Parse(xml);
    }

    public override string[] GetSummaryInfo()
    {
        return [Filename];
    }

    public override void CopyTo(PageModel target)
    {
        XmlFileUploadModel xmlFileUpload = target.As<XmlFileUploadModel>();
        xmlFileUpload.Filename = Filename;
        xmlFileUpload.Contents = Contents;
    }
    
    public override void ClearValues()
    {
        base.ClearValues();
        Filename = string.Empty;
        Contents = string.Empty;
    }
}