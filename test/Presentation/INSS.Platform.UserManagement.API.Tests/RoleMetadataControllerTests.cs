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
/// Unit tests for the <see cref="RoleMetadataController"/> class.
/// </summary>
public class RoleMetadataControllerTests
{
    private readonly Mock<ILogger<RoleMetadataController>> _loggerMock = new();
    private readonly Mock<IRoleMetadataRepository> _repositoryMock = new();

    [Fact]
    public async Task GetById_ReturnsOk_WhenRoleMetadataExists()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid roleMetadataId = Guid.NewGuid();
        OperationResult<RoleMetadata> operationResult = Operation.Ok(new RoleMetadata { Id = roleMetadataId });
        _repositoryMock.Setup(r => r.GetByIdAsync(roleMetadataId, cancellationToken)).ReturnsAsync(operationResult);

        RoleMetadataController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(roleMetadataId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(operationResult.Entity);
        }
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenRoleMetadataDoesNotExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid roleMetadataId = Guid.NewGuid();
        OperationResult<RoleMetadata> operationResult = Operation.Fail<RoleMetadata>(It.IsAny<string>(), OperationErrorCode.NotFound);
        _repositoryMock.Setup(r => r.GetByIdAsync(roleMetadataId, cancellationToken)).ReturnsAsync(operationResult);

        RoleMetadataController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(roleMetadataId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).Value.Should().Be($"Role metadata not found with ID {roleMetadataId}");
        }
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WhenRoleMetadatasExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        List<RoleMetadata> entities = new() { new RoleMetadata { Id = Guid.NewGuid() } };
        OperationResult<IEnumerable<RoleMetadata>> operationResult = Operation.Ok<IEnumerable<RoleMetadata>>(entities);
        _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

        RoleMetadataController controller = CreateController();

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
        OperationResult<IEnumerable<RoleMetadata>> operationResult = Operation.Fail<IEnumerable<RoleMetadata>>("Failed to get role metadata.", OperationErrorCode.SqlError);
        _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

        RoleMetadataController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to get role metadata.");
        }
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenRoleMetadataIsCreated()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        RoleMetadata entity = new() { Id = Guid.NewGuid() };
        OperationResult<RoleMetadata> operationResult = Operation.Ok(entity);
        _repositoryMock.Setup(r => r.AddAsync(entity, cancellationToken)).ReturnsAsync(operationResult);

        RoleMetadataController controller = CreateController("Creator UserName");

        // Act
        IActionResult result = await controller.Create(entity, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<CreatedAtActionResult>();
            CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
            createdResult.Value.Should().Be(entity);
            createdResult.RouteValues.Should().NotBeNull();
            createdResult.RouteValues["roleMetadataId"].Should().Be(entity.Id);
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
        RoleMetadata entity = new() { Id = Guid.NewGuid() };
        OperationResult<RoleMetadata> operationResult = Operation.Fail<RoleMetadata>("Failed to create role metadata.", OperationErrorCode.SqlError);
        _repositoryMock.Setup(r => r.AddAsync(entity, cancellationToken)).ReturnsAsync(operationResult);

        RoleMetadataController controller = CreateController();

        // Act
        IActionResult result = await controller.Create(entity, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to create role metadata.");
        }
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenRoleMetadataIsUpdated()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        RoleMetadata entity = new() { Id = Guid.NewGuid() };
        OperationResult<RoleMetadata> operationResult = Operation.Ok(entity);
        _repositoryMock.Setup(r => r.UpdateAsync(entity, cancellationToken)).ReturnsAsync(operationResult);

        RoleMetadataController controller = CreateController("Modifier UserName");

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
        RoleMetadata entity = new() { Id = Guid.NewGuid() };
        OperationResult<RoleMetadata> operationResult = Operation.Fail<RoleMetadata>("Failed to update role metadata.", OperationErrorCode.SqlError);
        _repositoryMock.Setup(r => r.UpdateAsync(entity, cancellationToken)).ReturnsAsync(operationResult);

        RoleMetadataController controller = CreateController();

        // Act
        IActionResult result = await controller.Update(entity, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to update role metadata.");
        }
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenRoleMetadataIsDeleted()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid roleMetadataId = Guid.NewGuid();
        OperationResult<RoleMetadata> operationResult = Operation.Ok(new RoleMetadata { Id = roleMetadataId });
        _repositoryMock.Setup(r => r.DeleteAsync(roleMetadataId, cancellationToken)).ReturnsAsync(operationResult);

        RoleMetadataController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(roleMetadataId, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenRoleMetadataDoesNotExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid roleMetadataId = Guid.NewGuid();
        OperationResult<RoleMetadata> operationResult = Operation.Fail<RoleMetadata>(It.IsAny<string>(), OperationErrorCode.NotFound);
        _repositoryMock.Setup(r => r.DeleteAsync(roleMetadataId, cancellationToken)).ReturnsAsync(operationResult);

        RoleMetadataController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(roleMetadataId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).Value.Should().Be($"Role metadata not found with ID {roleMetadataId}");
        }
    }

    [Fact]
    public async Task Delete_ReturnsStatusCode500_WhenDeleteFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid roleMetadataId = Guid.NewGuid();
        OperationResult<RoleMetadata> operationResult = Operation.Fail<RoleMetadata>("Failed to delete role metadata.", OperationErrorCode.SqlError);
        _repositoryMock.Setup(r => r.DeleteAsync(roleMetadataId, cancellationToken)).ReturnsAsync(operationResult);

        RoleMetadataController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(roleMetadataId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to delete role metadata.");
        }
    }

    private RoleMetadataController CreateController(string? userName = null)
    {
        RoleMetadataController controller = new(_loggerMock.Object, _repositoryMock.Object)
        {
            ControllerContext = TestHelper.CreateControllerContext(userName)
        };

        return controller;
    }
}