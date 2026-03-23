using GovUk.Forms.Domain;

namespace GovUk.Forms.Application.Services;

public interface ISubmitSectionService
{
    Task SubmitAsync(SectionModel section, string userSessionId);
}