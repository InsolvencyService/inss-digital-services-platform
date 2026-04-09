using Inss.Auth.RpsProvider.Application.Clients;
using Inss.Auth.RpsProvider.Domain.Enums;

namespace Inss.Auth.RpsProvider.Application.Services;

public sealed class LoginService : ILoginService
{
    private readonly IUserAuthenticationClient _userAuthenticationClient;

    public LoginService(IUserAuthenticationClient userAuthenticationClient)
    {
        _userAuthenticationClient = userAuthenticationClient;
    }

    public async Task<RpsAuthenticationTypes> AuthenticateAsync(string email, string password)
    {
        return await _userAuthenticationClient.AuthenticateAsync(email, password);
    }
}