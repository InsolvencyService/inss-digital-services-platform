using INSS.Platform.Auth.API.Models;
using INSS.Platform.Auth.API.Services;
using INSS.Platform.Auth.Application;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Moq;
using System.Security.Claims;

namespace INSS.Platform.Auth.API.Tests;

public class AuthenticationBuilderExtensionsTests
{
    [Fact]
    public async Task AddEntraOpenIdConnect_RegistersAuthenticationScheme()
    {
        // Arrange
        ServiceCollection services = new ();
        AuthenticationBuilder builder = services.AddAuthentication();
        EntraOptions entraOptions = new ()
        {
            BaseUri = "https://login.microsoftonline.com",
            Tenant = "test-tenant",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            SignInCallbackPath = "/signin-entra",
            SignOutCallbackPath = "signout-entra",
            Scopes = ["openid", "profile", "email"]
        };

        // Act
        AuthenticationBuilder result = builder.AddEntraOpenIdConnect(entraOptions);

        // Assert
        Assert.NotNull(result);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IAuthenticationSchemeProvider schemeProvider = serviceProvider.GetRequiredService<IAuthenticationSchemeProvider>();
        AuthenticationScheme? scheme = await schemeProvider.GetSchemeAsync("Entra");
        Assert.NotNull(scheme);
        Assert.Equal("Entra", scheme.Name);
    }

    [Fact]
    public void AddEntraOpenIdConnect_ConfiguresOptionsCorrectly()
    {
        // Arrange
        ServiceCollection services = new ();
        AuthenticationBuilder builder = services.AddAuthentication();
        EntraOptions entraOptions = new ()
        {
            BaseUri = "https://login.microsoftonline.com",
            Tenant = "test-tenant-123",
            ClientId = "entra-client-id",
            ClientSecret = "entra-client-secret",
            SignInCallbackPath = "/signin-entra-callback",
            SignOutCallbackPath = "signout-entra-callback",
            Scopes = ["openid", "profile"]
        };

        // Act
        builder.AddEntraOpenIdConnect(entraOptions);

        // Assert
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IOptionsMonitor<OpenIdConnectOptions> optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>();
        OpenIdConnectOptions options = optionsMonitor.Get("Entra");

        Assert.Equal($"{entraOptions.BaseUri}/{entraOptions.Tenant}/v2.0", options.Authority);
        Assert.Equal(entraOptions.ClientId, options.ClientId);
        Assert.Equal(entraOptions.ClientSecret, options.ClientSecret);
        Assert.Equal("code", options.ResponseType);
        Assert.True(options.SaveTokens);
        Assert.Equal(entraOptions.SignInCallbackPath, options.CallbackPath);
        Assert.Equal(2, options.Scope.Count);
        Assert.Contains("openid", options.Scope);
        Assert.Contains("profile", options.Scope);
    }

    [Fact]
    public void AddEntraOpenIdConnect_ConfiguresAllScopes()
    {
        // Arrange
        ServiceCollection services = new ();
        AuthenticationBuilder builder = services.AddAuthentication();
        EntraOptions entraOptions = new ()
        {
            BaseUri = "https://login.microsoftonline.com",
            Tenant = "test-tenant",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            SignInCallbackPath = "/signin-entra",
            SignOutCallbackPath = "signout-entra",
            Scopes = ["openid", "profile", "email", "offline_access"]
        };

        // Act
        builder.AddEntraOpenIdConnect(entraOptions);

        // Assert
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IOptionsMonitor<OpenIdConnectOptions> optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>();
        OpenIdConnectOptions options = optionsMonitor.Get("Entra");

        Assert.Equal(4, options.Scope.Count);
        Assert.Contains("openid", options.Scope);
        Assert.Contains("profile", options.Scope);
        Assert.Contains("email", options.Scope);
        Assert.Contains("offline_access", options.Scope);
    }

    [Fact]
    public void AddEntraOpenIdConnect_ConfiguresEventHandlers()
    {
        // Arrange
        ServiceCollection services = new ();
        AuthenticationBuilder builder = services.AddAuthentication();
        EntraOptions entraOptions = new ()
        {
            BaseUri = "https://login.microsoftonline.com",
            Tenant = "test-tenant",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            SignInCallbackPath = "/signin-entra",
            SignOutCallbackPath = "signout-entra",
            Scopes = ["openid"]
        };

        // Act
        builder.AddEntraOpenIdConnect(entraOptions);

        // Assert
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IOptionsMonitor<OpenIdConnectOptions> optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>();
        OpenIdConnectOptions options = optionsMonitor.Get("Entra");

        Assert.NotNull(options.Events);
        Assert.NotNull(options.Events.OnTokenValidated);
        Assert.NotNull(options.Events.OnRedirectToIdentityProviderForSignOut);
        Assert.NotNull(options.Events.OnRemoteFailure);
    }

