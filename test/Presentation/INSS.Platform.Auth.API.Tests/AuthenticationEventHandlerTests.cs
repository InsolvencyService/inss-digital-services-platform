using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.Auth.API.Models;
using INSS.Platform.Auth.API.Services;
using INSS.Platform.Auth.Application;
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

namespace INSS.Platform.Auth.API.Tests;

public class AuthenticationEventHandlerTests
{
    private readonly Mock<ILogger<AuthenticationEventHandler>> _loggerMock;
    private readonly Mock<IOptions<AuthenticationProviderOptions>> _optionsMock;
    private readonly AuthenticationProviderOptions _providerOptions;
    private readonly AuthenticationEventHandler _handler;

    public AuthenticationEventHandlerTests()
    {
        _loggerMock = new Mock<ILogger<AuthenticationEventHandler>>();

        _providerOptions = new AuthenticationProviderOptions
        {
            Inss = new InssOptions
            {
                Audience = "inss-audience",
                ExpiryMinutes = 5,
                Issuer = "inss-issuer",
                JwtPrivateKey = TestHelper.GetKeyPem("Keys\\test_private_key.pem")
            },
            OneLogin = new OneLoginOptions
            {
                ClientId = "test-client-id",
                TokenUri = "https://test.token.uri",
                JwtPrivateKey = TestHelper.GetKeyPem("Keys\\test_private_key.pem"),
                SignOutCallbackPath = "signout-callback-oidc",
                Scopes = new List<string> { "openid", "profile" },
                BaseUri = "https://test.base.uri"
            },
            Entra = new EntraOptions
            {
                ClientId = "entra-client-id",
                ClientSecret = "entra-client-secret",
                BaseUri = "https://entra.base.uri",
                Tenant = "entra-tenant",
                SignInCallbackPath = "signin-callback-entra",
                SignOutCallbackPath = "signout-callback-entra",
                Scopes = new List<string> { "openid", "profile" }
            },
            AllowedClientRedirectUrls = new List<string> { "https://localhost/signin-oidc" }
        };

        _optionsMock = new Mock<IOptions<AuthenticationProviderOptions>>();
        _optionsMock.Setup(x => x.Value).Returns(_providerOptions);

        _handler = new AuthenticationEventHandler(_loggerMock.Object, _optionsMock.Object);
    }

