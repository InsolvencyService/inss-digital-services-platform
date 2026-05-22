using System.Text.RegularExpressions;
using Inss.Auth.RpsProvider.Application.Clients;

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