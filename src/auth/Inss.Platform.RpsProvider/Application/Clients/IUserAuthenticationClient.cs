using Inss.Platform.RpsProvider.Domain.Enums;

namespace Inss.Platform.RpsProvider.Application.Clients;

public interface IUserAuthenticationClient
{
    Task<RpsAuthenticationTypes> AuthenticateAsync(string email, string password, string csrfToken);
}