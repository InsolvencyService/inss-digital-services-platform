using Azure.Security.KeyVault.Secrets;
using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.Auth.API.Models;
using INSS.Platform.Auth.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Moq;
using System.Security.Claims;

namespace INSS.Platform.Auth.API.Tests
{
    public class OneLoginAuthServiceTests
    {
        private readonly Mock<ILogger<OneLoginAuthService>> _loggerMock;
        private readonly Mock<SecretClient> _secretClientMock;
        private readonly Mock<IOptions<AuthProviderOptions>> _optionsMock;
        private readonly AuthProviderOptions _authProviderOptions;
        private readonly OneLoginAuthService _service;

        public OneLoginAuthServiceTests()
        {
            _loggerMock = new Mock<ILogger<OneLoginAuthService>>();
            _secretClientMock = new Mock<SecretClient>();
            _authProviderOptions = new AuthProviderOptions
            {
                OneLogin = new OneLoginOptions
                {
                    ClientId = "test-client-id",
                    TokenUri = "https://test.token.uri",
                    JwtPrivateKey = TestHelper.GetKeyPem("Keys\\test_private_key.pem"),
                    PostSignOutPath = "signout-callback-oidc",
                    Scopes = new List<string> { "openid", "profile" },
                    BaseUri = "https://test.base.uri"
                },
                AllowedPostSignInRedirectUris = new List<string> { "https://localhost/signin-oidc" },
                AllowedPostSignOutRedirectUris = new List<string> { "https://localhost/signout-callback-oidc" }
            };
            _optionsMock = new Mock<IOptions<AuthProviderOptions>>();
            _optionsMock.Setup(x => x.Value).Returns(_authProviderOptions);

            _service = new OneLoginAuthService(
                _loggerMock.Object,
                _optionsMock.Object,
                _secretClientMock.Object
            );
        }

        [Fact]
        public async Task AuthorizationCodeReceivedAsync_SetsClientAssertion()
        {
            // Arrange
            AuthorizationCodeReceivedContext context = new(
                new DefaultHttpContext(),
                new AuthenticationScheme("oidc", null, typeof(OpenIdConnectHandler)),
                new OpenIdConnectOptions(),
                new AuthenticationProperties()
            )
            {
                TokenEndpointRequest = new OpenIdConnectMessage()
            };

            // Act
            await _service.AuthorizationCodeReceivedAsync(context);

            // Assert
            using (new AssertionScope())
            {
                string? assertionType = context.TokenEndpointRequest.ClientAssertionType;
                string? assertion = context.TokenEndpointRequest.ClientAssertion;

                assertionType.Should().Be("urn:ietf:params:oauth:client-assertion-type:jwt-bearer");
                assertion.Should().NotBeNullOrWhiteSpace();
            }
        }

        [Fact]
        public async Task TokenValidatedAsync_FailsIfPropertiesAreNull()
        {
            // Arrange
            TokenValidatedContext context = new(
                new DefaultHttpContext(),
                new AuthenticationScheme("oidc", null, typeof(OpenIdConnectHandler)),
                new OpenIdConnectOptions(),
                new ClaimsPrincipal(),
                new AuthenticationProperties()
            )
            {
                Properties = null
            };

            // Act
            await _service.TokenValidatedAsync(context);

            // Assert
            using (new AssertionScope())
            {
                context.Result.Succeeded.Should().BeFalse();
                context.Result.Failure.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task TokenValidatedAsync_FailsIfRequiredTokensAreMissing()
        {
            // Arrange
            TokenValidatedContext context = new(
                new DefaultHttpContext(),
                new AuthenticationScheme("oidc", null, typeof(OpenIdConnectHandler)),
                new OpenIdConnectOptions(),
                new ClaimsPrincipal(),
                new AuthenticationProperties()
            )
            {
                TokenEndpointResponse = null
            };

            // Act
            await _service.TokenValidatedAsync(context);

            // Assert
            using (new AssertionScope())
            {
                context.Result.Succeeded.Should().BeFalse();
                context.Result.Failure.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task RedirectToIdentityProviderForSignOutAsync_SetsIdTokenHintAndPostLogoutRedirectUri()
        {
            // Arrange
            AuthenticationProperties properties = new();
            properties.StoreTokens([new AuthenticationToken { Name = "id_token", Value = "test-id-token" }]);

            Mock<IAuthenticationService> authenticationServiceMock = new ();
            AuthenticateResult expectedResult = AuthenticateResult.Success(
                new AuthenticationTicket(
                    new ClaimsPrincipal(new ClaimsIdentity()),
                    properties,
                    CookieAuthenticationDefaults.AuthenticationScheme
                )
            );

            authenticationServiceMock
                .Setup(x => x.AuthenticateAsync(It.IsAny<HttpContext>(), CookieAuthenticationDefaults.AuthenticationScheme))
                .ReturnsAsync(expectedResult);

            ServiceCollection services = new();
            services.AddSingleton(authenticationServiceMock.Object);
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            DefaultHttpContext httpContext = new()
            {
                RequestServices = serviceProvider
            };


            RedirectContext context = new(
                httpContext,
                new AuthenticationScheme("oidc", null, typeof(OpenIdConnectHandler)),
                new OpenIdConnectOptions(),
                properties
            )
            {
                ProtocolMessage = new OpenIdConnectMessage()
            };

            // Act
            await _service.RedirectToIdentityProviderForSignOutAsync(context);

            // Assert
            using (new AssertionScope())
            {
                string? idTokenHint = context.ProtocolMessage.IdTokenHint;
                string? postLogoutRedirectUri = context.ProtocolMessage.PostLogoutRedirectUri;

                idTokenHint.Should().Be("test-id-token");
                postLogoutRedirectUri.Should().Contain(_authProviderOptions.OneLogin.PostSignOutPath);
            }
        }
    }
}