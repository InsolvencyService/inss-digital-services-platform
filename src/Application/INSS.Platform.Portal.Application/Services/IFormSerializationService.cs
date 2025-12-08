using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Services;

public interface IFormSerializationService
{
    string Serialize(FormModel model);
    FormModel Deserialize(string json);
}