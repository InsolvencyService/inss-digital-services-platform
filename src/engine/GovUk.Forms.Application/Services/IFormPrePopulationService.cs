using GovUk.Forms.Domain;

namespace GovUk.Forms.Application.Services;

public interface IFormPrePopulationService
{
    Task PrePopulateAsync(FormModel form, string userSessionId);
}