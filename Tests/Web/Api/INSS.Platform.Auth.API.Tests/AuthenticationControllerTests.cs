using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.Auth.API.Controllers;
using INSS.Platform.Auth.API.Models;
using INSS.Platform.Auth.Contracts;
using INSS.Platform.Auth.Contracts.Request;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace INSS.Platform.Auth.API.Tests
{
    public class AuthenticationControllerTests
    {
        private readonly Mock<ILogger<AuthenticationController>> _loggerMock;
        private readonly Mock<IOptions<AuthenticationProviderOptions>> _optionsMock;
        private readonly AuthenticationProviderOptions _options;
        private readonly AuthenticationController _controller;

        public AuthenticationControllerTests()
        {
            _options = new AuthenticationProviderOptions
            {
                AllowedPostSignInRedirectUris = new List<string> { "https://onelogin.client/callback", "https://entra.client/callback" },
                AllowedPostSignOutRedirectUris = new List<string> { "https://onelogin.client/callback", "https://entra.client/callback" },
            };

            _optionsMock = new Mock<IOptions<AuthenticationProviderOptions>>();
            _optionsMock.Setup(x => x.Value).Returns(_options);

            _loggerMock = new Mock<ILogger<AuthenticationController>>();
            _controller = new AuthenticationController(_loggerMock.Object, _optionsMock.Object);
        }

        [Theory]
        [InlineData(AuthenticationProvider.OneLogin, "https://onelogin.client/callback")]
        [InlineData(AuthenticationProvider.Entra, "https://entra.client/callback")]
        public void SignIn_ReturnsChallenge_WhenRedirectUriIsValid(AuthenticationProvider provider, string redirectUri)
        {
            // Arrange
            SignInRequest signInRequest = new()
            {
                PostSignInRedirectUri = redirectUri,
                UserId = "user123"
            };

            // Act
            IActionResult result = _controller.SignIn(provider, signInRequest);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ChallengeResult>();
                ChallengeResult? challenge = result as ChallengeResult;
                challenge!.AuthenticationSchemes.Should().Contain(provider.ToString());
                challenge.Properties!.Items.Should().ContainKey("returnUrl");
                challenge.Properties.Items["returnUrl"].Should().Be(redirectUri);
                challenge.Properties.Items.Should().ContainKey("userId");
                challenge.Properties.Items["userId"].Should().Be("user123");
            }
        }

        [Theory]
        [InlineData(AuthenticationProvider.OneLogin)]
        [InlineData(AuthenticationProvider.Entra)]
        public void SignIn_ReturnsBadRequest_WhenRedirectUriIsInvalid(AuthenticationProvider provider)
        {
            // Arrange
            SignInRequest signInRequest = new()
            {
                PostSignInRedirectUri = "https://malicious/callback",
                UserId = "user123"
            };

            // Act
            IActionResult result = _controller.SignIn(provider, signInRequest);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<BadRequestObjectResult>();
                BadRequestObjectResult? badRequest = result as BadRequestObjectResult;
                badRequest!.Value.Should().Be("Invalid PostSignInRedirectUri");
            }
        }

        [Theory]
        [InlineData(AuthenticationProvider.OneLogin, "https://onelogin.client/callback")]
        [InlineData(AuthenticationProvider.Entra, "https://entra.client/callback")]
        public async Task SignOut_ReturnsSignOutResult_WhenRedirectUriIsValid(AuthenticationProvider provider, string redirectUri)
        {
            // Arrange
            SignOutRequest signOutRequest = new()
            {
                PostSignOutRedirectUri = redirectUri
            };

            // Act
            IActionResult result = await _controller.SignOut(provider, signOutRequest);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<SignOutResult>();
                SignOutResult? signOut = result as SignOutResult;
                signOut!.AuthenticationSchemes.Should().Contain(provider.ToString());
                signOut.Properties!.Items.Should().ContainKey("returnUrl");
                signOut.Properties.Items["returnUrl"].Should().Be(redirectUri);
            }
        }

        [Theory]
        [InlineData(AuthenticationProvider.OneLogin)]
        [InlineData(AuthenticationProvider.Entra)]
        public async Task SignOut_ReturnsBadRequest_WhenRedirectUriIsInvalid(AuthenticationProvider provider)
        {
            // Arrange
            SignOutRequest signOutRequest = new()
            {
                PostSignOutRedirectUri = "https://malicious/signout"
            };

            // Act
            IActionResult result = await _controller.SignOut(provider, signOutRequest);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<BadRequestObjectResult>();
                BadRequestObjectResult? badRequest = result as BadRequestObjectResult;
                badRequest!.Value.Should().Be("Invalid PostSignOutRedirectUri");
            }
        }

        [Theory]
        [InlineData(AuthenticationProvider.OneLogin, "https://onelogin.client/callback")]
        [InlineData(AuthenticationProvider.Entra, "https://entra.client/callback")]
        public async Task PostSignOut_RedirectsToReturnUrl_AndSignsOutCookie(AuthenticationProvider provider, string redirectUri)
        {
            // Arrange
            DefaultHttpContext httpContext = new();
            AuthenticationProperties authProperties = new(new Dictionary<string, string?>
            {
                { "returnUrl", redirectUri }
            });

            AuthenticateResult authenticateResult = AuthenticateResult.Success(new AuthenticationTicket(new System.Security.Claims.ClaimsPrincipal(), authProperties, provider.ToString()));
            Mock<IAuthenticationService> authServiceMock = new();
            authServiceMock.Setup(x => x.AuthenticateAsync(httpContext, "Cookies")).ReturnsAsync(authenticateResult);
            authServiceMock.Setup(x => x.SignOutAsync(httpContext, "Cookies", null)).Returns(Task.CompletedTask);

            httpContext.RequestServices = new ServiceCollection()
                .AddSingleton(authServiceMock.Object)
                .BuildServiceProvider();

            AuthenticationController controller = new(_loggerMock.Object, _optionsMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            // Act
            IActionResult result = await controller.PostSignOut();

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<RedirectResult>();
                RedirectResult? redirect = result as RedirectResult;
                redirect!.Url.Should().Be(redirectUri);
            }
        }
    }
}
