using Inss.Platform.RpsProvider.Domain;

namespace Inss.Platform.RpsProvider.Application.Providers;

public interface IUserAuthStoreProvider
{
    Task StoreAsync(UserAuth user);
    Task<UserAuth?>  GetAsync(string code);
    Task RemoveAsync(string code);
}