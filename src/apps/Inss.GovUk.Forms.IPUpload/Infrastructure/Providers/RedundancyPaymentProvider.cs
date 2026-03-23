using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using GovUk.Forms.IPUpload.Domain;
using Inss.GovUk.Forms.IPUpload.Application.Providers;

namespace Inss.GovUk.Forms.IPUpload.Infrastructure.Providers;

public sealed class RedundancyPaymentProvider : IRedundancyPaymentProvider
{
    public Rp14A Create(XDocument document)
    {
        // TODO: Handle correct model
        XmlSerializer serializer = new(typeof(Rp14A));
        using XmlReader reader = document.Root!.CreateReader();
        return (Rp14A)serializer.Deserialize(reader)!;
    }
}