    [Fact]
    public async Task AddEntraOpenIdConnect_OnTokenValidated_CallsAuthenticationEventHandler()
    {
        // Arrange
        Mock<IAuthenticationEventHandler> authEventHandlerMock = new ();
        ServiceCollection services = new ();
        services.AddSingleton(authEventHandlerMock.Object);
        AuthenticationBuilder builder = services.AddAuthentication();
        EntraOptions entraOptions = new ()
        {
            BaseUri = "https://login.microsoftonline.com",
            Tenant = "test-tenant",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            SignInCallbackPath = "/signin-entra",
            SignOutCallbackPath = "signout-entra",
            Scopes = ["openid"]
        };

        builder.AddEntraOpenIdConnect(entraOptions);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IOptionsMonitor<OpenIdConnectOptions> optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>();
        OpenIdConnectOptions options = optionsMonitor.Get("Entra");

        DefaultHttpContext httpContext = new()
        {
            RequestServices = serviceProvider
        };
        ClaimsPrincipal principal = new(new ClaimsIdentity());
        AuthenticationProperties properties = new();
        TokenValidatedContext context = new(
            httpContext,
            new AuthenticationScheme("Entra", "Entra", typeof(OpenIdConnectHandler)),
            options,
            principal,
            properties);

        // Act
        await options.Events.OnTokenValidated(context);

        // Assert
        authEventHandlerMock.Verify(
            x => x.HandleTokenValidatedAsync(context, AuthenticationProvider.Entra),
            Times.Once);
    }

    [Fact]
    public async Task AddEntraOpenIdConnect_OnRedirectToIdentityProviderForSignOut_CallsAuthenticationEventHandler()
    {
        // Arrange
        Mock<IAuthenticationEventHandler> authEventHandlerMock = new();
        ServiceCollection services = new ();
        services.AddSingleton(authEventHandlerMock.Object);
        AuthenticationBuilder builder = services.AddAuthentication();
        EntraOptions entraOptions = new ()
        {
            BaseUri = "https://login.microsoftonline.com",
            Tenant = "test-tenant",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            SignInCallbackPath = "/signin-entra",
            SignOutCallbackPath = "signout-entra-callback",
            Scopes = ["openid"]
        };

        builder.AddEntraOpenIdConnect(entraOptions);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IOptionsMonitor<OpenIdConnectOptions> optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>();
        OpenIdConnectOptions options = optionsMonitor.Get("Entra");

        DefaultHttpContext httpContext = new ()
        {
            RequestServices = serviceProvider
        };
        AuthenticationProperties properties = new ();
        RedirectContext context = new (
            httpContext,
            new AuthenticationScheme("Entra", "Entra", typeof(OpenIdConnectHandler)),
            options,
            properties)
        {
            ProtocolMessage = new OpenIdConnectMessage()
        };

        // Act
        await options.Events.OnRedirectToIdentityProviderForSignOut(context);

        // Assert
        authEventHandlerMock.Verify(
            x => x.HandleRedirectToIdentityProviderForSignOutAsync(context, "signout-entra-callback", AuthenticationProvider.Entra),
            Times.Once);
    }

    [Fact]
    public async Task AddEntraOpenIdConnect_OnRemoteFailure_CallsAuthenticationEventHandler()
    {
        // Arrange
        Mock<IAuthenticationEventHandler> authEventHandlerMock = new ();
        ServiceCollection services = new ();
        services.AddSingleton(authEventHandlerMock.Object);
        AuthenticationBuilder builder = services.AddAuthentication();
        EntraOptions entraOptions = new ()
        {
            BaseUri = "https://login.microsoftonline.com",
            Tenant = "test-tenant",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            SignInCallbackPath = "/signin-entra",
            SignOutCallbackPath = "signout-entra",
            Scopes = ["openid"]
        };

        builder.AddEntraOpenIdConnect(entraOptions);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IOptionsMonitor<OpenIdConnectOptions> optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>();
        OpenIdConnectOptions options = optionsMonitor.Get("Entra");

        DefaultHttpContext httpContext = new ()
        {
            RequestServices = serviceProvider
        };
        InvalidOperationException failure = new("Test failure");
        AuthenticationProperties properties = new();
        RemoteFailureContext context = new(
            httpContext,
            new AuthenticationScheme("Entra", "Entra", typeof(OpenIdConnectHandler)),
            options,
            failure);

        // Act
        await options.Events.OnRemoteFailure(context);

        // Assert
        authEventHandlerMock.Verify(
            x => x.HandleRemoteFailureAsync(context, AuthenticationProvider.Entra),
            Times.Once);
    }

