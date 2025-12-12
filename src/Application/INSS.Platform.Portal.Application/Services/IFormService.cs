using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Services;

public interface IFormService
{
    Task<BaseModel> GetAsync(string path);
    Task<string> SaveAsync(BaseModel model);
    Task<string> AddAsync(string itemId);
    Task<string> ChangeAsync(string itemId);
    Task<ConfirmModel> RemoveAsync(string itemId);
}