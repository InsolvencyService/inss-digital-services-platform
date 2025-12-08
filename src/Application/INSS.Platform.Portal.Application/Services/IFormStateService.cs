using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Services;

public interface IFormStateService
{
    Task<bool> FormExistsAsync();
    Task<FormModel> GetAsync();
    Task SaveAsync(FormModel model);
}