using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Services;

public interface IFormStateService
{
    Task<bool> FormExistsAsync(string sessionId);
    
    Task<FormModel> GetAsync(string sessionId);
    
    Task SaveAsync(string sessionId, FormModel model);
}