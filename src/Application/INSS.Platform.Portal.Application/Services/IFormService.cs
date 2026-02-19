using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Services;

[Obsolete("This interface is deprecated and will be removed. See 'FormService.cs'")]
public interface IFormService
{
    Task<BaseModel> GetAsync(string path);
    Task<BaseModel> StartAsync(string path);
    Task<BaseModel> SaveAsync(BaseModel model);
    Task<BaseModel> ChangeAsync(string itemId);
    Task<ConfirmModel> RemoveAsync(string itemId);
    Task<string> GoBackAsync();
}