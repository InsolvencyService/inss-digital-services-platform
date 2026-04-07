using System.Text;
using System.Xml.Linq;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Attributes;

namespace Inss.GovUk.Forms.IPUpload.Domain;

public sealed class XmlFileUploadModel : PageModel
{
    public XmlFileUploadModel()
    {
        EncodingType = "multipart/form-data";
    }
    
    [Summary]
    [Copyable]
    public string Filename { get; set; }
    
    [Copyable]
    public string Contents { get; set; }
    
    public int Length { get; set; }
    
    public int SizeInMb => Length / (1024 * 1024);
    
    public XDocument GetXml()
    {
        byte[] xmlBytes = Convert.FromBase64String(Contents);
        string xml = Encoding.UTF8.GetString(xmlBytes);
        return XDocument.Parse(xml);
    }
}