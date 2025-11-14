using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Services;

public interface IFormStateService
{
    Task<FormModel?> GetAsync(string sessionId);
    
    Task SaveAsync(string sessionId, FormModel model);
}