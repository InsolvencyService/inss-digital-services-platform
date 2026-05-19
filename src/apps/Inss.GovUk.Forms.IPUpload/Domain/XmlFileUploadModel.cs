using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Application.Exceptions;

namespace Inss.GovUk.Forms.IPUpload.Domain;

public sealed class XmlFileUploadModel : PageModel
{
    private const string RP14ASpreadsheetNamespace = "http://www.ins.gsi.gov.uk/fileupload/rp14a_application";
    private const string RP14AApiNamespace = "www.inss.gsi.gov.uk/rp14a_application";
    private const string RP14SpreadsheetNamespace = "http://www.ins.gsi.gov.uk/fileupload/rp14_application";
    private const string RP14ApiNamespace = "www.inss.gsi.gov.uk/rp14_application";
    
    public XmlFileUploadModel()
    {
        EncodingType = "multipart/form-data";
    }
    
    public string Filename { get; set; }
    
    public string Contents { get; set; }
    
    public int Length { get; set; }
    
    public int SizeInMb => Length / (1024 * 1024);

    public static bool IsEmployeeDocument(XDocument document)
    {
        return document.Root?.Name.NamespaceName.ToLower() switch
        {
            RP14ASpreadsheetNamespace => true,
            RP14AApiNamespace => true,
            _ => false
        };
    }
    
    public object GetRedundancyPaymentObject()
    {
        XDocument document = GetXml();
        return document.Root?.Name.NamespaceName.ToLower() switch
        {
            RP14ASpreadsheetNamespace => CreateModel<Employee.Spreadsheet.RP14A>(document),
            RP14AApiNamespace => CreateModel<Employee.Api.RP14A>(document),
            RP14SpreadsheetNamespace => CreateModel<Employer.Spreadsheet.RP14>(document),
            RP14ApiNamespace => CreateModel<Employer.Api.RP14>(document),
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
    
    public XDocument GetXml()
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