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
    /// Unit tests for the <see cref="AuthenticationProviderMetadataController"/> class.
    /// </summary>
    public class AuthenticationProviderMetadataControllerTests
    {
        private readonly Mock<ILogger<AuthenticationProviderMetadataController>> _loggerMock = new();
        private readonly Mock<IAuthenticationProviderMetadataRepository> _metadataRepositoryMock = new();

        [Fact]
        public async Task GetById_ReturnsOk_WhenMetadataExists()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid metadataId = Guid.NewGuid();
            OperationResult<AuthenticationProviderMetadata> operationResult = Operation.Ok(new AuthenticationProviderMetadata { Id = metadataId });
            _metadataRepositoryMock.Setup(r => r.GetByIdAsync(metadataId, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderMetadataController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(metadataId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(operationResult.Entity);
            }
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMetadataDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid metadataId = Guid.NewGuid();
            OperationResult<AuthenticationProviderMetadata> operationResult = Operation.Fail<AuthenticationProviderMetadata>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _metadataRepositoryMock.Setup(r => r.GetByIdAsync(metadataId, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderMetadataController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(metadataId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Authentication provider metadata not found with ID {metadataId}");
            }
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WhenMetadataExists()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            List<AuthenticationProviderMetadata> metadataList = new() { new AuthenticationProviderMetadata { Id = Guid.NewGuid() } };
            OperationResult<IEnumerable<AuthenticationProviderMetadata>> operationResult = Operation.Ok<IEnumerable<AuthenticationProviderMetadata>>(metadataList);
            _metadataRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderMetadataController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().BeEquivalentTo(metadataList);
            }
        }

        [Fact]
        public async Task GetAll_ReturnsStatusCode500_WhenRepositoryFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            OperationResult<IEnumerable<AuthenticationProviderMetadata>> operationResult = Operation.Fail<IEnumerable<AuthenticationProviderMetadata>>("Failed to get authentication provider metadata.", OperationErrorCode.SqlError);
            _metadataRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderMetadataController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to get authentication provider metadata.");
            }
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenMetadataIsCreated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            AuthenticationProviderMetadata metadata = new() { Id = Guid.NewGuid() };
            OperationResult<AuthenticationProviderMetadata> operationResult = Operation.Ok(metadata);
            _metadataRepositoryMock.Setup(r => r.AddAsync(metadata, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderMetadataController controller = CreateController("Creator UserName");

            // Act
            IActionResult result = await controller.Create(metadata, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<CreatedAtActionResult>();
                CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
                createdResult.Value.Should().Be(metadata);
                createdResult.RouteValues.Should().NotBeNull();
                createdResult.RouteValues!["authenticationProviderMetadataId"].Should().Be(metadata.Id);
                metadata.CreatedBy.Should().Be("Creator UserName");
                metadata.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                metadata.ModifiedBy.Should().BeNull();
                metadata.Modified.Should().BeNull();
            }
        }

        [Fact]
        public async Task Create_ReturnsStatusCode500_WhenMetadataCreationFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            AuthenticationProviderMetadata metadata = new() { Id = Guid.NewGuid() };
            OperationResult<AuthenticationProviderMetadata> operationResult = Operation.Fail<AuthenticationProviderMetadata>("Failed to create authentication provider metadata.", OperationErrorCode.SqlError);
            _metadataRepositoryMock.Setup(r => r.AddAsync(metadata, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderMetadataController controller = CreateController();

            // Act
            IActionResult result = await controller.Create(metadata, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to create authentication provider metadata.");
            }
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenMetadataIsUpdated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            AuthenticationProviderMetadata metadata = new() { Id = Guid.NewGuid() };
            OperationResult<AuthenticationProviderMetadata> operationResult = Operation.Ok(metadata);
            _metadataRepositoryMock.Setup(r => r.UpdateAsync(metadata, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderMetadataController controller = CreateController("Modifier UserName");

            // Act
            IActionResult result = await controller.Update(metadata, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(metadata);
                metadata.ModifiedBy.Should().Be("Modifier UserName");
                metadata.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            }
        }

        [Fact]
        public async Task Update_ReturnsStatusCode500_WhenMetadataUpdateFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            AuthenticationProviderMetadata metadata = new() { Id = Guid.NewGuid() };
            OperationResult<AuthenticationProviderMetadata> operationResult = Operation.Fail<AuthenticationProviderMetadata>("Failed to update authentication provider metadata.", OperationErrorCode.SqlError);
            _metadataRepositoryMock.Setup(r => r.UpdateAsync(metadata, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderMetadataController controller = CreateController();

            // Act
            IActionResult result = await controller.Update(metadata, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to update authentication provider metadata.");
            }
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenMetadataIsDeleted()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid metadataId = Guid.NewGuid();
            OperationResult<AuthenticationProviderMetadata> operationResult = Operation.Ok(new AuthenticationProviderMetadata { Id = metadataId });
            _metadataRepositoryMock.Setup(r => r.DeleteAsync(metadataId, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderMetadataController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(metadataId, cancellationToken);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenMetadataDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid metadataId = Guid.NewGuid();
            OperationResult<AuthenticationProviderMetadata> operationResult = Operation.Fail<AuthenticationProviderMetadata>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _metadataRepositoryMock.Setup(r => r.DeleteAsync(metadataId, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderMetadataController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(metadataId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Authentication provider metadata not found with ID {metadataId}");
            }
        }

        [Fact]
        public async Task Delete_ReturnsStatusCode500_WhenMetadataDeleteFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid metadataId = Guid.NewGuid();
            OperationResult<AuthenticationProviderMetadata> operationResult = Operation.Fail<AuthenticationProviderMetadata>("Failed to delete authentication provider metadata.", OperationErrorCode.SqlError);
            _metadataRepositoryMock.Setup(r => r.DeleteAsync(metadataId, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationProviderMetadataController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(metadataId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to delete authentication provider metadata.");
            }
        }

        private AuthenticationProviderMetadataController CreateController(string? userName = null)
        {
            AuthenticationProviderMetadataController controller = new(_loggerMock.Object, _metadataRepositoryMock.Object)
            {
                ControllerContext = TestHelper.CreateControllerContext(userName)
            };

            return controller;
        }
    }
}