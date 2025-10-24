//using FluentAssertions;
//using FluentAssertions.Execution;
//using INSS.Platform.Common.Auth.API.Services;
//using INSS.Platform.Common.Auth.Contracts.Request;
//using INSS.Platform.Common.Auth.Contracts.Response;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Moq;
//using Moq.Protected;
//using System.Net;

//namespace INSS.Platform.Common.Auth.API.Tests
//{
//    public class OneLoginAuthServiceTests
//    {
//        private readonly Mock<ILogger<OneLoginAuthService>> _loggerMock;
//        private readonly Mock<IConfiguration> _configMock;
//        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
//        private readonly OneLoginAuthService _service;

//        public OneLoginAuthServiceTests()
//        {
//            _loggerMock = new Mock<ILogger<OneLoginAuthService>>();
//            _configMock = new Mock<IConfiguration>();
//            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
//            _service = new OneLoginAuthService(_loggerMock.Object, _configMock.Object, _httpClientFactoryMock.Object);
//        }

//        [Fact]
//        public async Task GetLoginRedirectUrlAsync_ReturnsUrl_WhenConfigValid()
//        {
//            // Arrange
//            LoginRequest loginRequest = TestHelper.CreateLoginRequest();
//            _configMock.Setup(x => x["OneLogin:ClientId"]).Returns("clientId");
//            _configMock.Setup(x => x["OneLogin:AuthorizeUri"]).Returns("https://auth");
//            _configMock.Setup(x => x["OneLogin:Scopes"]).Returns("openid profile");
//            _configMock.Setup(x => x["OneLogin:RedirectUri"]).Returns("https://redirect");
//            _configMock.Setup(x => x["OneLogin:QueryJwtPrivateKeyFile"]).Returns(string.Empty);
//            _configMock.Setup(x => x["OneLogin:StateJwtPrivateKeyFile"]).Returns(string.Empty);

//            string privateKey = @"-----BEGIN RSA PRIVATE KEY-----
//            MIIBOgIBAAJBALe...
//            -----END RSA PRIVATE KEY-----";

//            string stateKey = privateKey;
//            TestableOneLoginAuthService service = new (_loggerMock.Object, _configMock.Object, _httpClientFactoryMock.Object)
//            {
//                QueryJwtPrivateKey = privateKey,
//                StateJwtPrivateKey = stateKey
//            };

//            // Act
//            string url = await service.GetLoginRedirectUrlAsync(loginRequest);

//            // Assert
//            using (new AssertionScope())
//            {
//                url.Should().Contain("https://auth");
//                url.Should().Contain("client_id=clientId");
//                url.Should().Contain("scope=openid%20profile");
//                url.Should().Contain("request=");
//            }
//        }

//        [Fact]
//        public async Task LogoutAsync_ReturnsTrue_WhenLogoutSuccess()
//        {
//            // Arrange
//            LogoutRequest logoutRequest = TestHelper.CreateLogoutRequest();
//            _configMock.Setup(x => x["OneLogin:LogoutUri"]).Returns("https://logout");

//            Mock<HttpMessageHandler> handlerMock = new ();
//            handlerMock.Protected()
//                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
//                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.Found });

//            HttpClient httpClient = new (handlerMock.Object);
//            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

//            // Act
//            bool result = await _service.LogoutAsync(logoutRequest);

//            // Assert
//            result.Should().BeTrue();
//        }

//        [Fact]
//        public async Task LogoutAsync_ReturnsFalse_WhenLogoutFails()
//        {
//            // Arrange
//            LogoutRequest logoutRequest = TestHelper.CreateLogoutRequest();

//            _configMock.Setup(x => x["OneLogin:LogoutUri"]).Returns("https://logout");

//            Mock<HttpMessageHandler> handlerMock = new Mock<HttpMessageHandler>();
//            handlerMock.Protected()
//                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
//                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent("fail") });

//            HttpClient httpClient = new (handlerMock.Object);
//            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

//            // Act
//            bool result = await _service.LogoutAsync(logoutRequest);

//            // Assert
//            result.Should().BeFalse();
//        }

//        [Fact]
//        public async Task HandleCallbackAsync_ReturnsTokenData_WhenSuccess()
//        {
//            // Arrange
//            _configMock.Setup(x => x["OneLogin:ClientId"]).Returns("clientId");
//            _configMock.Setup(x => x["OneLogin:TokenUri"]).Returns("https://token");
//            _configMock.Setup(x => x["OneLogin:RedirectUri"]).Returns("https://redirect");
//            _configMock.Setup(x => x["OneLogin:QueryJwtPrivateKeyFile"]).Returns(string.Empty);

//            Mock<HttpMessageHandler> handlerMock = new ();
//            string tokenJson = "{\"access_token\":\"at\",\"id_token\":\"it\"}";
            
//            handlerMock.Protected()
//                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
//                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(tokenJson) });

