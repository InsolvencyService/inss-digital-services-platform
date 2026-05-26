using System.Net;
using Inss.Auth.RpsProvider.Application.Clients;
using Inss.Auth.RpsProvider.Domain.Enums;
using Inss.Auth.RpsProvider.Extensions;

namespace Inss.Auth.RpsProvider.Infrastructure.Clients;

// Cases from: https://github.com/InsolvencyService/insolvency-practitioner-service/blob/develop/ips-ui/src/main/java/uk/gov/insolvency/ipui/CustomAuthenticationFailureHandler.java

public sealed class UserAuthenticationClient : IUserAuthenticationClient
{
    private readonly HttpClient _client;
    private readonly ILogger<UserAuthenticationClient> _logger;

    public UserAuthenticationClient(HttpClient client, ILogger<UserAuthenticationClient> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<RpsAuthenticationTypes> AuthenticateAsync(string email, string password, string csrfToken)
    {
        FormUrlEncodedContent formData = new(
        [
            new KeyValuePair<string, string>("_csrf", csrfToken),
            new KeyValuePair<string, string>("username", email),
            new KeyValuePair<string, string>("password", password)
        ]);
        
        HttpResponseMessage loginResponse = await _client.PostAsync("/login", formData);
        
        if (loginResponse.StatusCode is HttpStatusCode.Found)
        {
            string redirectUrl = loginResponse.Headers.Location?.ToString() ?? string.Empty;
            _logger.LoginResponse(loginResponse.StatusCode.ToString(), redirectUrl);
            
            if (IsLoginSuccess(redirectUrl))
            {
                _logger.LoginSuccessResponse();
                return RpsAuthenticationTypes.Matched;
            }

            if (IsMismatchedCredentials(redirectUrl))
            {
                _logger.LoginFailedResponse();
                return RpsAuthenticationTypes.Unknown;
            }

            if (IsAccountLocked(redirectUrl))
            {
                _logger.LoginAccountLockedResponse();
                return RpsAuthenticationTypes.Locked;
            }
            
            if (IsAccountOutage(redirectUrl))
            {
                _logger.LoginAccountLockedResponse();
                return RpsAuthenticationTypes.Outage;
            }
            _logger.LoginUnknownFailureResponse();
            return RpsAuthenticationTypes.Unknown;
        }

        string responseBody = await loginResponse.Content.ReadAsStringAsync();
        _logger.LoginUnexpectedResponseResponse(responseBody);
        return RpsAuthenticationTypes.Unknown;
    }

    private static bool IsLoginSuccess(string redirectUrl)
    {
        return !string.IsNullOrWhiteSpace(redirectUrl) && redirectUrl.Contains("/ip-hub/hub", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsMismatchedCredentials(string redirectUrl)
    {
        return !string.IsNullOrWhiteSpace(redirectUrl) && 
               redirectUrl.Contains("/login?error=credentials", StringComparison.OrdinalIgnoreCase);
    }
    
    private static bool IsAccountLocked(string redirectUrl)
    {
        return !string.IsNullOrWhiteSpace(redirectUrl) && 
               redirectUrl.Contains("/login?error=locked", StringComparison.OrdinalIgnoreCase);
    }
    
    private static bool IsAccountOutage(string redirectUrl)
    {
        return !string.IsNullOrWhiteSpace(redirectUrl) && 
               redirectUrl.Contains("/login?error=outage", StringComparison.OrdinalIgnoreCase);
    }
}