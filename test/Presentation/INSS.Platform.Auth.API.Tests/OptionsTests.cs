using INSS.Platform.Auth.API.Models;
using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Auth.API.Tests;

public class OptionsTests
{
    [Fact]
    public void Validate_WithValidConfiguration_ReturnsNoErrors()
    {
        // Arrange
        AuthenticationProviderOptions options = new ()
        {
            Entra = new EntraOptions
            {
                SignInCallbackPath = "/signin-entra",
                SignOutCallbackPath = "signout-entra",
                Scopes = ["openid", "profile"]
            },
            OneLogin = new OneLoginOptions
            {
                SignInCallbackPath = "/signin-onelogin",
                SignOutCallbackPath = "signout-onelogin",
                Scopes = ["openid", "profile"]
            },
            AllowedClientRedirectUrls = ["https://example.com"]
        };

        ValidationContext validationContext = new (options);

        // Act
        List<ValidationResult> results = options.Validate(validationContext).ToList();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_WhenEntraSignOutCallbackPathStartsWithSlash_ReturnsValidationError()
    {
        // Arrange
        AuthenticationProviderOptions options = new ()
        {
            Entra = new EntraOptions
            {
                SignInCallbackPath = "/signin-entra",
                SignOutCallbackPath = "/signout-entra",
                Scopes = ["openid"]
            },
            OneLogin = new OneLoginOptions
            {
                SignInCallbackPath = "/signin-onelogin",
                SignOutCallbackPath = "signout-onelogin",
                Scopes = ["openid"]
            },
            AllowedClientRedirectUrls = ["https://example.com"]
        };

        ValidationContext validationContext = new (options);

        // Act
        List<ValidationResult> results = options.Validate(validationContext).ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal("Entra SignOutCallbackPath must not start with a '/'.", results[0].ErrorMessage);
        Assert.Contains("SignOutCallbackPath", results[0].MemberNames);
    }

    [Fact]
    public void Validate_WhenEntraSignInCallbackPathDoesNotStartWithSlash_ReturnsValidationError()
    {
        // Arrange
        AuthenticationProviderOptions options = new ()
        {
            Entra = new EntraOptions
            {
                SignInCallbackPath = "signin-entra",
                SignOutCallbackPath = "signout-entra",
                Scopes = ["openid"]
            },
            OneLogin = new OneLoginOptions
            {
                SignInCallbackPath = "/signin-onelogin",
                SignOutCallbackPath = "signout-onelogin",
                Scopes = ["openid"]
            },
            AllowedClientRedirectUrls = ["https://example.com"]
        };

        ValidationContext validationContext = new (options);

        // Act
        List<ValidationResult> results = options.Validate(validationContext).ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal("Entra SignInCallbackPath must start with a '/'.", results[0].ErrorMessage);
        Assert.Contains("SignInCallbackPath", results[0].MemberNames);
    }

    [Fact]
    public void Validate_WhenEntraScopesIsEmpty_ReturnsValidationError()
    {
        // Arrange
        AuthenticationProviderOptions options = new ()
        {
            Entra = new EntraOptions
            {
                SignInCallbackPath = "/signin-entra",
                SignOutCallbackPath = "signout-entra",
                Scopes = []
            },
            OneLogin = new OneLoginOptions
            {
                SignInCallbackPath = "/signin-onelogin",
                SignOutCallbackPath = "signout-onelogin",
                Scopes = ["openid"]
            },
            AllowedClientRedirectUrls = ["https://example.com"]
        };

        ValidationContext validationContext = new (options);

        // Act
        List<ValidationResult> results = options.Validate(validationContext).ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal("Entra Scopes must not be empty.", results[0].ErrorMessage);
        Assert.Contains("Scopes", results[0].MemberNames);
    }

    [Fact]
    public void Validate_WhenOneLoginSignOutCallbackPathStartsWithSlash_ReturnsValidationError()
    {
        // Arrange
        AuthenticationProviderOptions options = new ()
        {
            Entra = new EntraOptions
            {
                SignInCallbackPath = "/signin-entra",
                SignOutCallbackPath = "signout-entra",
                Scopes = ["openid"]
            },
            OneLogin = new OneLoginOptions
            {
                SignInCallbackPath = "/signin-onelogin",
                SignOutCallbackPath = "/signout-onelogin",
                Scopes = ["openid"]
            },
            AllowedClientRedirectUrls = ["https://example.com"]
        };

        ValidationContext validationContext = new (options);

        // Act
        List<ValidationResult> results = options.Validate(validationContext).ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal("OneLogin SignOutCallbackPath must not start with a '/'.", results[0].ErrorMessage);
        Assert.Contains("SignOutCallbackPath", results[0].MemberNames);
    }

    [Fact]
    public void Validate_WhenOneLoginSignInCallbackPathDoesNotStartWithSlash_ReturnsValidationError()
    {
        // Arrange
        AuthenticationProviderOptions options = new ()
        {
            Entra = new EntraOptions
            {
                SignInCallbackPath = "/signin-entra",
                SignOutCallbackPath = "signout-entra",
                Scopes = ["openid"]
            },
            OneLogin = new OneLoginOptions
            {
                SignInCallbackPath = "signin-onelogin",
                SignOutCallbackPath = "signout-onelogin",
                Scopes = ["openid"]
            },
            AllowedClientRedirectUrls = ["https://example.com"]
        };

        ValidationContext validationContext = new (options);

        // Act
        List<ValidationResult> results = options.Validate(validationContext).ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal("OneLogin SignInCallbackPath must start with a '/'.", results[0].ErrorMessage);
        Assert.Contains("SignInCallbackPath", results[0].MemberNames);
    }

    [Fact]
    public void Validate_WhenOneLoginScopesIsEmpty_ReturnsValidationError()
    {
        // Arrange
        AuthenticationProviderOptions options = new ()
        {
            Entra = new EntraOptions
            {
                SignInCallbackPath = "/signin-entra",
                SignOutCallbackPath = "signout-entra",
                Scopes = ["openid"]
            },
            OneLogin = new OneLoginOptions
            {
                SignInCallbackPath = "/signin-onelogin",
                SignOutCallbackPath = "signout-onelogin",
                Scopes = []
            },
            AllowedClientRedirectUrls = ["https://example.com"]
        };

        ValidationContext validationContext = new (options);

        // Act
        List<ValidationResult> results = options.Validate(validationContext).ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal("OneLoginScopes must not be empty.", results[0].ErrorMessage);
        Assert.Contains("Scopes", results[0].MemberNames);
    }

    [Fact]
    public void Validate_WhenAllowedClientRedirectUrlsIsEmpty_ReturnsValidationError()
    {
        // Arrange
        AuthenticationProviderOptions options = new ()
        {
            Entra = new EntraOptions
            {
                SignInCallbackPath = "/signin-entra",
                SignOutCallbackPath = "signout-entra",
                Scopes = ["openid"]
            },
            OneLogin = new OneLoginOptions
            {
                SignInCallbackPath = "/signin-onelogin",
                SignOutCallbackPath = "signout-onelogin",
                Scopes = ["openid"]
            },
            AllowedClientRedirectUrls = []
        };

        ValidationContext validationContext = new (options);

        // Act
        List<ValidationResult> results = options.Validate(validationContext).ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal("AllowedClientRedirectUrls must not be empty.", results[0].ErrorMessage);
        Assert.Contains("AllowedClientRedirectUrls", results[0].MemberNames);
    }

    [Fact]
    public void Validate_WithMultipleValidationErrors_ReturnsAllErrors()
    {
        // Arrange
        AuthenticationProviderOptions options = new ()
        {
            Entra = new EntraOptions
            {
                SignInCallbackPath = "signin-entra", // Missing leading slash
                SignOutCallbackPath = "/signout-entra", // Has leading slash (invalid)
                Scopes = [] // Empty
            },
            OneLogin = new OneLoginOptions
            {
                SignInCallbackPath = "signin-onelogin", // Missing leading slash
                SignOutCallbackPath = "/signout-onelogin", // Has leading slash (invalid)
                Scopes = [] // Empty
            },
            AllowedClientRedirectUrls = [] // Empty
        };

        ValidationContext validationContext = new (options);

        // Act
        List<ValidationResult> results = options.Validate(validationContext).ToList();

        // Assert
        Assert.Equal(7, results.Count);
        
        Assert.Contains(results, r => r.ErrorMessage == "Entra SignOutCallbackPath must not start with a '/'.");
        Assert.Contains(results, r => r.ErrorMessage == "Entra SignOutCallbackPath must not start with a '/'.");
        Assert.Contains(results, r => r.ErrorMessage == "Entra SignInCallbackPath must start with a '/'.");
        Assert.Contains(results, r => r.ErrorMessage == "Entra Scopes must not be empty.");
        Assert.Contains(results, r => r.ErrorMessage == "OneLogin SignOutCallbackPath must not start with a '/'.");
        Assert.Contains(results, r => r.ErrorMessage == "OneLogin SignInCallbackPath must start with a '/'.");
        Assert.Contains(results, r => r.ErrorMessage == "OneLoginScopes must not be empty.");
        Assert.Contains(results, r => r.ErrorMessage == "AllowedClientRedirectUrls must not be empty.");
    }
}
