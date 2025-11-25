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
/// Unit tests for the <see cref="PartyAuthenticationProviderMetadataController"/> class.
/// </summary>
public class PartyAuthenticationProviderMetadataControllerTests
{
    private readonly Mock<ILogger<PartyAuthenticationProviderMetadataController>> _loggerMock = new();
    private readonly Mock<IPartyAuthenticationProviderMetadataRepository> _repositoryMock = new();

    [Fact]
    public async Task GetById_ReturnsOk_WhenMetadataExists()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid metadataId = Guid.NewGuid();
        OperationResult<PartyAuthenticationProviderMetadata> operationResult = Operation.Ok(new PartyAuthenticationProviderMetadata { Id = metadataId });
        _repositoryMock.Setup(r => r.GetByIdAsync(metadataId, cancellationToken)).ReturnsAsync(operationResult);

        PartyAuthenticationProviderMetadataController controller = CreateController();

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
        OperationResult<PartyAuthenticationProviderMetadata> operationResult = Operation.Fail<PartyAuthenticationProviderMetadata>(It.IsAny<string>(), OperationErrorCode.NotFound);
        _repositoryMock.Setup(r => r.GetByIdAsync(metadataId, cancellationToken)).ReturnsAsync(operationResult);

        PartyAuthenticationProviderMetadataController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(metadataId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).Value.Should().Be($"Party authentication provider metadata not found with ID {metadataId}");
        }
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WhenMetadataExists()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        List<PartyAuthenticationProviderMetadata> metadataList = new() { new PartyAuthenticationProviderMetadata { Id = Guid.NewGuid() } };
        OperationResult<IEnumerable<PartyAuthenticationProviderMetadata>> operationResult = Operation.Ok<IEnumerable<PartyAuthenticationProviderMetadata>>(metadataList);
        _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

        PartyAuthenticationProviderMetadataController controller = CreateController();

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
        OperationResult<IEnumerable<PartyAuthenticationProviderMetadata>> operationResult = Operation.Fail<IEnumerable<PartyAuthenticationProviderMetadata>>("Failed to get party authentication provider metadata.", OperationErrorCode.SqlError);
        _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

        PartyAuthenticationProviderMetadataController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to get party authentication provider metadata.");
        }
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenMetadataIsCreated()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        PartyAuthenticationProviderMetadata metadata = new() { Id = Guid.NewGuid() };
        OperationResult<PartyAuthenticationProviderMetadata> operationResult = Operation.Ok(metadata);
        _repositoryMock.Setup(r => r.AddAsync(metadata, cancellationToken)).ReturnsAsync(operationResult);

        PartyAuthenticationProviderMetadataController controller = CreateController("Creator UserName");

        // Act
        IActionResult result = await controller.Create(metadata, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<CreatedAtActionResult>();
            CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
            createdResult.Value.Should().Be(metadata);
            createdResult.RouteValues.Should().NotBeNull();
            createdResult.RouteValues["partyAuthenticationProviderMetadataId"].Should().Be(metadata.Id);
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
        PartyAuthenticationProviderMetadata metadata = new() { Id = Guid.NewGuid() };
        OperationResult<PartyAuthenticationProviderMetadata> operationResult = Operation.Fail<PartyAuthenticationProviderMetadata>("Failed to create party authentication provider metadata.", OperationErrorCode.SqlError);
        _repositoryMock.Setup(r => r.AddAsync(metadata, cancellationToken)).ReturnsAsync(operationResult);

        PartyAuthenticationProviderMetadataController controller = CreateController();

        // Act
        IActionResult result = await controller.Create(metadata, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to create party authentication provider metadata.");
        }
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenMetadataIsUpdated()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        PartyAuthenticationProviderMetadata metadata = new() { Id = Guid.NewGuid() };
        OperationResult<PartyAuthenticationProviderMetadata> operationResult = Operation.Ok(metadata);
        _repositoryMock.Setup(r => r.UpdateAsync(metadata, cancellationToken)).ReturnsAsync(operationResult);

        PartyAuthenticationProviderMetadataController controller = CreateController("Modifier UserName");

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
        PartyAuthenticationProviderMetadata metadata = new() { Id = Guid.NewGuid() };
        OperationResult<PartyAuthenticationProviderMetadata> operationResult = Operation.Fail<PartyAuthenticationProviderMetadata>("Failed to update party authentication provider metadata.", OperationErrorCode.SqlError);
        _repositoryMock.Setup(r => r.UpdateAsync(metadata, cancellationToken)).ReturnsAsync(operationResult);

        PartyAuthenticationProviderMetadataController controller = CreateController();

        // Act
        IActionResult result = await controller.Update(metadata, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to update party authentication provider metadata.");
        }
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenMetadataIsDeleted()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid metadataId = Guid.NewGuid();
        OperationResult<PartyAuthenticationProviderMetadata> operationResult = Operation.Ok(new PartyAuthenticationProviderMetadata { Id = metadataId });
        _repositoryMock.Setup(r => r.DeleteAsync(metadataId, cancellationToken)).ReturnsAsync(operationResult);

        PartyAuthenticationProviderMetadataController controller = CreateController();

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
        OperationResult<PartyAuthenticationProviderMetadata> operationResult = Operation.Fail<PartyAuthenticationProviderMetadata>(It.IsAny<string>(), OperationErrorCode.NotFound);
        _repositoryMock.Setup(r => r.DeleteAsync(metadataId, cancellationToken)).ReturnsAsync(operationResult);

        PartyAuthenticationProviderMetadataController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(metadataId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).Value.Should().Be($"Party authentication provider metadata not found with ID {metadataId}");
        }
    }

    [Fact]
    public async Task Delete_ReturnsStatusCode500_WhenMetadataDeleteFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid metadataId = Guid.NewGuid();
        OperationResult<PartyAuthenticationProviderMetadata> operationResult = Operation.Fail<PartyAuthenticationProviderMetadata>("Failed to delete party authentication provider metadata.", OperationErrorCode.SqlError);
        _repositoryMock.Setup(r => r.DeleteAsync(metadataId, cancellationToken)).ReturnsAsync(operationResult);

        PartyAuthenticationProviderMetadataController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(metadataId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to delete party authentication provider metadata.");
        }
    }

    private PartyAuthenticationProviderMetadataController CreateController(string? userName = null)
    {
        PartyAuthenticationProviderMetadataController controller = new(_loggerMock.Object, _repositoryMock.Object)
        {
            ControllerContext = TestHelper.CreateControllerContext(userName)
        };

        return controller;
    }
}