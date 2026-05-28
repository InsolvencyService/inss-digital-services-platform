using Inss.Auth.RpsProvider.Application.Clients;

namespace Inss.Auth.RpsProvider.Infrastructure.Clients;

public sealed class MockUserAuthenticationPageClient : IUserAuthenticationPageClient
{
    public Task<LoginResponse> GetAsync()
    {
        return Task.FromResult(new LoginResponse { CsrfToken = "1234" });
    }
}