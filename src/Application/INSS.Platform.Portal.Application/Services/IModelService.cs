using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace INSS.Platform.Portal.Application.Services;

public interface IModelService<TModel>
{
    Task<TModel> LoadAsync(string? pageUrl);
    Task ValidateAsync(ModelStateDictionary modelState, TModel model);
    Task<string> SaveAsync(string requestPath, TModel model);
    Task<string> GetPageUrlAsync(string? pageUrl, string id);
    Task<string> GetRemovedPageUrlAsync(string? pageUrl, string id);
}