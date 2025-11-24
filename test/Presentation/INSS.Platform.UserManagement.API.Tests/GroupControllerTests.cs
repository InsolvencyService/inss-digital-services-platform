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
/// Unit tests for the <see cref="GroupController"/> class.
/// </summary>
public class GroupControllerTests
{
    private readonly Mock<ILogger<GroupController>> _loggerMock = new();
    private readonly Mock<IGroupRepository> _groupRepositoryMock = new();

    [Fact]
    public async Task GetById_ReturnsOk_WhenGroupExists()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid groupId = Guid.NewGuid();
        OperationResult<Group> operationResult = Operation.Ok(new Group { Id = groupId });
        _groupRepositoryMock.Setup(r => r.GetByIdAsync(groupId, cancellationToken)).ReturnsAsync(operationResult);

        GroupController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(groupId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(operationResult.Entity);
        }
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenGroupDoesNotExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid groupId = Guid.NewGuid();
        OperationResult<Group> operationResult = Operation.Fail<Group>(It.IsAny<string>(), OperationErrorCode.NotFound);
        _groupRepositoryMock.Setup(r => r.GetByIdAsync(groupId, cancellationToken)).ReturnsAsync(operationResult);

        GroupController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(groupId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).Value.Should().Be($"Group not found with ID {groupId}");
        }
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WhenGroupsExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        List<Group> groups = new() { new Group { Id = Guid.NewGuid() } };
        OperationResult<IEnumerable<Group>> operationResult = Operation.Ok<IEnumerable<Group>>(groups);
        _groupRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

        GroupController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().BeEquivalentTo(groups);
        }
    }

    [Fact]
    public async Task GetAll_ReturnsStatusCode500_WhenRepositoryFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        OperationResult<IEnumerable<Group>> operationResult = Operation.Fail<IEnumerable<Group>>("Failed to get groups.", OperationErrorCode.SqlError);
        _groupRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

        GroupController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to get groups.");
        }
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenGroupIsCreated()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Group group = new() { Id = Guid.NewGuid() };
        OperationResult<Group> operationResult = Operation.Ok(group);
        _groupRepositoryMock.Setup(r => r.AddAsync(group, cancellationToken)).ReturnsAsync(operationResult);

        GroupController controller = CreateController("Creator UserName");

        // Act
        IActionResult result = await controller.Create(group, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<CreatedAtActionResult>();
            CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
            createdResult.Value.Should().Be(group);
            createdResult.RouteValues.Should().NotBeNull();
            createdResult.RouteValues["groupId"].Should().Be(group.Id);
            group.CreatedBy.Should().Be("Creator UserName");
            group.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            group.ModifiedBy.Should().BeNull();
            group.Modified.Should().BeNull();
        }
    }

    [Fact]
    public async Task Create_ReturnsStatusCode500_WhenGroupCreationFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Group group = new() { Id = Guid.NewGuid() };
        OperationResult<Group> operationResult = Operation.Fail<Group>("Failed to create group.", OperationErrorCode.SqlError);
        _groupRepositoryMock.Setup(r => r.AddAsync(group, cancellationToken)).ReturnsAsync(operationResult);

        GroupController controller = CreateController();

        // Act
        IActionResult result = await controller.Create(group, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to create group.");
        }
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenGroupIsUpdated()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Group group = new() { Id = Guid.NewGuid() };
        OperationResult<Group> operationResult = Operation.Ok(group);
        _groupRepositoryMock.Setup(r => r.UpdateAsync(group, cancellationToken)).ReturnsAsync(operationResult);

        GroupController controller = CreateController("Modifier UserName");

        // Act
        IActionResult result = await controller.Update(group, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(group);
            group.ModifiedBy.Should().Be("Modifier UserName");
            group.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }

    [Fact]
    public async Task Update_ReturnsStatusCode500_WhenGroupUpdateFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Group group = new() { Id = Guid.NewGuid() };
        OperationResult<Group> operationResult = Operation.Fail<Group>("Failed to update group.", OperationErrorCode.SqlError);
        _groupRepositoryMock.Setup(r => r.UpdateAsync(group, cancellationToken)).ReturnsAsync(operationResult);

        GroupController controller = CreateController();

        // Act
        IActionResult result = await controller.Update(group, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to update group.");
        }
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenGroupIsDeleted()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid groupId = Guid.NewGuid();
        OperationResult<Group> operationResult = Operation.Ok(new Group { Id = groupId });
        _groupRepositoryMock.Setup(r => r.DeleteAsync(groupId, cancellationToken)).ReturnsAsync(operationResult);

        GroupController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(groupId, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenGroupDoesNotExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid groupId = Guid.NewGuid();
        OperationResult<Group> operationResult = Operation.Fail<Group>(It.IsAny<string>(), OperationErrorCode.NotFound);
        _groupRepositoryMock.Setup(r => r.DeleteAsync(groupId, cancellationToken)).ReturnsAsync(operationResult);

        GroupController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(groupId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).Value.Should().Be($"Group not found with ID {groupId}");
        }
    }

    [Fact]
    public async Task Delete_ReturnsStatusCode500_WhenGroupDeleteFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid groupId = Guid.NewGuid();
        OperationResult<Group> operationResult = Operation.Fail<Group>("Failed to delete group.", OperationErrorCode.SqlError);
        _groupRepositoryMock.Setup(r => r.DeleteAsync(groupId, cancellationToken)).ReturnsAsync(operationResult);

        GroupController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(groupId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to delete group.");
        }
    }

    private GroupController CreateController(string? userName = null)
    {
        GroupController controller = new(_loggerMock.Object, _groupRepositoryMock.Object)
        {
            ControllerContext = TestHelper.CreateControllerContext(userName)
        };

        return controller;
    }
}