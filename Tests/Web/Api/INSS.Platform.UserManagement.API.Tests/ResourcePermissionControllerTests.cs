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
    /// Unit tests for the <see cref="ResourcePermissionController"/> class.
    /// </summary>
    public class ResourcePermissionControllerTests
    {
        private readonly Mock<ILogger<ResourcePermissionController>> _loggerMock = new();
        private readonly Mock<IResourcePermissionRepository> _repositoryMock = new();

        [Fact]
        public async Task GetById_ReturnsOk_WhenResourcePermissionExists()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid id = Guid.NewGuid();
            OperationResult<ResourcePermission> operationResult = Operation.Ok(new ResourcePermission { Id = id });
            _repositoryMock.Setup(r => r.GetByIdAsync(id, cancellationToken)).ReturnsAsync(operationResult);

            ResourcePermissionController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(id, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(operationResult.Entity);
            }
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenResourcePermissionDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid id = Guid.NewGuid();
            OperationResult<ResourcePermission> operationResult = Operation.Fail<ResourcePermission>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _repositoryMock.Setup(r => r.GetByIdAsync(id, cancellationToken)).ReturnsAsync(operationResult);

            ResourcePermissionController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(id, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Resource permission not found with ID {id}");
            }
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WhenResourcePermissionsExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            List<ResourcePermission> entities = new() { new ResourcePermission { Id = Guid.NewGuid() } };
            OperationResult<IEnumerable<ResourcePermission>> operationResult = Operation.Ok<IEnumerable<ResourcePermission>>(entities);
            _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            ResourcePermissionController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().BeEquivalentTo(entities);
            }
        }

        [Fact]
        public async Task GetAll_ReturnsStatusCode500_WhenRepositoryFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            OperationResult<IEnumerable<ResourcePermission>> operationResult = Operation.Fail<IEnumerable<ResourcePermission>>("Failed to get resource permissions.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            ResourcePermissionController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to get resource permissions.");
            }
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenResourcePermissionIsCreated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            ResourcePermission entity = new() { Id = Guid.NewGuid() };
            OperationResult<ResourcePermission> operationResult = Operation.Ok(entity);
            _repositoryMock.Setup(r => r.AddAsync(entity, cancellationToken)).ReturnsAsync(operationResult);

            ResourcePermissionController controller = CreateController("Creator UserName");

            // Act
            IActionResult result = await controller.Create(entity, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<CreatedAtActionResult>();
                CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
                createdResult.Value.Should().Be(entity);
                createdResult.RouteValues.Should().NotBeNull();
                createdResult.RouteValues["resourcePermissionId"].Should().Be(entity.Id);
                entity.CreatedBy.Should().Be("Creator UserName");
                entity.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                entity.ModifiedBy.Should().BeNull();
                entity.Modified.Should().BeNull();
            }
        }

        [Fact]
        public async Task Create_ReturnsStatusCode500_WhenCreationFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            ResourcePermission entity = new() { Id = Guid.NewGuid() };
            OperationResult<ResourcePermission> operationResult = Operation.Fail<ResourcePermission>("Failed to create resource permission.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.AddAsync(entity, cancellationToken)).ReturnsAsync(operationResult);

            ResourcePermissionController controller = CreateController();

            // Act
            IActionResult result = await controller.Create(entity, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to create resource permission.");
            }
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenResourcePermissionIsUpdated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            ResourcePermission entity = new() { Id = Guid.NewGuid() };
            OperationResult<ResourcePermission> operationResult = Operation.Ok(entity);
            _repositoryMock.Setup(r => r.UpdateAsync(entity, cancellationToken)).ReturnsAsync(operationResult);

            ResourcePermissionController controller = CreateController("Modifier UserName");

            // Act
            IActionResult result = await controller.Update(entity, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(entity);
                entity.ModifiedBy.Should().Be("Modifier UserName");
                entity.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            }
        }

        [Fact]
        public async Task Update_ReturnsStatusCode500_WhenUpdateFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            ResourcePermission entity = new() { Id = Guid.NewGuid() };
            OperationResult<ResourcePermission> operationResult = Operation.Fail<ResourcePermission>("Failed to update resource permission.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.UpdateAsync(entity, cancellationToken)).ReturnsAsync(operationResult);

            ResourcePermissionController controller = CreateController();

            // Act
            IActionResult result = await controller.Update(entity, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to update resource permission.");
            }
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenResourcePermissionIsDeleted()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid id = Guid.NewGuid();
            OperationResult<ResourcePermission> operationResult = Operation.Ok(new ResourcePermission { Id = id });
            _repositoryMock.Setup(r => r.DeleteAsync(id, cancellationToken)).ReturnsAsync(operationResult);

            ResourcePermissionController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(id, cancellationToken);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenResourcePermissionDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid id = Guid.NewGuid();
            OperationResult<ResourcePermission> operationResult = Operation.Fail<ResourcePermission>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _repositoryMock.Setup(r => r.DeleteAsync(id, cancellationToken)).ReturnsAsync(operationResult);

            ResourcePermissionController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(id, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Resource permission not found with ID {id}");
            }
        }

        [Fact]
        public async Task Delete_ReturnsStatusCode500_WhenDeleteFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid id = Guid.NewGuid();
            OperationResult<ResourcePermission> operationResult = Operation.Fail<ResourcePermission>("Failed to delete resource permission.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.DeleteAsync(id, cancellationToken)).ReturnsAsync(operationResult);

            ResourcePermissionController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(id, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to delete resource permission.");
            }
        }

        private ResourcePermissionController CreateController(string? userName = null)
        {
            ResourcePermissionController controller = new(_loggerMock.Object, _repositoryMock.Object)
            {
                ControllerContext = TestHelper.CreateControllerContext(userName)
            };

            return controller;
        }
    }
}