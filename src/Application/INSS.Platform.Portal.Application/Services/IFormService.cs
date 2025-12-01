using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Services;

public interface IFormService
{
    Task<BaseModel> GetAsync(string path);
    Task<string> SaveAsync(BaseModel model);
    Task<string> ChangeAsync(string itemId);
    Task<ConfirmModel> RemoveAsync(string itemId);
}