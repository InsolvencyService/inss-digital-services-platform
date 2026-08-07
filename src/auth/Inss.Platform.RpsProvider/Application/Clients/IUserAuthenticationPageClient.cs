namespace Inss.Platform.RpsProvider.Application.Clients;

public interface IUserAuthenticationPageClient
{
    Task<LoginResponse> GetAsync();
}

public sealed class LoginResponse
{
    public string CsrfToken { get; init; }
}