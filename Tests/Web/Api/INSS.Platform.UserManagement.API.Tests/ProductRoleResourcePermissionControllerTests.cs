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
    /// Unit tests for the <see cref="ProductRoleResourcePermissionController"/> class.
    /// </summary>
    public class ProductRoleResourcePermissionControllerTests
    {
        private readonly Mock<ILogger<ProductRoleResourcePermissionController>> _loggerMock = new();
        private readonly Mock<IProductRoleResourcePermissionRepository> _repositoryMock = new();

        [Fact]
        public async Task GetById_ReturnsOk_WhenEntityExists()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid id = Guid.NewGuid();
            OperationResult<ProductRoleResourcePermission> operationResult = Operation.Ok(new ProductRoleResourcePermission { Id = id });
            _repositoryMock.Setup(r => r.GetByIdAsync(id, cancellationToken)).ReturnsAsync(operationResult);

            ProductRoleResourcePermissionController controller = CreateController();

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
        public async Task GetById_ReturnsNotFound_WhenEntityDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid id = Guid.NewGuid();
            OperationResult<ProductRoleResourcePermission> operationResult = Operation.Fail<ProductRoleResourcePermission>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _repositoryMock.Setup(r => r.GetByIdAsync(id, cancellationToken)).ReturnsAsync(operationResult);

            ProductRoleResourcePermissionController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(id, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Product role resource permission not found with ID {id}");
            }
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WhenEntitiesExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            List<ProductRoleResourcePermission> entities = new() { new ProductRoleResourcePermission { Id = Guid.NewGuid() } };
            OperationResult<IEnumerable<ProductRoleResourcePermission>> operationResult = Operation.Ok<IEnumerable<ProductRoleResourcePermission>>(entities);
            _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            ProductRoleResourcePermissionController controller = CreateController();

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
            OperationResult<IEnumerable<ProductRoleResourcePermission>> operationResult = Operation.Fail<IEnumerable<ProductRoleResourcePermission>>("Failed to get product role resource permissions.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            ProductRoleResourcePermissionController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to get product role resource permissions.");
            }
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenEntityIsCreated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            ProductRoleResourcePermission entity = new() { Id = Guid.NewGuid() };
            OperationResult<ProductRoleResourcePermission> operationResult = Operation.Ok(entity);
            _repositoryMock.Setup(r => r.AddAsync(entity, cancellationToken)).ReturnsAsync(operationResult);

            ProductRoleResourcePermissionController controller = CreateController("Creator UserName");

            // Act
            IActionResult result = await controller.Create(entity, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<CreatedAtActionResult>();
                CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
                createdResult.Value.Should().Be(entity);
                createdResult.RouteValues.Should().NotBeNull();
                createdResult.RouteValues["productRoleResourcePermissionId"].Should().Be(entity.Id);
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
            ProductRoleResourcePermission entity = new() { Id = Guid.NewGuid() };
            OperationResult<ProductRoleResourcePermission> operationResult = Operation.Fail<ProductRoleResourcePermission>("Failed to create product role resource permission.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.AddAsync(entity, cancellationToken)).ReturnsAsync(operationResult);

            ProductRoleResourcePermissionController controller = CreateController();

            // Act
            IActionResult result = await controller.Create(entity, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to create product role resource permission.");
            }
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenEntityIsUpdated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            ProductRoleResourcePermission entity = new() { Id = Guid.NewGuid() };
            OperationResult<ProductRoleResourcePermission> operationResult = Operation.Ok(entity);
            _repositoryMock.Setup(r => r.UpdateAsync(entity, cancellationToken)).ReturnsAsync(operationResult);

            ProductRoleResourcePermissionController controller = CreateController("Modifier UserName");

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
            ProductRoleResourcePermission entity = new() { Id = Guid.NewGuid() };
            OperationResult<ProductRoleResourcePermission> operationResult = Operation.Fail<ProductRoleResourcePermission>("Failed to update product role resource permission.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.UpdateAsync(entity, cancellationToken)).ReturnsAsync(operationResult);

            ProductRoleResourcePermissionController controller = CreateController();

            // Act
            IActionResult result = await controller.Update(entity, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to update product role resource permission.");
            }
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenEntityIsDeleted()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid id = Guid.NewGuid();
            OperationResult<ProductRoleResourcePermission> operationResult = Operation.Ok(new ProductRoleResourcePermission { Id = id });
            _repositoryMock.Setup(r => r.DeleteAsync(id, cancellationToken)).ReturnsAsync(operationResult);

            ProductRoleResourcePermissionController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(id, cancellationToken);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenEntityDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid id = Guid.NewGuid();
            OperationResult<ProductRoleResourcePermission> operationResult = Operation.Fail<ProductRoleResourcePermission>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _repositoryMock.Setup(r => r.DeleteAsync(id, cancellationToken)).ReturnsAsync(operationResult);

            ProductRoleResourcePermissionController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(id, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Product role resource permission not found with ID {id}");
            }
        }

        [Fact]
        public async Task Delete_ReturnsStatusCode500_WhenDeleteFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid id = Guid.NewGuid();
            OperationResult<ProductRoleResourcePermission> operationResult = Operation.Fail<ProductRoleResourcePermission>("Failed to delete product role resource permission.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.DeleteAsync(id, cancellationToken)).ReturnsAsync(operationResult);

            ProductRoleResourcePermissionController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(id, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to delete product role resource permission.");
            }
        }

        private ProductRoleResourcePermissionController CreateController(string? userName = null)
        {
            ProductRoleResourcePermissionController controller = new(_loggerMock.Object, _repositoryMock.Object)
            {
                ControllerContext = TestHelper.CreateControllerContext(userName)
            };

            return controller;
        }
    }
}