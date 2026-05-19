using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Inss.Common.Exceptions;

namespace Inss.Common.IPUpload;

public static class FileHelper
{
    private const string RP14ASpreadsheetNamespace = "http://www.ins.gsi.gov.uk/fileupload/rp14a_application";
    private const string RP14AApiNamespace = "www.inss.gsi.gov.uk/rp14a_application";
    private const string RP14SpreadsheetNamespace = "http://www.ins.gsi.gov.uk/fileupload/rp14_application";
    private const string RP14ApiNamespace = "www.inss.gsi.gov.uk/rp14_application";
    
    public static bool IsEmployeeDocument(XDocument document)
    {
        return document.Root?.Name.NamespaceName.ToLower() switch
        {
            RP14ASpreadsheetNamespace => true,
            RP14AApiNamespace => true,
            _ => false
        };
    }
    
    public static object GetRedundancyPaymentObject(string contents)
    {
        XDocument document = GetXml(contents);
        return document.Root?.Name.NamespaceName.ToLower() switch
        {
            RP14ASpreadsheetNamespace => CreateModel<Inss.Common.IPUpload.Employee.Spreadsheet.RP14A>(document),
            RP14AApiNamespace => CreateModel<Inss.Common.IPUpload.Employee.Api.RP14A>(document),
            RP14SpreadsheetNamespace => CreateModel<Inss.Common.IPUpload.Employer.Spreadsheet.RP14>(document),
            RP14ApiNamespace => CreateModel<Inss.Common.IPUpload.Employer.Api.RP14>(document),
            _ => throw new CommonException($"Unknown or empty XML schema {document.Root?.Name.NamespaceName} provided.")
        };
    }
    
    public static XDocument GetXml(string contents)
    {
        byte[] xmlBytes = Convert.FromBase64String(contents);
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