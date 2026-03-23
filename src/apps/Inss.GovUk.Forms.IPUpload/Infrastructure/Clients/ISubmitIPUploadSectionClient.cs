using GovUk.Forms.Domain;

namespace Inss.GovUk.Forms.IPUpload.Infrastructure.Clients;

public interface ISubmitIPUploadSectionClient
{
    Task SubmitAsync(SectionModel section, string userSessionId);
}