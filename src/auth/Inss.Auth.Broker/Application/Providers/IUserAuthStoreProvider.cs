using Inss.Auth.Broker.Domain;

namespace Inss.Auth.Broker.Application.Providers;

public interface IAuthCodeStoreProvider
{
    Task StoreAsync(string code, AuthCode user);
    Task<AuthCode?>  GetAsync(string code);
    Task RemoveAsync(string code);
}