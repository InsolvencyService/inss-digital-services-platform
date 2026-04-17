using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Inss.GovUk.Forms.IPUpload.Extensions;

public static class XDocumentExtensions
{
    extension(XDocument document)
    {
        public T CreateModel<T>()
        {
            XmlSerializer serializer = new(typeof(T));
            using XmlReader reader = document.Root!.CreateReader();
            return (T)serializer.Deserialize(reader)!;
        }
    }
}