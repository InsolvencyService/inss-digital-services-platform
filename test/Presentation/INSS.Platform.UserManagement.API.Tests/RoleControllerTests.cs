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
/// Unit tests for the <see cref="RoleController"/> class.
/// </summary>
public class RoleControllerTests
{
    private readonly Mock<ILogger<RoleController>> _loggerMock = new();
    private readonly Mock<IRoleRepository> _roleRepositoryMock = new();

    [Fact]
    public async Task Get_ReturnsOk_WhenRoleExists()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid roleId = Guid.NewGuid();
        OperationResult<Role> operationResult = Operation.Ok(new Role { Id = roleId, Name = "TestRole", Description = "TestDesc" });
        _roleRepositoryMock.Setup(r => r.GetByIdAsync(roleId, cancellationToken)).ReturnsAsync(operationResult);

        RoleController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(roleId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(operationResult.Entity);
        }
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenRoleDoesNotExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid roleId = Guid.NewGuid();
        OperationResult<Role> operationResult = Operation.Fail<Role>(It.IsAny<string>(), OperationErrorCode.NotFound);
        _roleRepositoryMock.Setup(r => r.GetByIdAsync(roleId, cancellationToken)).ReturnsAsync(operationResult);

        RoleController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(roleId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).Value.Should().Be($"Role not found with ID {roleId}");
        }
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WhenRolesExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        List<Role> roles = new() { new Role { Id = Guid.NewGuid(), Name = "TestRole", Description = "TestDesc" } };
        OperationResult<IEnumerable<Role>> operationResult = Operation.Ok<IEnumerable<Role>>(roles);
        _roleRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

        RoleController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().BeEquivalentTo(roles);
        }
    }

    [Fact]
    public async Task GetAll_ReturnsStatusCode500_WhenRepositoryFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        OperationResult<IEnumerable<Role>> operationResult = Operation.Fail<IEnumerable<Role>>("Failed to get roles.", OperationErrorCode.SqlError);
        _roleRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

        RoleController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to get roles.");
        }
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenRoleIsCreated()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        OperationResult<Role> operationResult = Operation.Ok(new Role { Id = Guid.NewGuid(), Name = "TestRole", Description = "TestDesc" });
        _roleRepositoryMock.Setup(r => r.AddAsync(operationResult.Entity!, cancellationToken)).ReturnsAsync(operationResult);

        RoleController controller = CreateController("Creator UserName");

        // Act
        IActionResult result = await controller.Create(operationResult.Entity!, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<CreatedAtActionResult>();
            CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
            createdResult.Value.Should().Be(operationResult.Entity);
            createdResult.RouteValues.Should().NotBeNull();
            createdResult.RouteValues!["roleId"].Should().Be(operationResult.Entity!.Id);
            operationResult.Entity.CreatedBy.Should().Be("Creator UserName");
            operationResult.Entity.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            operationResult.Entity.ModifiedBy.Should().BeNull();
            operationResult.Entity.Modified.Should().BeNull();
        }
    }

    [Fact]
    public async Task Create_ReturnsStatusCode500_WhenRoleCreationFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Role role = new() { Id = Guid.NewGuid(), Name = "TestRole", Description = "TestDesc" };
        OperationResult<Role> operationResult = Operation.Fail<Role>("Failed to create role.", OperationErrorCode.SqlError);
        _roleRepositoryMock.Setup(r => r.AddAsync(role, cancellationToken)).ReturnsAsync(operationResult);

        RoleController controller = CreateController();

        // Act
        IActionResult result = await controller.Create(role, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to create role.");
        }
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenRoleIsUpdated()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        OperationResult<Role> operationResult = Operation.Ok(new Role { Id = Guid.NewGuid(), Name = "UpdatedRole", Description = "TestDesc" });
        _roleRepositoryMock.Setup(r => r.UpdateAsync(operationResult.Entity!, cancellationToken)).ReturnsAsync(operationResult);

        RoleController controller = CreateController("Modifier UserName");

        // Act
        IActionResult result = await controller.Update(operationResult.Entity!, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().BeOfType<Role>();
            Role role = (Role)((OkObjectResult)result).Value!;
            role.ModifiedBy.Should().Be("Modifier UserName");
            role.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }

    [Fact]
    public async Task Update_ReturnsStatusCode500_WhenRoleUpdateFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Role role = new() { Id = Guid.NewGuid(), Name = "UpdatedRole", Description = "TestDesc" };
        OperationResult<Role> operationResult = Operation.Fail<Role>("Failed to update role.", OperationErrorCode.SqlError);
        _roleRepositoryMock.Setup(r => r.UpdateAsync(role, cancellationToken)).ReturnsAsync(operationResult);

        RoleController controller = CreateController();

        // Act
        IActionResult result = await controller.Update(role, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to update role.");
        }
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenRoleIsDeleted()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid roleId = Guid.NewGuid();
        OperationResult<Role> operationResult = Operation.Ok(new Role { Id = Guid.NewGuid(), Name = "RoleToDelete", Description = "TestDesc" });
        _roleRepositoryMock.Setup(r => r.GetByIdAsync(roleId, cancellationToken)).ReturnsAsync(operationResult);
        _roleRepositoryMock.Setup(r => r.DeleteAsync(roleId, cancellationToken)).ReturnsAsync(operationResult);

        RoleController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(roleId, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenRoleDoesNotExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid roleId = Guid.NewGuid();
        OperationResult<Role> operationResult = Operation.Fail<Role>(It.IsAny<string>(), OperationErrorCode.NotFound);
        _roleRepositoryMock.Setup(r => r.DeleteAsync(roleId, cancellationToken)).ReturnsAsync(operationResult);

        RoleController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(roleId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).Value.Should().Be($"Role not found with ID {roleId}");
        }
    }

    [Fact]
    public async Task Delete_ReturnsStatusCode500_WhenRoleDeleteFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid roleId = Guid.NewGuid();
        OperationResult<Role> operationResult = Operation.Fail<Role>("Failed to delete role.", OperationErrorCode.SqlError);
        _roleRepositoryMock.Setup(r => r.DeleteAsync(roleId, cancellationToken)).ReturnsAsync(operationResult);

        RoleController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(roleId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to delete role.");
        }
    }

    private RoleController CreateController(string? userName = null)
    {
        RoleController controller = new(_loggerMock.Object, _roleRepositoryMock.Object)
        {
            ControllerContext = TestHelper.CreateControllerContext(userName)
        };

        return controller;
    }
}