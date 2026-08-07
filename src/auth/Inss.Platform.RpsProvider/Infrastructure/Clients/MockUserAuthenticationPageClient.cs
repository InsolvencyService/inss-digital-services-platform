using Inss.Platform.RpsProvider.Application.Clients;

namespace Inss.Platform.RpsProvider.Infrastructure.Clients;

public sealed class MockUserAuthenticationPageClient : IUserAuthenticationPageClient
{
    public Task<LoginResponse> GetAsync()
    {
        return Task.FromResult(new LoginResponse { CsrfToken = "1234" });
    }
}