using System.Text.RegularExpressions;
using Inss.Auth.RpsProvider.Application.Clients;
using Inss.Auth.RpsProvider.Exceptions;

namespace Inss.Auth.RpsProvider.Infrastructure.Clients;

public sealed partial class UserAuthenticationPageClient : IUserAuthenticationPageClient
{
    private readonly HttpClient _client;

    public UserAuthenticationPageClient(HttpClient client)
    {
        _client = client;
    }
    
    public async Task<LoginResponse> GetAsync()
    {
        HttpResponseMessage loginPageResponse = await _client.GetAsync("/login");
        
        string loginPageHtml = await loginPageResponse.Content.ReadAsStringAsync();
        string csrfToken = ExtractCsrfToken(loginPageHtml);
        return new LoginResponse { CsrfToken = csrfToken };
    }
    
    private static string ExtractCsrfToken(string html)
    {
        Match match = CsrfTokenRegex().Match(html);
        return match.Success ? match.Groups[1].Value : throw new RpsProviderException("CSRF token not found.");
    }
    
    [GeneratedRegex(@"<input[^>]*name=[""']_csrf[""'][^>]*value=[""']([^""']+)[""']", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex CsrfTokenRegex();
}