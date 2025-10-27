using Azure.Security.KeyVault.Secrets;
using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.Common.Auth.API.Services;
using INSS.Platform.Common.Auth.Contracts.Request;
using INSS.Platform.Common.Auth.Contracts.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Encodings.Web;

namespace INSS.Platform.Common.Auth.API.Tests
{
    public class OneLoginAuthServiceTests
    {
        private readonly Mock<ILogger<OneLoginAuthService>> _loggerMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<SecretClient> _secretClientMock;
        private const string PrivateKeyPemFile = "Keys\\test_private_key.pem";
        private const string PublicKeyPemFile = "Keys\\test_public_key.pem";
        private const string ClientId = "test-issuer";
        private const string AuthUrl = "https://auth";
        private const string Scope = "openid profile";

        public OneLoginAuthServiceTests()
        {
            _loggerMock = new Mock<ILogger<OneLoginAuthService>>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _secretClientMock = new Mock<SecretClient>();

            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(x => x["OneLogin:ClientId"]).Returns(ClientId);
            _configMock.Setup(x => x["OneLogin:AuthorizeUri"]).Returns(AuthUrl);
            _configMock.Setup(x => x["OneLogin:TokenUri"]).Returns("https://token");
            _configMock.Setup(x => x["OneLogin:Scopes"]).Returns(Scope);
            _configMock.Setup(x => x["OneLogin:RedirectUri"]).Returns("https://redirect");
            _configMock.Setup(x => x["OneLogin:QueryJwtPrivateKeyFile"]).Returns(PrivateKeyPemFile);
            _configMock.Setup(x => x["OneLogin:StateJwtPrivateKeyFile"]).Returns(PrivateKeyPemFile);
            _configMock.Setup(x => x["OneLogin:StateJwtPublicKeyFile"]).Returns(PublicKeyPemFile);
            _configMock.Setup(x => x["OneLogin:LogoutUri"]).Returns("https://logout");
        }

        [Fact]
        public async Task GetLoginRedirectUrlAsync_ReturnsUrl_WhenConfigValid()
        {
            // Arrange
            LoginRequest loginRequest = TestHelper.CreateLoginRequest();

            OneLoginAuthService service = new(_loggerMock.Object, _configMock.Object, _httpClientFactoryMock.Object, _secretClientMock.Object);

            // Act
            string url = await service.GetLoginRedirectUrlAsync(loginRequest);

            // Assert
            using (new AssertionScope())
            {
                url.Should().Contain(AuthUrl);
                url.Should().Contain($"client_id={ClientId}");
                url.Should().Contain($"scope={UrlEncoder.Default.Encode(Scope)}");
                url.Should().Contain("request=");
            }
        }

        [Fact]
        public async Task LogoutAsync_ReturnsTrue_WhenLogoutSuccess()
        {
            // Arrange
            LogoutRequest logoutRequest = TestHelper.CreateLogoutRequest();

            Mock<HttpMessageHandler> handlerMock = new();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.Found });

            HttpClient httpClient = new(handlerMock.Object);
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            OneLoginAuthService service = new(_loggerMock.Object, _configMock.Object, _httpClientFactoryMock.Object, _secretClientMock.Object);

