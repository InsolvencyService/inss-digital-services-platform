using Inss.Auth.RpsProvider.Domain.Enums;

namespace Inss.Auth.RpsProvider.Application.Clients;

public interface IUserAuthenticationClient
{
    Task<RpsAuthenticationTypes> AuthenticateAsync(string email, string password);
}