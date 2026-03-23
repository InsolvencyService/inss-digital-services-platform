using System.Text;
using System.Xml.Linq;
using GovUk.Forms.Domain;

namespace Inss.GovUk.Forms.IPUpload.Domain;

public sealed class XmlFileUploadModel : FileUploadModel
{
    public XDocument GetXml()
    {
        byte[] xmlBytes = Convert.FromBase64String(Contents);
        string xml = Encoding.UTF8.GetString(xmlBytes);
        return XDocument.Parse(xml);
    }
}