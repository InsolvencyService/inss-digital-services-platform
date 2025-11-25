using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Infrastructure.Repositories;
using INSS.Platform.UserManagement.Infrastructure.Tests.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace INSS.Platform.UserManagement.Infrastructure.Tests;

/// <summary>
/// Unit tests for the <see cref="RepositoryBase{TEntity}"/> class.
/// </summary>
public class RepositoryBaseTests
{
    private readonly Mock<ILogger<RepositoryBase<TestEntity>>> _loggerMock = new();

    private static TestUserManagementDbContext CreateDbContext()
    {
        DbContextOptions<UserManagementDbContext> options = new DbContextOptionsBuilder<UserManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new TestUserManagementDbContext(options);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenEntityExists()
    {
        using UserManagementDbContext dbContext = CreateDbContext();
        TestEntity entity = new() { Id = Guid.NewGuid(), Name = "Test" };
        dbContext.Set<TestEntity>().Add(entity);
        dbContext.SaveChanges();

        TestRepository repository = new(_loggerMock.Object, dbContext);

        OperationResult<TestEntity> result = await repository.GetByIdAsync(entity.Id, TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Entity.Should().NotBeNull();
            result.Entity.Id.Should().Be(entity.Id);
            result.Entity.Name.Should().Be(entity.Name);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNotFound_WhenEntityDoesNotExist()
    {
        using UserManagementDbContext dbContext = CreateDbContext();
        TestRepository repository = new(_loggerMock.Object, dbContext);
        Guid entityId = Guid.NewGuid();

        OperationResult<TestEntity> result = await repository.GetByIdAsync(entityId, TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            result.ErrorMessage.Should().Be($"Entity of type TestEntity with ID {entityId} not found.");
            result.Entity.Should().BeNull();
        }

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"Entity of type TestEntity with ID {entityId} not found")),
                It.IsAny<SqlException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsSqlException_WhenQueryFails()
    {
        // Arrange
        Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
        dbContextMock.Setup(db => db.Set<TestEntity>()).Throws(TestHelper.CreateSqlException("Sql Error"));
        TestRepository repository = new(_loggerMock.Object, dbContextMock.Object);
        Guid entityId = Guid.NewGuid();

        // Act
        OperationResult<TestEntity> result = await repository.GetByIdAsync(entityId, TestContext.Current.CancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Be($"A database error occurred. Error details: Sql Error While retrieving entity of type TestEntity. Entity ID: {entityId}.");
        }

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains("Sql Error")),
                It.IsAny<SqlException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAsync_ReturnsAllEntities()
    {
        using UserManagementDbContext dbContext = CreateDbContext();
        IEnumerable<TestEntity> entities = TestHelper.GenerateTestEntities(2);
        dbContext.Set<TestEntity>().AddRange(entities);
        dbContext.SaveChanges();

        TestRepository repository = new(_loggerMock.Object, dbContext);

        OperationResult<IEnumerable<TestEntity>> result = await repository.GetAsync(TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Entity.Should().NotBeNull();
            result.Entity.Should().HaveCount(2);
        }
    }

    [Fact]
    public async Task GetAsync_ThrowsSqlException_WhenQueryFails()
    {
        // Arrange
        Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
        dbContextMock.Setup(db => db.Set<TestEntity>()).Throws(TestHelper.CreateSqlException("Sql Error"));
        TestRepository repository = new(_loggerMock.Object, dbContextMock.Object);

        // Act
        OperationResult<IEnumerable<TestEntity>> result = await repository.GetAsync(TestContext.Current.CancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain("A database error occurred. Error details: Sql Error While retrieving entity of type TestEntity.");
        }

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains("Sql Error")),
                It.IsAny<SqlException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task AddAsync_AddsEntitySuccessfully()
    {
        using UserManagementDbContext dbContext = CreateDbContext();
        TestRepository repository = new(_loggerMock.Object, dbContext);
        TestEntity entity = TestHelper.GenerateTestEntity();

        OperationResult<TestEntity> result = await repository.AddAsync(entity, TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Success.Should().BeTrue();
            result.Entity.Should().NotBeNull();
            result.Entity.Id.Should().Be(entity.Id);
            dbContext.Set<TestEntity>().Should().ContainEquivalentOf(entity);
        }
    }

    [Fact]
    public async Task AddAsync_ThrowsSqlException_WhenAddFails()
    {
        // Arrange
        Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
        dbContextMock.Setup(db => db.Set<TestEntity>()).Throws(TestHelper.CreateSqlException("Sql Error"));
        TestRepository repository = new(_loggerMock.Object, dbContextMock.Object);
        TestEntity entity = TestHelper.GenerateTestEntity();

        // Act
        OperationResult<TestEntity> result = await repository.AddAsync(entity, TestContext.Current.CancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain("A database error occurred. Error details: Sql Error While adding entity of type TestEntity.");
        }

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains("Sql Error")),
                It.IsAny<SqlException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task AddAsync_ThrowsDbUpdateException_WhenAddFails()
    {
        // Arrange
        Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
        dbContextMock.Setup(db => db.Set<TestEntity>()).Throws(new DbUpdateException("Insert Error"));
        TestRepository repository = new(_loggerMock.Object, dbContextMock.Object);
        TestEntity entity = TestHelper.GenerateTestEntity();

        // Act
        OperationResult<TestEntity> result = await repository.AddAsync(entity, TestContext.Current.CancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain($"A database update error occurred. Error details: Insert Error While adding entity of type TestEntity. Entity ID: {entity.Id}.");
        }

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains("Insert Error")),
                It.IsAny<DbUpdateException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesEntitySuccessfully()
    {
        using UserManagementDbContext dbContext = CreateDbContext();
        TestEntity entity = TestHelper.GenerateTestEntity(1, "Before");
        dbContext.Set<TestEntity>().Add(entity);
        dbContext.SaveChanges();

        TestRepository repository = new(_loggerMock.Object, dbContext);
        entity.Name = "After";

        OperationResult<TestEntity> result = await repository.UpdateAsync(entity, TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Success.Should().BeTrue();
            result.Entity.Should().NotBeNull();
            result.Entity.Name.Should().Be("After");
        }
    }

    [Fact]
    public async Task UpdateAsync_ThrowsSqlException_WhenUpdateFails()
    {
        // Arrange
        Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
        dbContextMock.Setup(db => db.Set<TestEntity>()).Throws(TestHelper.CreateSqlException("Sql Error"));
        TestRepository repository = new(_loggerMock.Object, dbContextMock.Object);
        TestEntity entity = TestHelper.GenerateTestEntity();

        // Act
        OperationResult<TestEntity> result = await repository.UpdateAsync(entity, TestContext.Current.CancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain("A database error occurred. Error details: Sql Error While updating entity of type TestEntity.");
        }

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains("Sql Error")),
                It.IsAny<SqlException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsDbUpdateException_WhenUpdateFails()
    {
        // Arrange
        Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
        dbContextMock.Setup(db => db.Set<TestEntity>()).Throws(new DbUpdateException("Update Error"));
        TestRepository repository = new(_loggerMock.Object, dbContextMock.Object);
        TestEntity entity = TestHelper.GenerateTestEntity();

        // Act
        OperationResult<TestEntity> result = await repository.UpdateAsync(entity, TestContext.Current.CancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain($"A database update error occurred. Error details: Update Error While updating entity of type TestEntity. Entity ID: {entity.Id}.");
        }

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains("Update Error")),
                It.IsAny<DbUpdateException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DeletesEntitySuccessfully()
    {
        using UserManagementDbContext dbContext = CreateDbContext();
        TestEntity entity = TestHelper.GenerateTestEntity();
        dbContext.Set<TestEntity>().Add(entity);
        dbContext.SaveChanges();

        TestRepository repository = new(_loggerMock.Object, dbContext);

        OperationResult<TestEntity> result = await repository.DeleteAsync(entity.Id, TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Success.Should().BeTrue();
            result.Entity.Should().NotBeNull();
            dbContext.Set<TestEntity>().Any(e => e.Id == entity.Id).Should().BeFalse();
        }
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNotFound_WhenEntityDoesNotExist()
    {
        using UserManagementDbContext dbContext = CreateDbContext();
        TestRepository repository = new(_loggerMock.Object, dbContext);
        Guid entityId = Guid.NewGuid();

        OperationResult<TestEntity> result = await repository.DeleteAsync(entityId, TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            result.Entity.Should().BeNull();
        }
    }

    [Fact]
    public async Task DeleteAsync_ThrowsSqlException_WhenDeleteFails()
    {
        // Arrange
        Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
        dbContextMock.Setup(db => db.Set<TestEntity>()).Throws(TestHelper.CreateSqlException("Sql Error"));
        TestRepository repository = new(_loggerMock.Object, dbContextMock.Object);
        Guid entityId = Guid.NewGuid();

        // Act
        OperationResult<TestEntity> result = await repository.DeleteAsync(entityId, TestContext.Current.CancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain("A database error occurred. Error details: Sql Error While retrieving entity of type TestEntity.");
        }

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains("Sql Error")),
                It.IsAny<SqlException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsDbUpdateException_WhenDeleteFails()
    {
        // Arrange
        Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
        dbContextMock.Setup(db => db.Set<TestEntity>()).Throws(new DbUpdateException("Delete Error"));
        TestRepository repository = new(_loggerMock.Object, dbContextMock.Object);
        Guid entityId = Guid.NewGuid();

        // Act
        OperationResult<TestEntity> result = await repository.DeleteAsync(entityId, TestContext.Current.CancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain($"A database update error occurred. Error details: Delete Error While deleting entity of type TestEntity. Entity ID: {entityId}.");
        }

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains("Delete Error")),
                It.IsAny<DbUpdateException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}