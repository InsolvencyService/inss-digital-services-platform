using Inss.Auth.RpsProvider.Application.Clients;
using Inss.Auth.RpsProvider.Domain.Enums;
using Inss.Auth.RpsProvider.Models;
using System.Net;
using System.Text.RegularExpressions;

namespace Inss.Auth.RpsProvider.Infrastructure.Clients;

public sealed class MockUserAuthenticationClient : IUserAuthenticationClient
{
    private readonly HttpClient _client;

    public MockUserAuthenticationClient(HttpClient client)
    {
        _client = client;
    }
    
    public async Task<RpsAuthenticationTypes> AuthenticateAsync(string email, string password)
    {
        // TODO: This is where we will provide the connection to RPS and handle the response
        
        Console.WriteLine("Calling RPS...");
        HttpResponseMessage response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();

        // Test cases...
        return email switch
        {
            "invalid@temp.org" => RpsAuthenticationTypes.Unknown,
            "locked@temp.org" => RpsAuthenticationTypes.Locked,
            _ => RpsAuthenticationTypes.Matched
        };
    }
}



public sealed partial class UserAuthenticationClient : IUserAuthenticationClient
{
    private readonly HttpClient _client;
    private readonly LoginContext _loginContext;

    private readonly string _loginUrl = "https://preprod.ui.report-director-conduct.service.gov.uk/login";

    public UserAuthenticationClient(HttpClient client, LoginContext loginContext)
    {
        _client = client;
        _loginContext = loginContext;
    }

    public async Task<RpsAuthenticationTypes> AuthenticateAsync(string email, string password)//, string loginUrl)
    {
        // Ensure we don't follow redirects, as RPS may redirect on certain responses
        //CookieContainer cookies = new();

        // GET Login page to retrieve CSRF token and initial cookies..
        _loginContext.AllowRedirects = true;
        _client.Timeout = TimeSpan.FromSeconds(30);

        Console.WriteLine("Retrieving login page...");

        HttpResponseMessage loginPageResponse = await _client.GetAsync(_loginUrl);
        string loginPageHtml = await loginPageResponse.Content.ReadAsStringAsync();


        // Extract CSRF token
        string csrfToken = ExtractCsrfToken(loginPageHtml);
        Console.WriteLine($"CSRF token found: {csrfToken}");


        // Build form data
        FormUrlEncodedContent formData = new(
        [
            new KeyValuePair<string, string>("_csrf", csrfToken),
            new KeyValuePair<string, string>("username", email),
            new KeyValuePair<string, string>("password", password)
        ]);

        // POST login
        _loginContext.AllowRedirects = false;
        _client.Timeout = TimeSpan.FromSeconds(30);

        HttpResponseMessage loginResponse = await _client.PostAsync(_loginUrl, formData);


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

        //// Test cases...
        //return email switch
        //{
        //    "invalid@temp.org" => RpsAuthenticationTypes.Unknown,
        //    "locked@temp.org" => RpsAuthenticationTypes.Locked,
        //    _ => RpsAuthenticationTypes.Matched
        //};
    }

    [GeneratedRegex(@"<input[^>]*name=[""']_csrf[""'][^>]*value=[""']([^""']+)[""']", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex CsrfTokenRegex();
    private static string ExtractCsrfToken(string html)
    {
        Match match = CsrfTokenRegex().Match(html);

        if (!match.Success)
        {
            throw new InvalidOperationException("CSRF token not found.");
        }

        return match.Groups[1].Value;
    }



}

public sealed class LoginHttpDelegatingHandler : DelegatingHandler
{
    private readonly LoginContext _loginContext;    
    public LoginHttpDelegatingHandler(LoginContext loginContext)
    {
        _loginContext = loginContext;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
       // AllowAutoRedirect = _loginContext.AllowRedirects;

       HttpClientHandler handler = InnerHandler as HttpClientHandler ?? throw new InvalidOperationException("Inner handler must be HttpClientHandler");
        handler.AllowAutoRedirect = _loginContext.AllowRedirects;
        handler.CookieContainer = new CookieContainer();

        return base.SendAsync(request, cancellationToken);
    }
}