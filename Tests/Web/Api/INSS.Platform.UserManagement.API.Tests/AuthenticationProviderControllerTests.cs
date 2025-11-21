using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Abstractions.Results;
using INSS.Platform.UserManagement.API.Controllers;
using INSS.Platform.UserManagement.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace INSS.Platform.UserManagement.API.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="AuthenticationProviderController"/> class.
    /// </summary>
    public class AuthenticationProviderControllerTests
    {
        private readonly Mock<ILogger<AuthenticationProviderController>> _loggerMock = new();
        private readonly Mock<IAuthenticationProviderRepository> _providerRepositoryMock = new();

        [Fact]
        public async Task GetById_ReturnsOk_WhenProviderExists()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid providerId = Guid.NewGuid();
            OperationResult<AuthenticationProvider> operationResult = Operation.Ok(new AuthenticationProvider { Id = providerId });
            _providerRepositoryMock.Setup(r => r.GetByIdAsync(providerId, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(providerId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(operationResult.Entity);
            }
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenProviderDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid providerId = Guid.NewGuid();
            OperationResult<AuthenticationProvider> operationResult = Operation.Fail<AuthenticationProvider>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _providerRepositoryMock.Setup(r => r.GetByIdAsync(providerId, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(providerId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Authentication provider not found with ID {providerId}");
            }
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WhenProvidersExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            List<AuthenticationProvider> providers = new() { new AuthenticationProvider { Id = Guid.NewGuid() } };
            OperationResult<IEnumerable<AuthenticationProvider>> operationResult = Operation.Ok<IEnumerable<AuthenticationProvider>>(providers);
            _providerRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().BeEquivalentTo(providers);
            }
        }

        [Fact]
        public async Task GetAll_ReturnsStatusCode500_WhenRepositoryFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            OperationResult<IEnumerable<AuthenticationProvider>> operationResult = Operation.Fail<IEnumerable<AuthenticationProvider>>("Failed to get authentication providers.", OperationErrorCode.SqlError);
            _providerRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to get authentication providers.");
            }
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenProviderIsCreated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            AuthenticationProvider provider = new() { Id = Guid.NewGuid() };
            OperationResult<AuthenticationProvider> operationResult = Operation.Ok(provider);
            _providerRepositoryMock.Setup(r => r.AddAsync(provider, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderController controller = CreateController("Creator UserName");

            // Act
            IActionResult result = await controller.Create(provider, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<CreatedAtActionResult>();
                CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
                createdResult.Value.Should().Be(provider);
                createdResult.RouteValues.Should().NotBeNull();
                createdResult.RouteValues!["authenticationProviderId"].Should().Be(provider.Id);
                provider.CreatedBy.Should().Be("Creator UserName");
                provider.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                provider.ModifiedBy.Should().BeNull();
                provider.Modified.Should().BeNull();
            }
        }

        [Fact]
        public async Task Create_ReturnsStatusCode500_WhenProviderCreationFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            AuthenticationProvider provider = new() { Id = Guid.NewGuid() };
            OperationResult<AuthenticationProvider> operationResult = Operation.Fail<AuthenticationProvider>("Failed to create authentication provider.", OperationErrorCode.SqlError);
            _providerRepositoryMock.Setup(r => r.AddAsync(provider, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderController controller = CreateController();

            // Act
            IActionResult result = await controller.Create(provider, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to create authentication provider.");
            }
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenProviderIsUpdated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            AuthenticationProvider provider = new() { Id = Guid.NewGuid() };
            OperationResult<AuthenticationProvider> operationResult = Operation.Ok(provider);
            _providerRepositoryMock.Setup(r => r.UpdateAsync(provider, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderController controller = CreateController("Modifier UserName");

            // Act
            IActionResult result = await controller.Update(provider, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(provider);
                provider.ModifiedBy.Should().Be("Modifier UserName");
                provider.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            }
        }

        [Fact]
        public async Task Update_ReturnsStatusCode500_WhenProviderUpdateFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            AuthenticationProvider provider = new() { Id = Guid.NewGuid() };
            OperationResult<AuthenticationProvider> operationResult = Operation.Fail<AuthenticationProvider>("Failed to update authentication provider.", OperationErrorCode.SqlError);
            _providerRepositoryMock.Setup(r => r.UpdateAsync(provider, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderController controller = CreateController();

            // Act
            IActionResult result = await controller.Update(provider, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to update authentication provider.");
            }
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenProviderIsDeleted()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid providerId = Guid.NewGuid();
            OperationResult<AuthenticationProvider> operationResult = Operation.Ok(new AuthenticationProvider { Id = providerId });
            _providerRepositoryMock.Setup(r => r.DeleteAsync(providerId, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(providerId, cancellationToken);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenProviderDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid providerId = Guid.NewGuid();
            OperationResult<AuthenticationProvider> operationResult = Operation.Fail<AuthenticationProvider>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _providerRepositoryMock.Setup(r => r.DeleteAsync(providerId, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(providerId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Authentication provider not found with ID {providerId}");
            }
        }

        [Fact]
        public async Task Delete_ReturnsStatusCode500_WhenProviderDeleteFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid providerId = Guid.NewGuid();
            OperationResult<AuthenticationProvider> operationResult = Operation.Fail<AuthenticationProvider>("Failed to delete authentication provider.", OperationErrorCode.SqlError);
            _providerRepositoryMock.Setup(r => r.DeleteAsync(providerId, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(providerId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to delete authentication provider.");
            }
        }

        private AuthenticationProviderController CreateController(string? userName = null)
        {
            AuthenticationProviderController controller = new(_loggerMock.Object, _providerRepositoryMock.Object)
            {
                ControllerContext = TestHelper.CreateControllerContext(userName)
            };

            return controller;
        }
    }
}