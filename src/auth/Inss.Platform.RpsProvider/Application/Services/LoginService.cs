using Inss.Platform.RpsProvider.Application.Clients;
using Inss.Platform.RpsProvider.Domain.Enums;

namespace Inss.Platform.RpsProvider.Application.Services;

public sealed class LoginService : ILoginService
{
    private readonly IUserAuthenticationPageClient _userAuthenticationPageClient;
    private readonly IUserAuthenticationClient _userAuthenticationClient;

    public LoginService(IUserAuthenticationPageClient userAuthenticationPageClient, IUserAuthenticationClient userAuthenticationClient)
    {
        _userAuthenticationPageClient = userAuthenticationPageClient;
        _userAuthenticationClient = userAuthenticationClient;
    }

    public async Task<RpsAuthenticationTypes> AuthenticateAsync(string email, string password)
    {
        LoginResponse userLoginPageResponse = await _userAuthenticationPageClient.GetAsync();
        return await _userAuthenticationClient.AuthenticateAsync(email, password, userLoginPageResponse.CsrfToken);
    }
}