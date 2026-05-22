using System.Net;
using System.Text.RegularExpressions;
using Inss.Auth.RpsProvider.Application.Clients;
using Inss.Auth.RpsProvider.Domain.Enums;

namespace Inss.Auth.RpsProvider.Infrastructure.Clients;

public sealed partial class UserAuthenticationPageClient : IUserAuthenticationPageClient
{
    private readonly HttpClient _client;
    //private readonly HttpClientHandler _handler;
    //private readonly IHttpClientFactory _httpClientFactory;

    public UserAuthenticationPageClient(HttpClient client)
    {
        _client = client;
        //_handler = handler;
        //_httpClientFactory = httpClientFactory;
    }
    
    public async Task<LoginResponse> GetAsync()
    {
        //_handler.AllowAutoRedirect = true;
        HttpResponseMessage loginPageResponse = await _client.GetAsync("/login");
        
        
        /*// Check if the server sent any cookies
        if (loginPageResponse.Headers.TryGetValues("Set-Cookie", out var cookieHeaders))
        {
            foreach (var cookie in cookieHeaders)
            {
                Console.WriteLine("Cookie: " + cookie);
            }
        }
        else
        {
            Console.WriteLine("No cookies returned.");
        }*/
        
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
/*
public sealed partial class TestAuthClient : IUserAuthenticationClient2
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClientHandler _httpClientHandler;

    public TestAuthClient(IHttpClientFactory httpClientFactory, HttpClientHandler httpClientHandler)
    {
        _httpClientFactory = httpClientFactory;
        _httpClientHandler = httpClientHandler;
    }

    public async Task<RpsAuthenticationTypes> AuthenticateAsync(string email, string password)
    {
        HttpClient client = _httpClientFactory.CreateClient("RPS");
        _httpClientHandler.AllowAutoRedirect = true;
        
        HttpResponseMessage loginPageResponse = await client.GetAsync("/login");
        string loginPageHtml = await loginPageResponse.Content.ReadAsStringAsync();
        
        string csrfToken = ExtractCsrfToken(loginPageHtml);
        Console.WriteLine($"CSRF token found: {csrfToken}");
        
        
        
        
        
        _httpClientHandler.AllowAutoRedirect = false;
        //_handler.CookieContainer = new CookieContainer();
        
        // Build form data
        FormUrlEncodedContent formData = new(
        [
            new KeyValuePair<string, string>("_csrf", csrfToken),
            new KeyValuePair<string, string>("username", email),
            new KeyValuePair<string, string>("password", password)
        ]);
        
        // TODO: Do we need this? Is so could be set in StartupConfiguration _client.Timeout = TimeSpan.FromSeconds(30);

        HttpResponseMessage loginResponse = await client.PostAsync("/login", formData);
        
        // Check Login result
        if (loginResponse.StatusCode is HttpStatusCode.Found or
            HttpStatusCode.Redirect)
        {
            string redirectUrl = loginResponse.Headers.Location?.ToString() ?? string.Empty;
            Console.WriteLine($"Login response: {(int)loginResponse.StatusCode} {loginResponse.StatusCode}");
            Console.WriteLine($"Redirect location: {redirectUrl}");

            // Sucessful login....
            if (!string.IsNullOrWhiteSpace(redirectUrl) &&
                redirectUrl.Contains("/ip-hub/hub", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Login successful - redirected to authenticated area.");
                return RpsAuthenticationTypes.Matched;
            }

            // Failed Login - invalid credentials...
            if (!string.IsNullOrWhiteSpace(redirectUrl) &&
                redirectUrl.Contains("/login?error=credentials", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Login failed - invalid credentials.");
                return RpsAuthenticationTypes.Unknown;
            }

            //Locked...
            //if (!string.IsNullOrWhiteSpace(redirectUrl) &&
            //         redirectUrl.Contains("/login?error=credentials", StringComparison.OrdinalIgnoreCase))
            //{
            //    Console.WriteLine("Login Locked.");
            //    return RpsAuthenticationTypes.Locked;
            //}
            else
            {
                // Something else... invalid!
                Console.WriteLine("Redirect received, but location was not recognised.");
                return RpsAuthenticationTypes.Unknown;
            }
        }
        else
        {
            string responseBody = await loginResponse.Content.ReadAsStringAsync();

            Console.WriteLine("Unexpected login response.");
            Console.WriteLine(responseBody);

            return RpsAuthenticationTypes.Unknown;
        }
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
*/