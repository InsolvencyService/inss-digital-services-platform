using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Application.Exceptions;

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

    public object GetRedundancyPaymentObject()
    {
        XDocument document = GetXml();
        return document.Root?.Name.NamespaceName.ToLower() switch
        {
            "http://www.ins.gsi.gov.uk/fileupload/rp14a_application" => CreateModel<Employee.Spreadsheet.RP14A>(document),
            "www.inss.gsi.gov.uk/rp14a_application" => CreateModel<Employee.Api.RP14A>(document),
            _ => throw new IPUploadException($"Unknown or empty XML schema {document.Root?.Name.NamespaceName} provided.")
        };
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
        xmlFileUpload.Length = Length;
    }
    
    public override void ClearValues()
    {
        base.ClearValues();
        Filename = string.Empty;
        Contents = string.Empty;
        Length = 0;
    }
    
    private XDocument GetXml()
    {
        byte[] xmlBytes = Convert.FromBase64String(Contents);
        string xml = Encoding.UTF8.GetString(xmlBytes);
        return XDocument.Parse(xml);
    }
    
    private static T CreateModel<T>(XDocument document)
    {
        XmlSerializer serializer = new(typeof(T));
        using XmlReader reader = document.Root!.CreateReader();
        return (T)serializer.Deserialize(reader)!;
    }
}