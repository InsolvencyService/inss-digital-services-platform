using System.Text;
using System.Xml.Linq;
using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Application.Exceptions;
using Inss.GovUk.Forms.IPUpload.Domain.Extensions;

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
            "http://www.ins.gsi.gov.uk/fileupload/rp14a_application" => document.CreateModel<Spreadsheet.RP14A>(),
            "www.inss.gsi.gov.uk/rp14a_application" => document.CreateModel<Api.RP14A>(),
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
    }
    
    private XDocument GetXml()
    {
        byte[] xmlBytes = Convert.FromBase64String(Contents);
        string xml = Encoding.UTF8.GetString(xmlBytes);
        return XDocument.Parse(xml);
    }
}