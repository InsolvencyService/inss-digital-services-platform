using Inss.Platform.Broker.Domain;

namespace Inss.Platform.Broker.Application.Providers;

public interface IAuthCodeStoreProvider
{
    Task StoreAsync(AuthCode authCode);
    Task<AuthCode?>  GetAsync(string id);
    Task RemoveAsync(string id);
}