    [Fact]
    public async Task AddOneLoginOpenIdConnect_RegistersAuthenticationScheme()
    {
        // Arrange
        ServiceCollection services = new ();
        AuthenticationBuilder builder = services.AddAuthentication();
        OneLoginOptions oneLoginOptions = new ()
        {
            BaseUri = "https://oidc.integration.account.gov.uk",
            ClientId = "onelogin-client-id",
            TokenUri = "https://oidc.integration.account.gov.uk/token",
            JwtPrivateKey = "test-private-key",
            SignInCallbackPath = "/signin-onelogin",
            SignOutCallbackPath = "signout-onelogin",
            Scopes = ["openid"]
        };

        // Act
        AuthenticationBuilder result = builder.AddOneLoginOpenIdConnect(oneLoginOptions);

        // Assert
        Assert.NotNull(result);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IAuthenticationSchemeProvider schemeProvider = serviceProvider.GetRequiredService<IAuthenticationSchemeProvider>();
        AuthenticationScheme? scheme = await schemeProvider.GetSchemeAsync("OneLogin");
        Assert.NotNull(scheme);
        Assert.Equal("OneLogin", scheme.Name);
    }

    [Fact]
    public void AddOneLoginOpenIdConnect_ConfiguresOptionsCorrectly()
    {
        // Arrange
        ServiceCollection services = new ();
        AuthenticationBuilder builder = services.AddAuthentication();
        OneLoginOptions oneLoginOptions = new ()
        {
            BaseUri = "https://oidc.integration.account.gov.uk",
            ClientId = "onelogin-client-123",
            TokenUri = "https://oidc.integration.account.gov.uk/token",
            JwtPrivateKey = "test-key",
            SignInCallbackPath = "/signin-onelogin-callback",
            SignOutCallbackPath = "signout-onelogin-callback",
            Scopes = ["openid", "email"]
        };

        // Act
        builder.AddOneLoginOpenIdConnect(oneLoginOptions);

        // Assert
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IOptionsMonitor<OpenIdConnectOptions> optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>();
        OpenIdConnectOptions options = optionsMonitor.Get("OneLogin");

        Assert.Equal(oneLoginOptions.BaseUri, options.Authority);
        Assert.Equal(oneLoginOptions.ClientId, options.ClientId);
        Assert.Equal("code", options.ResponseType);
        Assert.True(options.SaveTokens);
        Assert.Equal(oneLoginOptions.SignInCallbackPath, options.CallbackPath);
        Assert.Equal(2, options.Scope.Count);
        Assert.Contains("openid", options.Scope);
        Assert.Contains("email", options.Scope);
    }

    [Fact]
    public void AddOneLoginOpenIdConnect_ConfiguresAllScopes()
    {
        // Arrange
        ServiceCollection services = new ();
        AuthenticationBuilder builder = services.AddAuthentication();
        OneLoginOptions oneLoginOptions = new ()
        {
            BaseUri = "https://oidc.integration.account.gov.uk",
            ClientId = "onelogin-client-id",
            TokenUri = "https://oidc.integration.account.gov.uk/token",
            JwtPrivateKey = "test-private-key",
            SignInCallbackPath = "/signin-onelogin",
            SignOutCallbackPath = "signout-onelogin",
            Scopes = ["openid", "email", "phone"]
        };

        // Act
        builder.AddOneLoginOpenIdConnect(oneLoginOptions);

        // Assert
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IOptionsMonitor<OpenIdConnectOptions> optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>();
        OpenIdConnectOptions options = optionsMonitor.Get("OneLogin");

        Assert.Equal(3, options.Scope.Count);
        Assert.Contains("openid", options.Scope);
        Assert.Contains("email", options.Scope);
        Assert.Contains("phone", options.Scope);
    }

