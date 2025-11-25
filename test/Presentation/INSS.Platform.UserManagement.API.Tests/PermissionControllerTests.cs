using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.UserManagement.API.Controllers;
using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace INSS.Platform.UserManagement.API.Tests;

/// <summary>
/// Unit tests for the <see cref="PermissionController"/> class.
/// </summary>
public class PermissionControllerTests
{
    private readonly Mock<ILogger<PermissionController>> _loggerMock = new();
    private readonly Mock<IPermissionRepository> _repositoryMock = new();

    [Fact]
    public async Task GetById_ReturnsOk_WhenPermissionExists()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid permissionId = Guid.NewGuid();
        OperationResult<Permission> operationResult = Operation.Ok(new Permission { Id = permissionId });
        _repositoryMock.Setup(r => r.GetByIdAsync(permissionId, cancellationToken)).ReturnsAsync(operationResult);

        PermissionController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(permissionId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(operationResult.Entity);
        }
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenPermissionDoesNotExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid permissionId = Guid.NewGuid();
        OperationResult<Permission> operationResult = Operation.Fail<Permission>(It.IsAny<string>(), OperationErrorCode.NotFound);
        _repositoryMock.Setup(r => r.GetByIdAsync(permissionId, cancellationToken)).ReturnsAsync(operationResult);

        PermissionController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(permissionId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).Value.Should().Be($"Permission not found with ID {permissionId}");
        }
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WhenPermissionsExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        List<Permission> permissions = new() { new Permission { Id = Guid.NewGuid() } };
        OperationResult<IEnumerable<Permission>> operationResult = Operation.Ok<IEnumerable<Permission>>(permissions);
        _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

        PermissionController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().BeEquivalentTo(permissions);
        }
    }

    [Fact]
    public async Task GetAll_ReturnsStatusCode500_WhenRepositoryFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        OperationResult<IEnumerable<Permission>> operationResult = Operation.Fail<IEnumerable<Permission>>("Failed to get permissions.", OperationErrorCode.SqlError);
        _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

        PermissionController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to get permissions.");
        }
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenPermissionIsCreated()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Permission permission = new() { Id = Guid.NewGuid() };
        OperationResult<Permission> operationResult = Operation.Ok(permission);
        _repositoryMock.Setup(r => r.AddAsync(permission, cancellationToken)).ReturnsAsync(operationResult);

        PermissionController controller = CreateController("Creator UserName");

        // Act
        IActionResult result = await controller.Create(permission, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<CreatedAtActionResult>();
            CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
            createdResult.Value.Should().Be(permission);
            createdResult.RouteValues.Should().NotBeNull();
            createdResult.RouteValues["permissionId"].Should().Be(permission.Id);
            permission.CreatedBy.Should().Be("Creator UserName");
            permission.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            permission.ModifiedBy.Should().BeNull();
            permission.Modified.Should().BeNull();
        }
    }

    [Fact]
    public async Task Create_ReturnsStatusCode500_WhenPermissionCreationFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Permission permission = new() { Id = Guid.NewGuid() };
        OperationResult<Permission> operationResult = Operation.Fail<Permission>("Failed to create permission.", OperationErrorCode.SqlError);
        _repositoryMock.Setup(r => r.AddAsync(permission, cancellationToken)).ReturnsAsync(operationResult);

        PermissionController controller = CreateController();

        // Act
        IActionResult result = await controller.Create(permission, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to create permission.");
        }
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenPermissionIsUpdated()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Permission permission = new() { Id = Guid.NewGuid() };
        OperationResult<Permission> operationResult = Operation.Ok(permission);
        _repositoryMock.Setup(r => r.UpdateAsync(permission, cancellationToken)).ReturnsAsync(operationResult);

        PermissionController controller = CreateController("Modifier UserName");

        // Act
        IActionResult result = await controller.Update(permission, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(permission);
            permission.ModifiedBy.Should().Be("Modifier UserName");
            permission.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }

    [Fact]
    public async Task Update_ReturnsStatusCode500_WhenPermissionUpdateFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Permission permission = new() { Id = Guid.NewGuid() };
        OperationResult<Permission> operationResult = Operation.Fail<Permission>("Failed to update permission.", OperationErrorCode.SqlError);
        _repositoryMock.Setup(r => r.UpdateAsync(permission, cancellationToken)).ReturnsAsync(operationResult);

        PermissionController controller = CreateController();

        // Act
        IActionResult result = await controller.Update(permission, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to update permission.");
        }
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenPermissionIsDeleted()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid permissionId = Guid.NewGuid();
        OperationResult<Permission> operationResult = Operation.Ok(new Permission { Id = permissionId });
        _repositoryMock.Setup(r => r.DeleteAsync(permissionId, cancellationToken)).ReturnsAsync(operationResult);

        PermissionController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(permissionId, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenPermissionDoesNotExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid permissionId = Guid.NewGuid();
        OperationResult<Permission> operationResult = Operation.Fail<Permission>(It.IsAny<string>(), OperationErrorCode.NotFound);
        _repositoryMock.Setup(r => r.DeleteAsync(permissionId, cancellationToken)).ReturnsAsync(operationResult);

        PermissionController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(permissionId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).Value.Should().Be($"Permission not found with ID {permissionId}");
        }
    }

    [Fact]
    public async Task Delete_ReturnsStatusCode500_WhenPermissionDeleteFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid permissionId = Guid.NewGuid();
        OperationResult<Permission> operationResult = Operation.Fail<Permission>("Failed to delete permission.", OperationErrorCode.SqlError);
        _repositoryMock.Setup(r => r.DeleteAsync(permissionId, cancellationToken)).ReturnsAsync(operationResult);

        PermissionController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(permissionId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to delete permission.");
        }
    }

    private PermissionController CreateController(string? userName = null)
    {
        PermissionController controller = new(_loggerMock.Object, _repositoryMock.Object)
        {
            ControllerContext = TestHelper.CreateControllerContext(userName)
        };

        return controller;
    }
}