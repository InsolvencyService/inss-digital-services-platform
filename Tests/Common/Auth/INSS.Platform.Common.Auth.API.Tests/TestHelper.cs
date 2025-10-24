using INSS.Platform.Common.Auth.Contracts.Request;

namespace INSS.Platform.Common.Auth.API.Tests
{
    internal static class TestHelper
    {
        internal static LoginRequest CreateLoginRequest(string clientUrl = "https://client", string csrfToken = "csrf", string userId = "user")
        {
            return new ()
            {
                ClientUrl = clientUrl,
                CsrfToken = csrfToken,
                UserId = userId
            };
        }

        internal static LogoutRequest CreateLogoutRequest() 
        {
            return new() { IdToken = "idtoken" };
        }
    }
}
