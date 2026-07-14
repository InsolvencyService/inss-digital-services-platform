using Inss.Auth.RpsProvider.Domain.Enums;

namespace Inss.Auth.RpsProvider.Application.Services;

public interface ILoginService
{
    Task<RpsAuthenticationTypes> AuthenticateAsync(string email, string password);
}