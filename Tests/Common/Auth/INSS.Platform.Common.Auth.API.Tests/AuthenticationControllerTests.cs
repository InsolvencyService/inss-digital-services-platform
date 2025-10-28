using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.Common.Auth.API.Controllers;
using INSS.Platform.Common.Auth.API.Services;
using INSS.Platform.Common.Auth.Contracts.Request;
using INSS.Platform.Common.Auth.Contracts.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace INSS.Platform.Common.Auth.API.Tests
{
    public class AuthenticationControllerTests
    {
        private readonly Mock<ILogger<AuthenticationController>> _loggerMock;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthenticationController _controller;

        public AuthenticationControllerTests()
        {
            _loggerMock = new Mock<ILogger<AuthenticationController>>();
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthenticationController(_loggerMock.Object, _authServiceMock.Object);
        }

        [Fact]
        public async Task LoginAsync_ReturnsRedirect_WhenSuccess()
        {
            // Arrange
            LoginRequest request = TestHelper.CreateLoginRequest();
            _authServiceMock.Setup(x => x.GetLoginRedirectUrlAsync(request)).ReturnsAsync("https://login");

            // Act
            IActionResult result = await _controller.LoginAsync(request);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<RedirectResult>();
                RedirectResult? redirect = result as RedirectResult;
                redirect.Should().NotBeNull();
                redirect!.Url.Should().Be("https://login");
            }
        }

        [Fact]
        public async Task LoginAsync_ReturnsError_WhenException()
        {
            LoginRequest request = TestHelper.CreateLoginRequest();
            _authServiceMock.Setup(x => x.GetLoginRedirectUrlAsync(request)).ThrowsAsync(new InvalidOperationException("fail"));

            IActionResult result = await _controller.LoginAsync(request);

            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult? objectResult = result as ObjectResult;
                objectResult.Should().NotBeNull();
                objectResult!.StatusCode.Should().Be(500);
            }
        }

        [Fact]
        public async Task LoginUrlAsync_ReturnsOkWithUrl_WhenSuccess()
        {
            LoginRequest request = TestHelper.CreateLoginRequest();
            _authServiceMock.Setup(x => x.GetLoginRedirectUrlAsync(request)).ReturnsAsync("https://login");

            IActionResult result = await _controller.LoginUrlAsync(request);

            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                OkObjectResult? okResult = result as OkObjectResult;
                okResult.Should().NotBeNull();
                okResult!.Value.Should().Be("https://login");
            }
        }

        [Fact]
        public async Task LoginUrlAsync_ReturnsError_WhenException()
        {
            LoginRequest request = TestHelper.CreateLoginRequest();
            _authServiceMock.Setup(x => x.GetLoginRedirectUrlAsync(request)).ThrowsAsync(new InvalidOperationException("fail"));

            IActionResult result = await _controller.LoginUrlAsync(request);

            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult? objectResult = result as ObjectResult;
                objectResult.Should().NotBeNull();
                objectResult!.StatusCode.Should().Be(500);
            }
        }

        [Fact]
        public async Task LogOutAsync_ReturnsOk_WhenSuccess()
        {
            LogoutRequest request = TestHelper.CreateLogoutRequest();
            _authServiceMock.Setup(x => x.LogoutAsync(request)).ReturnsAsync(true);

            IActionResult result = await _controller.LogOutAsync(request);

            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                OkObjectResult? okResult = result as OkObjectResult;
                okResult.Should().NotBeNull();
                okResult!.Value.Should().Be("Logout successful.");
            }
        }

        [Fact]
        public async Task LogOutAsync_ReturnsError_WhenFailed()
        {
            LogoutRequest request = TestHelper.CreateLogoutRequest();
            _authServiceMock.Setup(x => x.LogoutAsync(request)).ReturnsAsync(false);

            IActionResult result = await _controller.LogOutAsync(request);

            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult? objectResult = result as ObjectResult;
                objectResult.Should().NotBeNull();
                objectResult!.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Logout failed.");
            }
        }

        [Fact]
        public async Task CallBackAsync_ReturnsBadRequest_WhenCodeMissing()
        {
            IActionResult result = await _controller.CallBackAsync(null, "state");
            using (new AssertionScope())
            {
                result.Should().BeOfType<BadRequestObjectResult>();
                BadRequestObjectResult? badRequest = result as BadRequestObjectResult;
                badRequest.Should().NotBeNull();
                badRequest!.Value.Should().Be("Authorization code is missing.");
            }
        }

        [Fact]
        public async Task CallBackAsync_ReturnsBadRequest_WhenStateMissing()
        {
            IActionResult result = await _controller.CallBackAsync("code", null);
            using (new AssertionScope())
            {
                result.Should().BeOfType<BadRequestObjectResult>();
                BadRequestObjectResult? badRequest = result as BadRequestObjectResult;
                badRequest.Should().NotBeNull();
                badRequest!.Value.Should().Be("State code is missing.");
            }
        }

        [Fact]
        public async Task CallBackAsync_ReturnsBadRequest_WhenStateInvalid()
        {
            _authServiceMock.Setup(x => x.ValidateAndExtractRequestStateAsync("state"))
                .ReturnsAsync((false, "", "", "", ""));

            IActionResult result = await _controller.CallBackAsync("code", "state");
            using (new AssertionScope())
            {
                result.Should().BeOfType<BadRequestObjectResult>();
                BadRequestObjectResult? badRequest = result as BadRequestObjectResult;
                badRequest.Should().NotBeNull();
                badRequest!.Value.Should().Be("Invalid state parameter.");
            }
        }

        [Fact]
        public async Task CallBackAsync_ReturnsError_WhenHandleCallbackThrows()
        {
            _authServiceMock.Setup(x => x.ValidateAndExtractRequestStateAsync("state"))
                .ReturnsAsync((true, "nonce", "csrf", "user", "https://client"));
            _authServiceMock.Setup(x => x.HandleCallbackAsync("code", "nonce"))
                .ThrowsAsync(new SecurityTokenMalformedException("invalid token"));

            IActionResult result = await _controller.CallBackAsync("code", "state");
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult? objectResult = result as ObjectResult;
                objectResult.Should().NotBeNull();
                objectResult!.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Authentication callback failed.");
            }
        }

        [Fact]
        public async Task CallBackAsync_ReturnsRedirect_WhenSuccess()
        {
            _authServiceMock.Setup(x => x.ValidateAndExtractRequestStateAsync("state"))
                .ReturnsAsync((true, "nonce", "csrf", "user", "https://client"));
            _authServiceMock.Setup(x => x.HandleCallbackAsync("code", "nonce"))
                .ReturnsAsync(new TokenData { AccessToken = "at", IdToken = "it" });

            IActionResult result = await _controller.CallBackAsync("code", "state");
            using (new AssertionScope())
            {
                result.Should().BeOfType<RedirectResult>();
                RedirectResult? redirect = result as RedirectResult;
                redirect.Should().NotBeNull();
                redirect!.Url.Should().Contain("https://client");
                redirect.Url.Should().Contain("csrf_token=csrf");
                redirect.Url.Should().Contain("user_id=user");
                redirect.Url.Should().Contain("access_token=at");
                redirect.Url.Should().Contain("id_token=it");
            }
        }
    }
}