            // Act
            bool result = await service.LogoutAsync(logoutRequest);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task LogoutAsync_ReturnsFalse_WhenLogoutFails()
        {
            // Arrange
            LogoutRequest logoutRequest = TestHelper.CreateLogoutRequest();

            const string failResponseContent = "request failed";
            Mock<HttpMessageHandler> handlerMock = new ();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent(failResponseContent) });

            HttpClient httpClient = new(handlerMock.Object);
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            OneLoginAuthService service = new(_loggerMock.Object, _configMock.Object, _httpClientFactoryMock.Object, _secretClientMock.Object);

            // Act
            bool result = await service.LogoutAsync(logoutRequest);

            // Assert
            result.Should().BeFalse();

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"Logout endpoint returned error: {HttpStatusCode.BadRequest} - {failResponseContent}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleCallbackAsync_ReturnsTokenData_WhenSuccess()
        {
            // Arrange
            const string nonce = "some-nonce";
            string idToken = TestHelper.CreateIdToken(PrivateKeyPemFile, nonce);
            string accessToken = TestHelper.CreateAccessToken(PrivateKeyPemFile);

            Mock<HttpMessageHandler> handlerMock = new();
            var tokenObj = new { access_token = accessToken, id_token = idToken};
            string tokenJson = System.Text.Json.JsonSerializer.Serialize(tokenObj);

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(tokenJson) });

            HttpClient httpClient = new(handlerMock.Object);
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            OneLoginAuthService service = new(_loggerMock.Object, _configMock.Object, _httpClientFactoryMock.Object, _secretClientMock.Object);

            // Act
            TokenData result = await service.HandleCallbackAsync("code", nonce);

            // Assert
            using (new AssertionScope())
            {
                result.AccessToken.Should().Be(accessToken);
                result.IdToken.Should().Be(idToken);
            }
        }

        [Fact]
        public async Task HandleCallbackAsync_ReturnsEmptyTokenData_WhenTokenEndpointFails()
        {
            // Arrange
            Mock<HttpMessageHandler> handlerMock = new();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent("fail") });

            HttpClient httpClient = new(handlerMock.Object);
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            OneLoginAuthService service = new(_loggerMock.Object, _configMock.Object, _httpClientFactoryMock.Object, _secretClientMock.Object);

            // Act
            TokenData result = await service.HandleCallbackAsync("code", "nonce");

            // Assert
            using (new AssertionScope())
            {
                result.AccessToken.Should().BeNullOrEmpty();
                result.IdToken.Should().BeNullOrEmpty();
            }

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains("Error handling authentication callback.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ValidateAndExtractRequestStateAsync_ReturnsValidTuple_WhenTokenValid()
        {
            // Arrange
            const string nonceIn = "some-nonce";
            const string csrfTokenIn = "some-csrf";
            const string userIdIn = "user-id";
            const string clientUrlIn = "http://some-url";
            string token = TestHelper.CreateIdToken(PrivateKeyPemFile, nonceIn, csrfTokenIn, userIdIn, clientUrlIn);

            OneLoginAuthService service = new(_loggerMock.Object, _configMock.Object, _httpClientFactoryMock.Object, _secretClientMock.Object);

            // Act
            (bool isValid, string? nonceOut, string? csrfTokenOut, string? userIdOut, string? clientUrlOut) = await service.ValidateAndExtractRequestStateAsync(token);

            // Assert
            using (new AssertionScope())
            {
                isValid.Should().BeTrue();
                nonceIn.Should().Be(nonceOut);
                csrfTokenIn.Should().Be(csrfTokenOut);
                userIdIn.Should().Be(userIdOut);
                clientUrlIn.Should().Be(clientUrlOut);
            }
        }

        [Fact]
        public async Task ValidateAndExtractRequestStateAsync_ReturnsInvalidTuple_WhenTokenInvalid()
        {
            // Arrange
            const string nonceIn = "some-nonce";
            const string csrfTokenIn = "some-csrf";
            const string userIdIn = "user-id";
            const string clientUrlIn = "http://some-url";
            const string invalidIssuer = "invalid-issuer";
            string token = TestHelper.CreateIdToken(PrivateKeyPemFile, nonceIn, csrfTokenIn, userIdIn, clientUrlIn, invalidIssuer);

            OneLoginAuthService service = new(_loggerMock.Object, _configMock.Object, _httpClientFactoryMock.Object, _secretClientMock.Object);

            // Act
            (bool isValid, string? nonceOut, string? csrfTokenOut, string? userIdOut, string? clientUrlOut) = await service.ValidateAndExtractRequestStateAsync(token);

            // Assert
            using (new AssertionScope())
            {
                isValid.Should().BeFalse();
                nonceOut.Should().BeEmpty();
                csrfTokenOut.Should().BeEmpty();
                userIdOut.Should().BeEmpty();
                clientUrlOut.Should().BeEmpty();
            }

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains("Request state validation failed on callback.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
