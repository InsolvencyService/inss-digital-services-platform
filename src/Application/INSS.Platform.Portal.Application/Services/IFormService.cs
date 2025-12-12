using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Services;

public interface IFormService
{
    Task<BaseModel> GetAsync(string path);
    Task<BaseModel> StartAsync(string path);
    Task<BaseModel> SaveAsync(BaseModel model);
    Task<BaseModel> AddAsync(string itemId);
    Task<BaseModel> ChangeAsync(string itemId);
    Task<ConfirmModel> RemoveAsync(string itemId);
}