using GovUk.Forms.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.Services;

public interface ISubmitUploadedXmlService
{
    Task<string> SubmitAsync(SectionModel section, string userId);
}