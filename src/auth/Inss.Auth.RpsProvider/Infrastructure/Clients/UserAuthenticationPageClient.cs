using System.Text.RegularExpressions;
using Inss.Auth.RpsProvider.Application.Clients;

namespace Inss.Auth.RpsProvider.Infrastructure.Clients;

public sealed partial class UserAuthenticationPageClient : IUserAuthenticationPageClient
{
    private readonly HttpClient _client;
    private readonly HttpClientHandler _handler;

    public UserAuthenticationPageClient(HttpClient client, HttpClientHandler handler)
    {
        _client = client;
        _handler = handler;
    }
    
    public async Task<LoginResponse> GetAsync()
    {
        _handler.AllowAutoRedirect = true;
        HttpResponseMessage loginPageResponse = await _client.GetAsync("/login");
        string loginPageHtml = await loginPageResponse.Content.ReadAsStringAsync();
        
        string csrfToken = ExtractCsrfToken(loginPageHtml);
        Console.WriteLine($"CSRF token found: {csrfToken}");

        return new LoginResponse { CsrfToken = csrfToken };
    }
    
    private static string ExtractCsrfToken(string html)
    {
        Match match = CsrfTokenRegex().Match(html);

        if (!match.Success)
        {
            throw new InvalidOperationException("CSRF token not found.");
        }

        return match.Groups[1].Value;
    }
    
    [GeneratedRegex(@"<input[^>]*name=[""']_csrf[""'][^>]*value=[""']([^""']+)[""']", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex CsrfTokenRegex();
}