using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Application.Services;
using INSS.Platform.UserManagement.Domain;
using Moq;

namespace INSS.Platform.UserManagement.Application.Tests;

public class RelationshipTypeLookupServiceTests
{
    private readonly Mock<IRelationshipTypeRepository> _repositoryMock;
    private readonly RelationshipTypeLookupService _service;

    public RelationshipTypeLookupServiceTests()
    {
        _repositoryMock = new Mock<IRelationshipTypeRepository>();
        _service = new RelationshipTypeLookupService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetByNameAsync_WhenRepositoryReturnsSuccessResult_ReturnsEntity()
    {
        // Arrange
        string relationshipName = "Parent-Child";
        RelationshipType expectedRelationshipType = new ()
        {
            Id = Guid.NewGuid(),
            Name = relationshipName,
            Description = "Parent to child relationship"
        };

        OperationResult<RelationshipType> operationResult = new ()
        {
            Success = true,
            Entity = expectedRelationshipType,
            ErrorCode = OperationErrorCode.None,
            ErrorMessage = null
        };

        _repositoryMock
            .Setup(x => x.GetByNameAsync(relationshipName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(operationResult);

        // Act
        RelationshipType? result = await _service.GetByNameAsync(relationshipName, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedRelationshipType.Id, result.Id);
        Assert.Equal(expectedRelationshipType.Name, result.Name);
        Assert.Equal(expectedRelationshipType.Description, result.Description);
    }

    [Fact]
    public async Task GetByNameAsync_WhenRepositoryReturnsNullEntity_ReturnsNull()
    {
        // Arrange
        string relationshipName = "NonExistent";
        OperationResult<RelationshipType> operationResult = new ()
        {
            Success = false,
            Entity = null,
            ErrorCode = OperationErrorCode.NotFound,
            ErrorMessage = "Relationship type not found"
        };

        _repositoryMock
            .Setup(x => x.GetByNameAsync(relationshipName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(operationResult);

        // Act
        RelationshipType? result = await _service.GetByNameAsync(relationshipName, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_CallsRepositoryWithCorrectName()
    {
        // Arrange
        string relationshipName = "Employer-Employee";
        OperationResult<RelationshipType> operationResult = new ()
        {
            Success = true,
            Entity = new RelationshipType { Name = relationshipName },
            ErrorCode = OperationErrorCode.None
        };

        _repositoryMock
            .Setup(x => x.GetByNameAsync(relationshipName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(operationResult);

        // Act
        await _service.GetByNameAsync(relationshipName, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(
            x => x.GetByNameAsync(relationshipName, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByNameAsync_PassesCancellationTokenToRepository()
    {
        // Arrange
        string relationshipName = "Manager-Employee";
        CancellationToken cancellationToken = new ();
        OperationResult<RelationshipType> operationResult = new ()
        {
            Success = true,
            Entity = new RelationshipType { Name = relationshipName },
            ErrorCode = OperationErrorCode.None
        };

        _repositoryMock
            .Setup(x => x.GetByNameAsync(relationshipName, cancellationToken))
            .ReturnsAsync(operationResult);

        // Act
        await _service.GetByNameAsync(relationshipName, cancellationToken);

        // Assert
        _repositoryMock.Verify(
            x => x.GetByNameAsync(relationshipName, cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task GetByNameAsync_WithEmptyName_CallsRepository()
    {
        // Arrange
        string emptyName = string.Empty;
        OperationResult<RelationshipType> operationResult = new ()
        {
            Success = false,
            Entity = null,
            ErrorCode = OperationErrorCode.NotFound
        };

        _repositoryMock
            .Setup(x => x.GetByNameAsync(emptyName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(operationResult);

        // Act
        RelationshipType? result = await _service.GetByNameAsync(emptyName, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(
            x => x.GetByNameAsync(emptyName, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByNameAsync_WithWhitespaceName_CallsRepository()
    {
        // Arrange
        string whitespaceName = "   ";
        OperationResult<RelationshipType> operationResult = new ()
        {
            Success = false,
            Entity = null,
            ErrorCode = OperationErrorCode.NotFound
        };

        _repositoryMock
            .Setup(x => x.GetByNameAsync(whitespaceName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(operationResult);

        // Act
        RelationshipType? result = await _service.GetByNameAsync(whitespaceName, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(
            x => x.GetByNameAsync(whitespaceName, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByNameAsync_WhenRepositoryThrowsException_PropagatesException()
    {
        // Arrange
        string relationshipName = "Test";
        Exception expectedException = new InvalidOperationException("Database error");

        _repositoryMock
            .Setup(x => x.GetByNameAsync(relationshipName, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        Exception exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.GetByNameAsync(relationshipName, CancellationToken.None));

        Assert.Equal("Database error", exception.Message);
    }

    [Fact]
    public async Task GetByNameAsync_WithCancelledToken_ThrowsOperationCanceledException()
    {
        // Arrange
        string relationshipName = "Test";
        CancellationTokenSource cts = new ();
        cts.Cancel();

        _repositoryMock
            .Setup(x => x.GetByNameAsync(relationshipName, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            async () => await _service.GetByNameAsync(relationshipName, cts.Token));
    }

    [Theory]
    [InlineData("Parent-Child")]
    [InlineData("Employer-Employee")]
    [InlineData("Manager-Subordinate")]
    [InlineData("Spouse")]
    public async Task GetByNameAsync_WithVariousNames_ReturnsCorrectEntity(string relationshipName)
    {
        // Arrange
        RelationshipType expectedRelationshipType = new ()
        {
            Id = Guid.NewGuid(),
            Name = relationshipName,
            Description = $"Description for {relationshipName}"
        };

        OperationResult<RelationshipType> operationResult = new ()
        {
            Success = true,
            Entity = expectedRelationshipType,
            ErrorCode = OperationErrorCode.None
        };

        _repositoryMock
            .Setup(x => x.GetByNameAsync(relationshipName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(operationResult);

        // Act
        RelationshipType? result = await _service.GetByNameAsync(relationshipName, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(relationshipName, result.Name);
    }

    [Fact]
    public async Task GetByNameAsync_WhenRepositoryReturnsError_ReturnsNull()
    {
        // Arrange
        string relationshipName = "Test";
        OperationResult<RelationshipType> operationResult = new ()
        {
            Success = false,
            Entity = null,
            ErrorCode = OperationErrorCode.SqlError,
            ErrorMessage = "Database connection failed"
        };

        _repositoryMock
            .Setup(x => x.GetByNameAsync(relationshipName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(operationResult);

        // Act
        RelationshipType? result = await _service.GetByNameAsync(relationshipName, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
