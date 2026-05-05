using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.Providers;

public interface IFormStorageProvider
{
    Task<bool> ExistsAsync(ContentPath path, string sessionId);
    Task<FormModel> GetAsync(ContentPath path, string sessionId);
    Task SaveAsync(string sessionId, FormModel form);
    Task RemoveAsync(string sessionId, FormModel form);
}