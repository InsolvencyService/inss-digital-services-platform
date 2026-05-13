using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.Services;

public interface IUserFormService
{
    Task<FormModel> GetAsync(ContentPath path);
    Task SaveAsync(FormModel form);
    Task SubmitAsync(FormModel form);
    Task RemoveAsync(FormModel form);
}