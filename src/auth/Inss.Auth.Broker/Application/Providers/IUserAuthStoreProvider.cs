using Inss.Auth.Broker.Domain;

namespace Inss.Auth.Broker.Application.Providers;

public interface IAuthCodeStoreProvider
{
    Task StoreAsync(AuthCode authCode);
    Task<AuthCode?>  GetAsync(string id);
    Task RemoveAsync(string id);
}