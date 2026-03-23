using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Inss.GovUk.Forms.IPUpload.Application.Providers;
using Inss.GovUk.Forms.IPUpload.Infrastructure.Extensions;

namespace Inss.GovUk.Forms.IPUpload.Infrastructure.Providers;

public sealed class XsdProvider : IXsdProvider
{
    public XmlSchemaSet Load(XElement root)
    {
        using Stream stream = typeof(XsdProvider).GetEmbeddedStream($"{root.Name.LocalName}.xsd");
        XmlSchemaSet schemaSet = new();
        schemaSet.Add("www.inss.gsi.gov.uk/RP14A_Application", XmlReader.Create(stream));
        return schemaSet;
    }
}