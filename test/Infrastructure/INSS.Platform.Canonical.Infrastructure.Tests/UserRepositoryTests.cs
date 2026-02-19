using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.Audit.Application.Events;
using INSS.Platform.Canonical.Application.Results;
using INSS.Platform.Canonical.Domain;
using INSS.Platform.Canonical.Infrastructure.Repositories;
using INSS.Platform.Canonical.Infrastructure.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace INSS.Platform.Canonical.Infrastructure.Tests;

public class UserRepositoryTests
{
    private readonly Mock<ILogger<UserRepository>> _loggerMock = new();
    private readonly Mock<IDomainEventDispatcher> _dispatcherMock = new();

    [Fact]
    public async Task GetByIdAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();
        User user = TestHelper.GenerateUser();
        dbContext.User.Add(user);
        dbContext.SaveChanges();

        UserRepository repository = new (_loggerMock.Object, dbContext, _dispatcherMock.Object);

        // Act
        OperationResult<User> result = await repository.GetByIdAsync(user.Id, CancellationToken.None);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Entity.Should().NotBeNull();
            result.Entity.Id.Should().Be(user.Id);
            result.Entity.FullName.Should().Be(user.FullName);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();
        UserRepository repository = new(_loggerMock.Object, dbContext, _dispatcherMock.Object);
        Guid userId = Guid.NewGuid();

        // Act
        OperationResult<User> result = await repository.GetByIdAsync(userId, CancellationToken.None);

        // Assert
        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            result.ErrorMessage.Should().Be($"Entity of type User with ID {userId} not found.");
            result.Entity.Should().BeNull();
        }
    }

    [Fact]
    public async Task GetByIdAsync_HandlesSqlException()
    {
        // Arrange
        Mock<CanonicalDbContext> dbContextMock = new(new DbContextOptions<CanonicalDbContext>());
        dbContextMock.Setup(db => db.User).Throws(TestHelper.CreateSqlException("database error"));
        UserRepository repository = new(_loggerMock.Object, dbContextMock.Object, _dispatcherMock.Object);
        Guid userId = Guid.NewGuid();

        // Act
        OperationResult<User> result = await repository.GetByIdAsync(userId, CancellationToken.None);

        // Assert
        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain("database error");
        }
    }
}
