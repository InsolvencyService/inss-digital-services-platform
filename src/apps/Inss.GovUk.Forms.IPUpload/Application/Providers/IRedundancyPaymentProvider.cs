using System.Xml.Linq;
using GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.Providers;

public interface IRedundancyPaymentProvider
{
    RP14A Create(XDocument document);
}