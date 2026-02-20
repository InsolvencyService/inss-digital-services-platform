using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.Canonical.Application.Results;
using INSS.Platform.Canonical.Domain;
using INSS.Platform.Canonical.Infrastructure.Repositories;
using INSS.Platform.Canonical.Infrastructure.Tests.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace INSS.Platform.Canonical.Infrastructure.Tests;

public class RepositoryBaseTests
{

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenExists()
    {
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();

        User user = TestHelper.GenerateUser();
        dbContext.User.Add(user);
        dbContext.SaveChanges();

        RepositoryBase<User> repository = CreateRepository(dbContext);

        OperationResult<User> result = await repository.GetByIdAsync(user.Id, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Success.Should().BeTrue();
            result.Entity.Should().NotBeNull();
            result.Entity.Id.Should().Be(user.Id);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNotFound_WhenNotExists()
    {
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();

        RepositoryBase<User> repository = CreateRepository(dbContext);
        Guid id = Guid.NewGuid();

        OperationResult<User> result = await repository.GetByIdAsync(id, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            result.Entity.Should().BeNull();
        }
    }

    [Fact]
    public async Task AddAsync_AddsEntitySuccessfully()
    {
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();

        User user = TestHelper.GenerateUser();
        RepositoryBase<User> repository = CreateRepository(dbContext);

        OperationResult<User> result = await repository.AddAsync(user, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Success.Should().BeTrue();
            result.Entity.Should().NotBeNull();
            dbContext.User.Count().Should().Be(1);
        }
    }

    [Fact]
    public async Task UpdateAsync_UpdatesEntitySuccessfully()
    {
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();

        User user = TestHelper.GenerateUser();
        dbContext.User.Add(user);
        dbContext.SaveChanges();
        
        RepositoryBase<User> repository = CreateRepository(dbContext);
        user.FullName = "Updated Name";

        OperationResult<User> result = await repository.UpdateAsync(user, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Success.Should().BeTrue();
            result.Entity.Should().NotBeNull();
            result.Entity.FullName.Should().Be("Updated Name");
        }
    }

    [Fact]
    public async Task DeleteAsync_DeletesEntitySuccessfully()
    {
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();

        User user = TestHelper.GenerateUser();
        dbContext.User.Add(user);
        dbContext.SaveChanges();
        
        RepositoryBase<User> repository = CreateRepository(dbContext);

        OperationResult<User> result = await repository.DeleteAsync(user.Id, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Success.Should().BeTrue();
            dbContext.User.Count().Should().Be(0);
        }
    }

    [Fact]
    public async Task GetAsync_ReturnsAllEntities()
    {
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();

        IEnumerable<User> users = TestHelper.GenerateUsers(2);
        dbContext.User.AddRange(users);
        dbContext.SaveChanges();

        RepositoryBase<User> repository = CreateRepository(dbContext);

        OperationResult<IEnumerable<User>> result = await repository.GetAsync(CancellationToken.None);

        using (new AssertionScope())
        {
            result.Success.Should().BeTrue();
            result.Entity.Should().NotBeNull();
            result.Entity.Count().Should().Be(2);
        }
    }

    [Fact]
    public void HandlePersistenceException_ReturnsSqlError()
    {
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();

        Mock<ILogger<RepositoryBase<User>>> loggerMock = new();
        RepositoryBase<User> repository = CreateRepository(dbContext, loggerMock);

        SqlException sqlException = TestHelper.CreateSqlException("database error");

        OperationResult<User> result = repository.HandlePersistenceException(sqlException, "adding", Guid.NewGuid());

        using (new AssertionScope())
            {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain("database error");
        }
    }

    [Fact]
    public void HandlePersistenceException_WithForeignKeyViolation_ReturnsAppropriateErrorMessage()
    {
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();

        Mock<ILogger<RepositoryBase<User>>> loggerMock = new ();
        RepositoryBase<User> repository = CreateRepository(dbContext, loggerMock);

        SqlException sqlException = TestHelper.CreateSqlException("Foreign key constraint violation");
        Guid testId = Guid.NewGuid();

        OperationResult<User> result = repository.HandlePersistenceException(sqlException, "deleting", testId);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain("Foreign key constraint violation");
            result.ErrorMessage.Should().Contain("deleting");
            result.ErrorMessage.Should().Contain(testId.ToString());
        }
    }

    [Fact]
    public void HandlePersistenceException_WithUniqueConstraintViolation_ReturnsAppropriateErrorMessage()
    {
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();

        Mock<ILogger<RepositoryBase<User>>> loggerMock = new ();
        RepositoryBase<User> repository = CreateRepository(dbContext, loggerMock);

        SqlException sqlException = TestHelper.CreateSqlException("Unique constraint violation");
        Guid testId = Guid.NewGuid();

        OperationResult<User> result = repository.HandlePersistenceException(sqlException, "adding", testId);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain("Unique constraint violation");
            result.ErrorMessage.Should().Contain("adding");
        }
    }

