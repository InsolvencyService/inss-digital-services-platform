using GovUk.Forms.Domain;

namespace GovUk.Forms.Application.Services;

public interface ISubmitFormService
{
    Task SubmitAsync(FormModel form, string userSessionId);
}