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
/// Unit tests for the <see cref="PartyRelationshipController"/> class.
/// </summary>
public class PartyRelationshipControllerTests
{
    private readonly Mock<ILogger<PartyRelationshipController>> _loggerMock = new();
    private readonly Mock<IPartyRelationshipRepository> _repositoryMock = new();

    [Fact]
    public async Task GetById_ReturnsOk_WhenRelationshipExists()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid relationshipId = Guid.NewGuid();
        OperationResult<PartyRelationship> operationResult = Operation.Ok(new PartyRelationship { Id = relationshipId });
        _repositoryMock.Setup(r => r.GetByIdAsync(relationshipId, cancellationToken)).ReturnsAsync(operationResult);

        PartyRelationshipController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(relationshipId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(operationResult.Entity);
        }
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenRelationshipDoesNotExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid relationshipId = Guid.NewGuid();
        OperationResult<PartyRelationship> operationResult = Operation.Fail<PartyRelationship>(It.IsAny<string>(), OperationErrorCode.NotFound);
        _repositoryMock.Setup(r => r.GetByIdAsync(relationshipId, cancellationToken)).ReturnsAsync(operationResult);

        PartyRelationshipController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(relationshipId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).Value.Should().Be($"Party relationship not found with ID {relationshipId}");
        }
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WhenRelationshipsExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        List<PartyRelationship> relationships = new() { new PartyRelationship { Id = Guid.NewGuid() } };
        OperationResult<IEnumerable<PartyRelationship>> operationResult = Operation.Ok<IEnumerable<PartyRelationship>>(relationships);
        _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

        PartyRelationshipController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().BeEquivalentTo(relationships);
        }
    }

    [Fact]
    public async Task GetAll_ReturnsStatusCode500_WhenRepositoryFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        OperationResult<IEnumerable<PartyRelationship>> operationResult = Operation.Fail<IEnumerable<PartyRelationship>>("Failed to get party relationships.", OperationErrorCode.SqlError);
        _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

        PartyRelationshipController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to get party relationships.");
        }
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenRelationshipIsCreated()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        PartyRelationship relationship = new() { Id = Guid.NewGuid() };
        OperationResult<PartyRelationship> operationResult = Operation.Ok(relationship);
        _repositoryMock.Setup(r => r.AddAsync(relationship, cancellationToken)).ReturnsAsync(operationResult);

        PartyRelationshipController controller = CreateController("Creator UserName");

        // Act
        IActionResult result = await controller.Create(relationship, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<CreatedAtActionResult>();
            CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
            createdResult.Value.Should().Be(relationship);
            createdResult.RouteValues.Should().NotBeNull();
            createdResult.RouteValues["partyRelationshipId"].Should().Be(relationship.Id);
            relationship.CreatedBy.Should().Be("Creator UserName");
            relationship.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            relationship.ModifiedBy.Should().BeNull();
            relationship.Modified.Should().BeNull();
        }
    }

    [Fact]
    public async Task Create_ReturnsStatusCode500_WhenRelationshipCreationFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        PartyRelationship relationship = new() { Id = Guid.NewGuid() };
        OperationResult<PartyRelationship> operationResult = Operation.Fail<PartyRelationship>("Failed to create party relationship.", OperationErrorCode.SqlError);
        _repositoryMock.Setup(r => r.AddAsync(relationship, cancellationToken)).ReturnsAsync(operationResult);

        PartyRelationshipController controller = CreateController();

        // Act
        IActionResult result = await controller.Create(relationship, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to create party relationship.");
        }
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenRelationshipIsUpdated()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        PartyRelationship relationship = new() { Id = Guid.NewGuid() };
        OperationResult<PartyRelationship> operationResult = Operation.Ok(relationship);
        _repositoryMock.Setup(r => r.UpdateAsync(relationship, cancellationToken)).ReturnsAsync(operationResult);

        PartyRelationshipController controller = CreateController("Modifier UserName");

        // Act
        IActionResult result = await controller.Update(relationship, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(relationship);
            relationship.ModifiedBy.Should().Be("Modifier UserName");
            relationship.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }

    [Fact]
    public async Task Update_ReturnsStatusCode500_WhenRelationshipUpdateFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        PartyRelationship relationship = new() { Id = Guid.NewGuid() };
        OperationResult<PartyRelationship> operationResult = Operation.Fail<PartyRelationship>("Failed to update party relationship.", OperationErrorCode.SqlError);
        _repositoryMock.Setup(r => r.UpdateAsync(relationship, cancellationToken)).ReturnsAsync(operationResult);

        PartyRelationshipController controller = CreateController();

        // Act
        IActionResult result = await controller.Update(relationship, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to update party relationship.");
        }
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenRelationshipIsDeleted()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid relationshipId = Guid.NewGuid();
        OperationResult<PartyRelationship> operationResult = Operation.Ok(new PartyRelationship { Id = relationshipId });
        _repositoryMock.Setup(r => r.DeleteAsync(relationshipId, cancellationToken)).ReturnsAsync(operationResult);

        PartyRelationshipController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(relationshipId, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenRelationshipDoesNotExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid relationshipId = Guid.NewGuid();
        OperationResult<PartyRelationship> operationResult = Operation.Fail<PartyRelationship>(It.IsAny<string>(), OperationErrorCode.NotFound);
        _repositoryMock.Setup(r => r.DeleteAsync(relationshipId, cancellationToken)).ReturnsAsync(operationResult);

        PartyRelationshipController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(relationshipId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).Value.Should().Be($"Party relationship not found with ID {relationshipId}");
        }
    }

    [Fact]
    public async Task Delete_ReturnsStatusCode500_WhenRelationshipDeleteFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid relationshipId = Guid.NewGuid();
        OperationResult<PartyRelationship> operationResult = Operation.Fail<PartyRelationship>("Failed to delete party relationship.", OperationErrorCode.SqlError);
        _repositoryMock.Setup(r => r.DeleteAsync(relationshipId, cancellationToken)).ReturnsAsync(operationResult);

        PartyRelationshipController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(relationshipId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to delete party relationship.");
        }
    }

    private PartyRelationshipController CreateController(string? userName = null)
    {
        PartyRelationshipController controller = new(_loggerMock.Object, _repositoryMock.Object)
        {
            ControllerContext = TestHelper.CreateControllerContext(userName)
        };

        return controller;
    }
}