    [Fact]
    public void HandlePersistenceException_WithUniqueIndexViolation_ReturnsAppropriateErrorMessage()
    {
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();

        Mock<ILogger<RepositoryBase<User>>> loggerMock = new ();
        RepositoryBase<User> repository = CreateRepository(dbContext, loggerMock);

        SqlException sqlException = TestHelper.CreateSqlException("Unique index violation");

        OperationResult<IEnumerable<User>> result = repository.HandlePersistenceException(sqlException, "updating");

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain("Unique index violation");
            result.ErrorMessage.Should().Contain("updating");
        }
    }

    [Fact]
    public void HandlePersistenceException_WithEntityName_IncludesNameInErrorMessage()
    {
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();

        Mock<ILogger<RepositoryBase<User>>> loggerMock = new ();
        RepositoryBase<User> repository = CreateRepository(dbContext, loggerMock);

        SqlException sqlException = TestHelper.CreateSqlException("Database error");
        Guid testId = Guid.NewGuid();
        string entityName = "TestUser";

        OperationResult<User> result = repository.HandlePersistenceException(sqlException, "retrieving", testId, entityName);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain(entityName);
            result.ErrorMessage.Should().Contain(testId.ToString());
        }
    }

    [Fact]
    public void HandlePersistenceException_WithDbUpdateException_ReturnsAppropriateErrorMessage()
    {
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();

        Mock<ILogger<RepositoryBase<User>>> loggerMock = new ();
        RepositoryBase<User> repository = CreateRepository(dbContext, loggerMock);

        DbUpdateException dbUpdateException = new ("Database update failed");

        OperationResult<User> result = repository.HandlePersistenceException(dbUpdateException, "updating", Guid.NewGuid());

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain("A database update error occurred.");
            result.ErrorMessage.Should().Contain("Database update failed");
        }
    }

    [Fact]
    public void HandlePersistenceException_WithDbUpdateExceptionContainingSqlException_ReturnsSqlErrorMessage()
    {
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();

        Mock<ILogger<RepositoryBase<User>>> loggerMock = new ();
        RepositoryBase<User> repository = CreateRepository(dbContext, loggerMock);

        SqlException sqlException = TestHelper.CreateSqlException("Inner SQL error");
        DbUpdateException dbUpdateException = new ("Outer exception", sqlException);

        OperationResult<IEnumerable<User>> result = repository.HandlePersistenceException(dbUpdateException, "adding");

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain("Inner SQL error");
        }
    }

    [Fact]
    public void HandlePersistenceException_ForCollectionOperation_ReturnsCollectionResult()
    {
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();

        Mock<ILogger<RepositoryBase<User>>> loggerMock = new ();
        RepositoryBase<User> repository = CreateRepository(dbContext, loggerMock);

        SqlException sqlException = TestHelper.CreateSqlException("Database connection failed");

        OperationResult<IEnumerable<User>> result = repository.HandlePersistenceException(sqlException, "retrieving");

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain("Database connection failed");
            result.ErrorMessage.Should().Contain("retrieving");
            result.Entity.Should().BeNull();
        }
    }

    [Fact]
    public void HandlePersistenceException_WithNoIdOrName_BuildsBasicErrorMessage()
    {
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();

        Mock<ILogger<RepositoryBase<User>>> loggerMock = new ();
        RepositoryBase<User> repository = CreateRepository(dbContext, loggerMock);

        SqlException sqlException = TestHelper.CreateSqlException("General error");

        OperationResult<IEnumerable<User>> result = repository.HandlePersistenceException(sqlException, "processing");

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain("processing");
            result.ErrorMessage.Should().Contain("entity of type User");
        }
    }

    [Fact]
    public void HandlePersistenceException_WithUnknownException_ReturnsGenericErrorMessage()
    {
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();

        Mock<ILogger<RepositoryBase<User>>> loggerMock = new ();
        RepositoryBase<User> repository = CreateRepository(dbContext, loggerMock);

        Exception genericException = new InvalidOperationException("Unknown error");

        OperationResult<IEnumerable<User>> result = repository.HandlePersistenceException(genericException, "executing");

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain("An unknown database error occurred.");
            result.ErrorMessage.Should().Contain("Unknown error");
        }
    }

    [Fact]
    public void HandlePersistenceException_LogsErrorWithCorrectMessage()
    {
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();

        Mock<ILogger<RepositoryBase<User>>> loggerMock = new ();
        RepositoryBase<User> repository = CreateRepository(dbContext, loggerMock);

        SqlException sqlException = TestHelper.CreateSqlException("Test error");
        Guid testId = Guid.NewGuid();

        OperationResult<User> result = repository.HandlePersistenceException(sqlException, "testing", testId);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("testing")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    private static RepositoryBase<User> CreateRepository(CanonicalDbContext dbContext, Mock<ILogger<RepositoryBase<User>>>? loggerMock = null)
    {
        loggerMock ??= new Mock<ILogger<RepositoryBase<User>>>();

        return new TestUserRepository(loggerMock.Object, dbContext);
    }
}
