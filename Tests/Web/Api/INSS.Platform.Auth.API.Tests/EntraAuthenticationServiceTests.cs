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
    public class EntraAuthenticationServiceTests
    {
        private readonly Mock<ILogger<EntraAuthenticationService>> _loggerMock;
        private readonly Mock<ILogger<AuthenticationEventHandler>> _loggerAuthMock;
        private readonly Mock<IOptions<AuthenticationProviderOptions>> _optionsMock;
        private readonly IAuthenticationEventHandler _authEventHandler;
        private readonly AuthenticationProviderOptions _authProviderOptions;
        private readonly EntraAuthenticationService _service;

        public EntraAuthenticationServiceTests()
        {
            _loggerMock = new Mock<ILogger<EntraAuthenticationService>>();

            _authProviderOptions = new AuthenticationProviderOptions
            {
                Entra = new EntraOptions
                {
                    ClientId = "test-client-id",
                    ClientSecret = "test-client-secret",
                    BaseUri = "https://entra.base.uri",
                    Tenant = "test-tenant",
                    SignInCallbackPath = "signin-callback-entra",
                    SignOutCallbackPath = "signout-callback-entra",
                    Scopes = new List<string> { "openid", "profile" }
                },
                AllowedPostSignInRedirectUris = new List<string> { "https://localhost/signin-oidc" },
                AllowedPostSignOutRedirectUris = new List<string> { "https://localhost/signout-callback-entra" }
            };
            _optionsMock = new Mock<IOptions<AuthenticationProviderOptions>>();
            _optionsMock.Setup(x => x.Value).Returns(_authProviderOptions);

            _loggerAuthMock = new Mock<ILogger<AuthenticationEventHandler>>();
            _authEventHandler = new AuthenticationEventHandler(_loggerAuthMock.Object, _optionsMock.Object);

            _service = new EntraAuthenticationService(
                _loggerMock.Object,
                _authEventHandler,
                _optionsMock.Object
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
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains("Entra Authentication - AuthorizationCodeReceivedAsync invoked.")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
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

            Mock<Microsoft.AspNetCore.Authentication.IAuthenticationService> authenticationServiceMock = new();
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
                postLogoutRedirectUri.Should().Contain(_authProviderOptions.Entra.SignOutCallbackPath);
            }
        }
    }
}