using System.Net;
using System.Text.RegularExpressions;
using Inss.Auth.RpsProvider.Application.Clients;
using Inss.Auth.RpsProvider.Domain.Enums;

namespace Inss.Auth.RpsProvider.Infrastructure.Clients;

public sealed class UserAuthenticationClient : IUserAuthenticationClient
{
    private readonly HttpClient _client;

    public UserAuthenticationClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<RpsAuthenticationTypes> AuthenticateAsync(string email, string password, string csrfToken)
    {
        // Build form data
        FormUrlEncodedContent formData = new(
        [
            new KeyValuePair<string, string>("_csrf", csrfToken),
            new KeyValuePair<string, string>("username", email),
            new KeyValuePair<string, string>("password", password)
        ]);
        
        // TODO: Do we need this? Is so could be set in StartupConfiguration _client.Timeout = TimeSpan.FromSeconds(30);

        HttpResponseMessage loginResponse = await _client.PostAsync("/login", formData);
        
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
}