//            HttpClient httpClient = new (handlerMock.Object);
//            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

//            OneLoginAuthService service = new (_loggerMock.Object, _configMock.Object, _httpClientFactoryMock.Object)
//            {
//                QueryJwtPrivateKey = @"-----BEGIN RSA PRIVATE KEY-----
//                MIIBOgIBAAJBALe...
//                -----END RSA PRIVATE KEY-----"
//            };

//            // Act
//            TokenData result = await service.HandleCallbackAsync("code", "nonce");

//            // Assert
//            result.AccessToken.Should().Be("at");
//            result.IdToken.Should().Be("it");
//        }

//        [Fact]
//        public async Task HandleCallbackAsync_ReturnsEmptyTokenData_WhenTokenEndpointFails()
//        {
//            // Arrange
//            _configMock.Setup(x => x["OneLogin:ClientId"]).Returns("clientId");
//            _configMock.Setup(x => x["OneLogin:TokenUri"]).Returns("https://token");
//            _configMock.Setup(x => x["OneLogin:RedirectUri"]).Returns("https://redirect");
//            _configMock.Setup(x => x["OneLogin:QueryJwtPrivateKeyFile"]).Returns(string.Empty);

//            var handlerMock = new Mock<HttpMessageHandler>();
//            handlerMock.Protected()
//                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
//                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent("fail") });
//            var httpClient = new HttpClient(handlerMock.Object);
//            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

//            var service = new TestableOneLoginAuthService(_loggerMock.Object, _configMock.Object, _httpClientFactoryMock.Object)
//            {
//                QueryJwtPrivateKey = @"-----BEGIN RSA PRIVATE KEY-----
//MIIBOgIBAAJBALe...
//-----END RSA PRIVATE KEY-----"
//            };

//            // Act
//            var result = await service.HandleCallbackAsync("code", "nonce");

//            // Assert
//            result.AccessToken.Should().BeNullOrEmpty();
//            result.IdToken.Should().BeNullOrEmpty();
//        }

//        [Fact]
//        public async Task ValidateAndExtractRequestStateAsync_ReturnsValidTuple_WhenTokenValid()
//        {
//            // Arrange
//            _configMock.Setup(x => x["OneLogin:ClientId"]).Returns("clientId");
//            _configMock.Setup(x => x["OneLogin:AuthorizeUri"]).Returns("https://auth");
//            _configMock.Setup(x => x["OneLogin:StateJwtPublicKeyFile"]).Returns(string.Empty);

//            var service = new TestableOneLoginAuthService(_loggerMock.Object, _configMock.Object, _httpClientFactoryMock.Object)
//            {
//                StateJwtPublicKey = @"-----BEGIN PUBLIC KEY-----
//MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBALe...
//-----END PUBLIC KEY-----"
//            };

//            // Use a valid JWT for the test, or mock GetClaimsFromJwt to return expected claims
//            var token = "valid.jwt.token";
//            service.MockClaims = new[]
//            {
//                new System.Security.Claims.Claim("nonce", "nonce"),
//                new System.Security.Claims.Claim("csrfToken", "csrf"),
//                new System.Security.Claims.Claim("userId", "user"),
//                new System.Security.Claims.Claim("clientUrl", "https://client")
//            };

//            // Act
//            var (isValid, nonce, csrfToken, userId, clientUrl) = await service.ValidateAndExtractRequestStateAsync(token);

//            // Assert
//            isValid.Should().BeTrue();
//            nonce.Should().Be("nonce");
//            csrfToken.Should().Be("csrf");
//            userId.Should().Be("user");
//            clientUrl.Should().Be("https://client");
//        }

//        [Fact]
//        public async Task ValidateAndExtractRequestStateAsync_ReturnsInvalidTuple_WhenTokenInvalid()
//        {
//            // Arrange
//            _configMock.Setup(x => x["OneLogin:ClientId"]).Returns("clientId");
//            _configMock.Setup(x => x["OneLogin:AuthorizeUri"]).Returns("https://auth");
//            _configMock.Setup(x => x["OneLogin:StateJwtPublicKeyFile"]).Returns(string.Empty);

//            var service = new TestableOneLoginAuthService(_loggerMock.Object, _configMock.Object, _httpClientFactoryMock.Object)
//            {
//                StateJwtPublicKey = @"-----BEGIN PUBLIC KEY-----
//MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBALe...
//-----END PUBLIC KEY-----"
//            };

//            // Simulate exception in ValidateToken
//            service.ThrowOnValidateToken = true;

//            // Act
//            var (isValid, nonce, csrfToken, userId, clientUrl) = await service.ValidateAndExtractRequestStateAsync("invalid.jwt.token");

//            // Assert
//            isValid.Should().BeFalse();
//            nonce.Should().BeEmpty();
//            csrfToken.Should().BeEmpty();
//            userId.Should().BeEmpty();
//            clientUrl.Should().BeEmpty();
//        }

//    }
//}