    [Fact]
    public void AddOneLoginOpenIdConnect_ConfiguresEventHandlers()
    {
        // Arrange
        ServiceCollection services = new ();
        AuthenticationBuilder builder = services.AddAuthentication();
        OneLoginOptions oneLoginOptions = new ()
        {
            BaseUri = "https://oidc.integration.account.gov.uk",
            ClientId = "onelogin-client-id",
            TokenUri = "https://oidc.integration.account.gov.uk/token",
            JwtPrivateKey = "test-private-key",
            SignInCallbackPath = "/signin-onelogin",
            SignOutCallbackPath = "signout-onelogin",
            Scopes = ["openid"]
        };

        // Act
        builder.AddOneLoginOpenIdConnect(oneLoginOptions);

        // Assert
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IOptionsMonitor<OpenIdConnectOptions> optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>();
        OpenIdConnectOptions options = optionsMonitor.Get("OneLogin");

        Assert.NotNull(options.Events);
        Assert.NotNull(options.Events.OnRedirectToIdentityProvider);
        Assert.NotNull(options.Events.OnAuthorizationCodeReceived);
        Assert.NotNull(options.Events.OnTokenValidated);
        Assert.NotNull(options.Events.OnRedirectToIdentityProviderForSignOut);
        Assert.NotNull(options.Events.OnRemoteFailure);
    }

    [Fact]
    public void AddEntraOpenIdConnect_ReturnsSameBuilderInstance()
    {
        // Arrange
        ServiceCollection services = new ();
        AuthenticationBuilder builder = services.AddAuthentication();
        EntraOptions entraOptions = new ()
        {
            BaseUri = "https://login.microsoftonline.com",
            Tenant = "test-tenant",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            SignInCallbackPath = "/signin-entra",
            SignOutCallbackPath = "signout-entra",
            Scopes = ["openid"]
        };

        // Act
        AuthenticationBuilder result = builder.AddEntraOpenIdConnect(entraOptions);

        // Assert
        Assert.Same(builder, result);
    }

    [Fact]
    public void AddOneLoginOpenIdConnect_ReturnsSameBuilderInstance()
    {
        // Arrange
        ServiceCollection services = new ();
        AuthenticationBuilder builder = services.AddAuthentication();
        OneLoginOptions oneLoginOptions = new ()
        {
            BaseUri = "https://oidc.integration.account.gov.uk",
            ClientId = "onelogin-client-id",
            TokenUri = "https://oidc.integration.account.gov.uk/token",
            JwtPrivateKey = "test-private-key",
            SignInCallbackPath = "/signin-onelogin",
            SignOutCallbackPath = "signout-onelogin",
            Scopes = ["openid"]
        };

        // Act
        AuthenticationBuilder result = builder.AddOneLoginOpenIdConnect(oneLoginOptions);

        // Assert
        Assert.Same(builder, result);
    }

    [Fact]
    public void AddEntraOpenIdConnect_ClearsScopesBeforeAddingNew()
    {
        // Arrange
        ServiceCollection services = new ();
        AuthenticationBuilder builder = services.AddAuthentication();
        EntraOptions entraOptions = new ()
        {
            BaseUri = "https://login.microsoftonline.com",
            Tenant = "test-tenant",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            SignInCallbackPath = "/signin-entra",
            SignOutCallbackPath = "signout-entra",
            Scopes = ["custom-scope-1", "custom-scope-2"]
        };

        // Act
        builder.AddEntraOpenIdConnect(entraOptions);

        // Assert
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IOptionsMonitor<OpenIdConnectOptions> optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>();
        OpenIdConnectOptions options = optionsMonitor.Get("Entra");

        Assert.Equal(2, options.Scope.Count);
        Assert.DoesNotContain("openid", options.Scope); // Default scope should be cleared
        Assert.Contains("custom-scope-1", options.Scope);
        Assert.Contains("custom-scope-2", options.Scope);
    }

    [Fact]
    public void AddOneLoginOpenIdConnect_ClearsScopesBeforeAddingNew()
    {
        // Arrange
        ServiceCollection services = new ();
        AuthenticationBuilder builder = services.AddAuthentication();
        OneLoginOptions oneLoginOptions = new ()
        {
            BaseUri = "https://oidc.integration.account.gov.uk",
            ClientId = "onelogin-client-id",
            TokenUri = "https://oidc.integration.account.gov.uk/token",
            JwtPrivateKey = "test-private-key",
            SignInCallbackPath = "/signin-onelogin",
            SignOutCallbackPath = "signout-onelogin",
            Scopes = ["custom-scope"]
        };

        // Act
        builder.AddOneLoginOpenIdConnect(oneLoginOptions);

        // Assert
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IOptionsMonitor<OpenIdConnectOptions> optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>();
        OpenIdConnectOptions options = optionsMonitor.Get("OneLogin");

        Assert.Single(options.Scope);
        Assert.DoesNotContain("openid", options.Scope); // Default scope should be cleared
        Assert.Contains("custom-scope", options.Scope);
    }

