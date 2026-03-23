using System.Xml.Linq;
using System.Xml.Schema;

namespace Inss.GovUk.Forms.IPUpload.Application.Providers;

public interface IXsdProvider
{
    XmlSchemaSet Load(XElement root);
}