    [Fact]
    public void HandleAuthorizationCodeReceived_SetsClientAssertion_ForOneLogin()
    {
        AuthorizationCodeReceivedContext context = new(
            new DefaultHttpContext(),
            new AuthenticationScheme("oidc", null, typeof(OpenIdConnectHandler)),
            new OpenIdConnectOptions(),
            new AuthenticationProperties()
        )
        {
            TokenEndpointRequest = new OpenIdConnectMessage()
        };

        _handler.HandleAuthorizationCodeReceived(context, AuthenticationProvider.OneLogin);

        using (new AssertionScope())
        {
            string? assertionType = context.TokenEndpointRequest.ClientAssertionType;
            string? assertion = context.TokenEndpointRequest.ClientAssertion;

            assertionType.Should().Be("urn:ietf:params:oauth:client-assertion-type:jwt-bearer");
            assertion.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public async Task HandleTokenValidatedAsync_FailsIfPropertiesAreNull()
    {
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

        await _handler.HandleTokenValidatedAsync(context, AuthenticationProvider.OneLogin);

        using (new AssertionScope())
        {
            context.Result.Succeeded.Should().BeFalse();
            context.Result.Failure.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task HandleTokenValidatedAsync_FailsIfRequiredTokensAreMissing()
    {
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

        await _handler.HandleTokenValidatedAsync(context, AuthenticationProvider.OneLogin);

        using (new AssertionScope())
        {
            context.Result.Succeeded.Should().BeFalse();
            context.Result.Failure.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task HandleTokenValidatedAsync_SignsInAndRedirectsToClientAsync()
    {
        // Arrange
        Mock<HttpContext> httpContextMock = new();
        Mock<IAuthenticationService> authServiceMock = new();
        
        Mock<IServiceProvider> serviceProviderMock = new();
        serviceProviderMock.Setup(x => x.GetService(typeof(IAuthenticationService))).Returns(authServiceMock.Object);

        httpContextMock.SetupGet(x => x.RequestServices).Returns(serviceProviderMock.Object);
        httpContextMock.SetupGet(x => x.Request).Returns(new DefaultHttpContext().Request);
        httpContextMock.SetupGet(x => x.Response).Returns(new DefaultHttpContext().Response);

        AuthenticationProperties properties = new()
        {
            Items = { { "returnUrl", "https://localhost/client_app" }, { "userId", "test-user-id" } }
        };

        string accessToken = TestHelper.CreateAccessTokenForTest(
            _providerOptions.Inss.JwtPrivateKey,
            _providerOptions.OneLogin.ClientId,
            _providerOptions.OneLogin.TokenUri,
            5);

        TokenValidatedContext context = new(
            httpContextMock.Object,
            new AuthenticationScheme("oidc", null, typeof(IAuthenticationHandler)),
            new OpenIdConnectOptions(),
            new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", "user1")])),
            properties)
        {
            TokenEndpointResponse = new OpenIdConnectMessage
            {
                IdToken = "test-id-token",
                AccessToken = accessToken
            }
        };

        // Act
        await _handler.HandleTokenValidatedAsync(context, AuthenticationProvider.OneLogin);

        // Assert
        using (new AssertionScope())
        {
            List<AuthenticationToken>? tokens = context.Properties?.GetTokens().ToList();
            tokens.Should().ContainSingle(t => t.Name == "id_token" && t.Value == "test-id-token");
            tokens.Should().ContainSingle(t => t.Name == "access_token" && t.Value == accessToken);
            context.Response.StatusCode.Should().Be(StatusCodes.Status302Found);
            context.Response.Headers.Location.ToString().Should().StartWith("https://localhost/client_app?token=");
        }

        authServiceMock.Verify(x =>
            x.SignInAsync(
                httpContextMock.Object,
                CookieAuthenticationDefaults.AuthenticationScheme,
                context.Principal!,
                context.Properties),
            Times.Once);
    }


    [Fact]
    public async Task HandleRedirectToIdentityProviderForSignOutAsync_SetsIdTokenHintAndPostLogoutRedirectUri()
    {
        AuthenticationProperties properties = new();
        properties.StoreTokens([new AuthenticationToken { Name = "id_token", Value = "test-id-token" }]);

        Mock<IAuthenticationService> authenticationServiceMock = new();
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

        await _handler.HandleRedirectToIdentityProviderForSignOutAsync(context, _providerOptions.OneLogin.SignOutCallbackPath, AuthenticationProvider.OneLogin);

        using (new AssertionScope())
        {
            string? idTokenHint = context.ProtocolMessage.IdTokenHint;
            string? postLogoutRedirectUri = context.ProtocolMessage.PostLogoutRedirectUri;

            idTokenHint.Should().Be("test-id-token");
            postLogoutRedirectUri.Should().Contain(_providerOptions.OneLogin.SignOutCallbackPath);
        }
    }

    [Fact]
    public async Task HandleRemoteFailureAsync_SetsStatusCodeAndWritesError()
    {
        RemoteFailureContext context = new(
            new DefaultHttpContext(),
            new AuthenticationScheme("oidc", null, typeof(OpenIdConnectHandler)),
            new OpenIdConnectOptions(),
            new InvalidOperationException("Test failure")
        );

        await _handler.HandleRemoteFailureAsync(context, AuthenticationProvider.OneLogin);

        using (new AssertionScope())
        {
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            context.Response.ContentType.Should().Be("application/json; charset=utf-8");
            context.Result.Handled.Should().BeTrue();
        }

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains("Remote authentication failure: { error = Authentication failed, message = Test failure }")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

    }
}