    [Fact]
    public async Task AddOneLoginOpenIdConnect_OnRedirectToIdentityProvider_SetsProtocolMessageParameters()
    {
        // Arrange
        ServiceCollection services = new ();
        AuthenticationBuilder builder = services.AddAuthentication();
        OneLoginOptions oneLoginOptions = new ()
        {
            BaseUri = "https://oidc.integration.account.gov.uk",
            ClientId = "onelogin-client-id",
            TokenUri = "https://oidc.integration.account.gov.uk/token",
            JwtPrivateKey = "test-private-key",
            SignInCallbackPath = "/signin-onelogin",
            SignOutCallbackPath = "signout-onelogin",
            Scopes = ["openid"]
        };

        builder.AddOneLoginOpenIdConnect(oneLoginOptions);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IOptionsMonitor<OpenIdConnectOptions> optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>();
        OpenIdConnectOptions options = optionsMonitor.Get("OneLogin");

        DefaultHttpContext httpContext = new ();
        AuthenticationProperties properties = new ();
        RedirectContext context = new(
            httpContext,
            new AuthenticationScheme("OneLogin", "OneLogin", typeof(OpenIdConnectHandler)),
            options,
            properties)
        {
            ProtocolMessage = new OpenIdConnectMessage()
        };

        // Act
        await options.Events.OnRedirectToIdentityProvider(context);

        // Assert
        Assert.Equal("query", context.ProtocolMessage.ResponseMode);
        Assert.Equal("en", context.ProtocolMessage.GetParameter("ui_locales"));
        Assert.Equal("[\"Cl.Cm\"]", context.ProtocolMessage.GetParameter("vtr"));
    }

    [Fact]
    public async Task AddOneLoginOpenIdConnect_OnAuthorizationCodeReceived_CallsAuthenticationEventHandler()
    {
        // Arrange
        Mock<IAuthenticationEventHandler> authEventHandlerMock = new();
        ServiceCollection services = new ();
        services.AddSingleton(authEventHandlerMock.Object);
        AuthenticationBuilder builder = services.AddAuthentication();
        OneLoginOptions oneLoginOptions = new ()
        {
            BaseUri = "https://oidc.integration.account.gov.uk",
            ClientId = "onelogin-client-id",
            TokenUri = "https://oidc.integration.account.gov.uk/token",
            JwtPrivateKey = "test-private-key",
            SignInCallbackPath = "/signin-onelogin",
            SignOutCallbackPath = "signout-onelogin",
            Scopes = ["openid"]
        };

        builder.AddOneLoginOpenIdConnect(oneLoginOptions);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IOptionsMonitor<OpenIdConnectOptions> optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>();
        OpenIdConnectOptions options = optionsMonitor.Get("OneLogin");

        DefaultHttpContext httpContext = new()
        {
            RequestServices = serviceProvider
        };
        AuthenticationProperties properties = new ();
        AuthorizationCodeReceivedContext context = new (
            httpContext,
            new AuthenticationScheme("OneLogin", "OneLogin", typeof(OpenIdConnectHandler)),
            options,
            properties);

        // Act
        await options.Events.OnAuthorizationCodeReceived(context);

        // Assert
        authEventHandlerMock.Verify(
            x => x.HandleAuthorizationCodeReceived(context, AuthenticationProvider.OneLogin),
            Times.Once);
    }

