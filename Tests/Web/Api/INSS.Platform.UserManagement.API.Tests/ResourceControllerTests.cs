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
    /// Unit tests for the <see cref="ResourceController"/> class.
    /// </summary>
    public class ResourceControllerTests
    {
        private readonly Mock<ILogger<ResourceController>> _loggerMock = new();
        private readonly Mock<IResourceRepository> _resourceRepositoryMock = new();

        [Fact]
        public async Task GetById_ReturnsOk_WhenResourceExists()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid resourceId = Guid.NewGuid();
            OperationResult<Resource> operationResult = Operation.Ok(new Resource { Id = resourceId });
            _resourceRepositoryMock.Setup(r => r.GetByIdAsync(resourceId, cancellationToken)).ReturnsAsync(operationResult);

            ResourceController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(resourceId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(operationResult.Entity);
            }
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenResourceDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid resourceId = Guid.NewGuid();
            OperationResult<Resource> operationResult = Operation.Fail<Resource>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _resourceRepositoryMock.Setup(r => r.GetByIdAsync(resourceId, cancellationToken)).ReturnsAsync(operationResult);

            ResourceController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(resourceId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Resource not found with ID {resourceId}");
            }
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WhenResourcesExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            List<Resource> resources = new() { new Resource { Id = Guid.NewGuid() } };
            OperationResult<IEnumerable<Resource>> operationResult = Operation.Ok<IEnumerable<Resource>>(resources);
            _resourceRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            ResourceController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().BeEquivalentTo(resources);
            }
        }

        [Fact]
        public async Task GetAll_ReturnsStatusCode500_WhenRepositoryFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            OperationResult<IEnumerable<Resource>> operationResult = Operation.Fail<IEnumerable<Resource>>("Failed to get resources.", OperationErrorCode.SqlError);
            _resourceRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            ResourceController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to get resources.");
            }
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenResourceIsCreated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Resource resource = new() { Id = Guid.NewGuid() };
            OperationResult<Resource> operationResult = Operation.Ok(resource);
            _resourceRepositoryMock.Setup(r => r.AddAsync(resource, cancellationToken)).ReturnsAsync(operationResult);

            ResourceController controller = CreateController("Creator UserName");

            // Act
            IActionResult result = await controller.Create(resource, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<CreatedAtActionResult>();
                CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
                createdResult.Value.Should().Be(resource);
                createdResult.RouteValues.Should().NotBeNull();
                createdResult.RouteValues["resourceId"].Should().Be(resource.Id);
                resource.CreatedBy.Should().Be("Creator UserName");
                resource.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                resource.ModifiedBy.Should().BeNull();
                resource.Modified.Should().BeNull();
            }
        }

        [Fact]
        public async Task Create_ReturnsStatusCode500_WhenResourceCreationFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Resource resource = new() { Id = Guid.NewGuid() };
            OperationResult<Resource> operationResult = Operation.Fail<Resource>("Failed to create resource.", OperationErrorCode.SqlError);
            _resourceRepositoryMock.Setup(r => r.AddAsync(resource, cancellationToken)).ReturnsAsync(operationResult);

            ResourceController controller = CreateController();

            // Act
            IActionResult result = await controller.Create(resource, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to create resource.");
            }
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenResourceIsUpdated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Resource resource = new() { Id = Guid.NewGuid() };
            OperationResult<Resource> operationResult = Operation.Ok(resource);
            _resourceRepositoryMock.Setup(r => r.UpdateAsync(resource, cancellationToken)).ReturnsAsync(operationResult);

            ResourceController controller = CreateController("Modifier UserName");

            // Act
            IActionResult result = await controller.Update(resource, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(resource);
                resource.ModifiedBy.Should().Be("Modifier UserName");
                resource.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            }
        }

        [Fact]
        public async Task Update_ReturnsStatusCode500_WhenResourceUpdateFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Resource resource = new() { Id = Guid.NewGuid() };
            OperationResult<Resource> operationResult = Operation.Fail<Resource>("Failed to update resource.", OperationErrorCode.SqlError);
            _resourceRepositoryMock.Setup(r => r.UpdateAsync(resource, cancellationToken)).ReturnsAsync(operationResult);

            ResourceController controller = CreateController();

            // Act
            IActionResult result = await controller.Update(resource, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to update resource.");
            }
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenResourceIsDeleted()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid resourceId = Guid.NewGuid();
            OperationResult<Resource> operationResult = Operation.Ok(new Resource { Id = resourceId });
            _resourceRepositoryMock.Setup(r => r.DeleteAsync(resourceId, cancellationToken)).ReturnsAsync(operationResult);

            ResourceController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(resourceId, cancellationToken);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenResourceDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid resourceId = Guid.NewGuid();
            OperationResult<Resource> operationResult = Operation.Fail<Resource>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _resourceRepositoryMock.Setup(r => r.DeleteAsync(resourceId, cancellationToken)).ReturnsAsync(operationResult);

            ResourceController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(resourceId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Resource not found with ID {resourceId}");
            }
        }

        [Fact]
        public async Task Delete_ReturnsStatusCode500_WhenResourceDeleteFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid resourceId = Guid.NewGuid();
            OperationResult<Resource> operationResult = Operation.Fail<Resource>("Failed to delete resource.", OperationErrorCode.SqlError);
            _resourceRepositoryMock.Setup(r => r.DeleteAsync(resourceId, cancellationToken)).ReturnsAsync(operationResult);

            ResourceController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(resourceId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to delete resource.");
            }
        }

        private ResourceController CreateController(string? userName = null)
        {
            ResourceController controller = new(_loggerMock.Object, _resourceRepositoryMock.Object)
            {
                ControllerContext = TestHelper.CreateControllerContext(userName)
            };

            return controller;
        }
    }
}