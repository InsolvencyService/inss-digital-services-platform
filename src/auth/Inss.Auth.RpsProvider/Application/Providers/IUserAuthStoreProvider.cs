using Inss.Auth.RpsProvider.Domain;

namespace Inss.Auth.RpsProvider.Application.Providers;

public interface IUserAuthStoreProvider
{
    Task StoreAsync(string code, UserAuth user);
    Task<UserAuth?>  GetAsync(string code);
    Task RemoveAsync(string code);
}