    [Fact]
    public async Task AddOneLoginOpenIdConnect_OnTokenValidated_CallsAuthenticationEventHandler()
    {
        // Arrange
        Mock<IAuthenticationEventHandler> authEventHandlerMock = new();
        ServiceCollection services = new ();
        services.AddSingleton(authEventHandlerMock.Object);
        AuthenticationBuilder builder = services.AddAuthentication();
        OneLoginOptions oneLoginOptions = new ()
        {
            BaseUri = "https://oidc.integration.account.gov.uk",
            ClientId = "onelogin-client-id",
            TokenUri = "https://oidc.integration.account.gov.uk/token",
            JwtPrivateKey = "test-private-key",
            SignInCallbackPath = "/signin-onelogin",
            SignOutCallbackPath = "signout-onelogin",
            Scopes = ["openid"]
        };

        builder.AddOneLoginOpenIdConnect(oneLoginOptions);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IOptionsMonitor<OpenIdConnectOptions> optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>();
        OpenIdConnectOptions options = optionsMonitor.Get("OneLogin");

        DefaultHttpContext httpContext = new()
        {
            RequestServices = serviceProvider
        };
        ClaimsPrincipal principal = new (new ClaimsIdentity());
        AuthenticationProperties properties = new ();
        TokenValidatedContext context = new (
            httpContext,
            new AuthenticationScheme("OneLogin", "OneLogin", typeof(OpenIdConnectHandler)),
            options,
            principal,
            properties);

        // Act
        await options.Events.OnTokenValidated(context);

        // Assert
        authEventHandlerMock.Verify(
            x => x.HandleTokenValidatedAsync(context, AuthenticationProvider.OneLogin),
            Times.Once);
    }

    [Fact]
    public async Task AddOneLoginOpenIdConnect_OnRedirectToIdentityProviderForSignOut_CallsAuthenticationEventHandler()
    {
        // Arrange
        Mock<IAuthenticationEventHandler> authEventHandlerMock = new();
        ServiceCollection services = new ();
        services.AddSingleton(authEventHandlerMock.Object);
        AuthenticationBuilder builder = services.AddAuthentication();
        OneLoginOptions oneLoginOptions = new()
        {
            BaseUri = "https://oidc.integration.account.gov.uk",
            ClientId = "onelogin-client-id",
            TokenUri = "https://oidc.integration.account.gov.uk/token",
            JwtPrivateKey = "test-private-key",
            SignInCallbackPath = "/signin-onelogin",
            SignOutCallbackPath = "signout-onelogin-callback",
            Scopes = ["openid"]
        };

        builder.AddOneLoginOpenIdConnect(oneLoginOptions);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IOptionsMonitor<OpenIdConnectOptions> optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>();
        OpenIdConnectOptions options = optionsMonitor.Get("OneLogin");

        DefaultHttpContext httpContext = new()
        {
            RequestServices = serviceProvider
        };
        AuthenticationProperties properties = new ();
        RedirectContext context = new(
            httpContext,
            new AuthenticationScheme("OneLogin", "OneLogin", typeof(OpenIdConnectHandler)),
            options,
            properties)
        {
            ProtocolMessage = new OpenIdConnectMessage()
        };

        // Act
        await options.Events.OnRedirectToIdentityProviderForSignOut(context);

        // Assert
        authEventHandlerMock.Verify(
            x => x.HandleRedirectToIdentityProviderForSignOutAsync(context, "signout-onelogin-callback", AuthenticationProvider.OneLogin),
            Times.Once);
    }

    [Fact]
    public async Task AddOneLoginOpenIdConnect_OnRemoteFailure_CallsAuthenticationEventHandler()
    {
        // Arrange
        Mock<IAuthenticationEventHandler> authEventHandlerMock = new();
        ServiceCollection services = new ();
        services.AddSingleton(authEventHandlerMock.Object);
        AuthenticationBuilder builder = services.AddAuthentication();
        OneLoginOptions oneLoginOptions = new ()
        {
            BaseUri = "https://oidc.integration.account.gov.uk",
            ClientId = "onelogin-client-id",
            TokenUri = "https://oidc.integration.account.gov.uk/token",
            JwtPrivateKey = "test-private-key",
            SignInCallbackPath = "/signin-onelogin",
            SignOutCallbackPath = "signout-onelogin",
            Scopes = ["openid"]
        };

        builder.AddOneLoginOpenIdConnect(oneLoginOptions);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IOptionsMonitor<OpenIdConnectOptions> optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>();
        OpenIdConnectOptions options = optionsMonitor.Get("OneLogin");

        DefaultHttpContext httpContext = new()
        {
            RequestServices = serviceProvider
        };
        InvalidOperationException failure = new ("Test failure");
        AuthenticationProperties properties = new ();
        RemoteFailureContext context = new (
            httpContext,
            new AuthenticationScheme("OneLogin", "OneLogin", typeof(OpenIdConnectHandler)),
            options,
            failure);

        // Act
        await options.Events.OnRemoteFailure(context);

        // Assert
        authEventHandlerMock.Verify(
            x => x.HandleRemoteFailureAsync(context, AuthenticationProvider.OneLogin),
            Times.Once);
    }
}
