using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.Canonical.Application.Results;
using INSS.Platform.Canonical.Domain;
using INSS.Platform.Canonical.Infrastructure.Repositories;
using INSS.Platform.Canonical.Infrastructure.Tests.Helpers;
using Microsoft.Data.SqlClient;
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

    private static RepositoryBase<User> CreateRepository(CanonicalDbContext dbContext, Mock<ILogger<RepositoryBase<User>>>? loggerMock = null)
    {
        loggerMock ??= new Mock<ILogger<RepositoryBase<User>>>();

        return new TestUserRepository(loggerMock.Object, dbContext);
    }
}
