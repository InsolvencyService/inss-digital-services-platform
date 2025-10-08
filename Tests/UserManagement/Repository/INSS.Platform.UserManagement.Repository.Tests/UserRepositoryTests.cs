using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.UserManagement.Core.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace INSS.Platform.UserManagement.Repository.Tests
{
    public class UserRepositoryTests
    {
        private readonly Mock<ILogger<UserRepository>> _loggerMock = new();

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            User newUser = TestHelper.GenerateUsers().Single();
            using UserManagementDbContext dbContext = new(_options);
            dbContext.User.Add(newUser);
            dbContext.SaveChanges();

            UserRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            User? foundUser = await repository.GetUserByIdAsync(newUser.Id);

            // Assert
            using (new AssertionScope())
            {
                foundUser.Should().NotBeNull();
                foundUser.Id.Should().Be(newUser.Id);
                foundUser.FirstName.Should().Be(newUser.FirstName);
                foundUser.LastName.Should().Be(newUser.LastName);
                foundUser.Email.Should().Be(newUser.Email);
                foundUser.Created.Should().BeCloseTo(newUser.Created ?? new(), TimeSpan.FromSeconds(5));
                foundUser.CreatedBy.Should().Be(newUser.CreatedBy);
                foundUser.Modified.Should().BeCloseTo(newUser.Modified ?? new(), TimeSpan.FromSeconds(5));
                foundUser.ModifiedBy.Should().Be(newUser.ModifiedBy);
                foundUser.UserIdentityId.Should().Be(newUser.UserIdentityId);
            }
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsNull_WhenUserNotExist()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            UserRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            User? notFoundUser = await repository.GetUserByIdAsync(Guid.NewGuid());

            // Assert
            notFoundUser.Should().BeNull();
        }

        [Fact]
        public async Task GetUserByIdentityProviderUserIdAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            IdentityProvider identityProvider = GenerateIdentityProvider();
            UserIdentity userIdentity = GenerateUserIdentity(identityProvider.Id);
            User newUser = TestHelper.GenerateUsers().Single();
            newUser.UserIdentityId = userIdentity.Id;

            using UserManagementDbContext dbContext = new(_options);
            dbContext.IdentityProvider.Add(identityProvider);
            dbContext.UserIdentity.Add(userIdentity);
            dbContext.User.Add(newUser);
            dbContext.SaveChanges();

            UserRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            User? foundUser = await repository.GetUserByIdentityProviderUserIdAsync(userIdentity.IdentityProviderUserId, identityProvider.Id);

            // Assert
            using (new AssertionScope())
            {
                foundUser.Should().NotBeNull();
                foundUser.Id.Should().Be(newUser.Id);
                foundUser.FirstName.Should().Be(newUser.FirstName);
                foundUser.LastName.Should().Be(newUser.LastName);
                foundUser.Email.Should().Be(newUser.Email);
                foundUser.Created.Should().BeCloseTo(newUser.Created ?? new(), TimeSpan.FromSeconds(5));
                foundUser.CreatedBy.Should().Be(newUser.CreatedBy);
                foundUser.Modified.Should().BeCloseTo(newUser.Modified ?? new(), TimeSpan.FromSeconds(5));
                foundUser.ModifiedBy.Should().Be(newUser.ModifiedBy);
                foundUser.UserIdentityId.Should().Be(newUser.UserIdentityId);
            }
        }

        [Fact]
        public async Task GetUserByIdentityProviderUserIdAsync_ReturnsNull_WhenUserNotExist()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            IdentityProvider identityProvider = GenerateIdentityProvider();

            using UserManagementDbContext dbContext = new(_options);
            UserRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            User? notFoundUser = await repository.GetUserByIdentityProviderUserIdAsync("unknown-one-login-id", identityProvider.Id);

            // Assert
            notFoundUser.Should().BeNull();
        }

        [Fact]
        public async Task GetUsersByOrganisationAsync_ReturnsUsers_WhenUsersExist()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Organisation organisation = TestHelper.GenerateOrganisations().First();

            List<User> users = [.. TestHelper.GenerateUsers(2)];

            OrganisationUser orgUser1 = GenerateOrganisationUser(organisation.Id, users[0].Id);
            OrganisationUser orgUser2 = GenerateOrganisationUser(organisation.Id, users[1].Id);

            using UserManagementDbContext dbContext = new(_options);
            dbContext.Organisation.Add(organisation);
            dbContext.User.AddRange(users);
            dbContext.OrganisationUser.AddRange(orgUser1, orgUser2);
            dbContext.SaveChanges();

            UserRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            IEnumerable<User> organisationUsers = await repository.GetUsersByOrganisationAsync(organisation.Id);

            // Assert
            using (new AssertionScope())
            {
                organisationUsers.Should().NotBeNull();
                organisationUsers.Should().HaveCount(2);
                organisationUsers.Should().Contain(u => u.Id == users[0].Id);
                organisationUsers.Should().Contain(u => u.Id == users[1].Id);
            }
        }

        private static OrganisationUser GenerateOrganisationUser(Guid organisationId, Guid userId)
        {
            return new OrganisationUser
            {
                OrganisationId = organisationId,
                UserId = userId,
                Created = DateTime.UtcNow,
                CreatedBy = "UnitTestOrganisationUser",
                Modified = DateTime.UtcNow,
                ModifiedBy = "UnitTestOrganisationUser"
            };
        }

        [Fact]
        public async Task AddUserAsync_ReturnsTrue_WhenUserIsAdded()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            UserRepository repository = new(_loggerMock.Object, dbContext);
            User newUser = TestHelper.GenerateUsers().Single();

            // Act
            bool result = await repository.AddUserAsync(newUser);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AddUserAsync_ReturnsFalse_WhenSqlExceptionThrown()
        {
            // Arrange
            Mock<DbSet<User>> dbSetMock = new();
            SqlException dbException = TestHelper.CreateSqlException("Sql Error");

            Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
            dbContextMock.Setup(x => x.User).Returns(dbSetMock.Object);
            dbContextMock.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(dbException);

            UserRepository repository = new(_loggerMock.Object, dbContextMock.Object);
            User newUser = TestHelper.GenerateUsers().Single();

            // Act
            bool result = await repository.AddUserAsync(newUser);

            // Assert
            result.Should().BeFalse();

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"Error adding user with email {newUser.Email}")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ReturnsTrue_WhenUserIsUpdated()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            UserRepository repository = new(_loggerMock.Object, dbContext);

            User newUser = TestHelper.GenerateUsers().Single();
            await repository.AddUserAsync(newUser);

            User userToUpdate = await repository.GetUserByIdAsync(newUser.Id) ?? new();
            userToUpdate.FirstName = "UpdatedFirstName";
            userToUpdate.LastName = "UpdatedLastName";
            userToUpdate.Email = "updated@example.com";
            userToUpdate.Modified = DateTime.UtcNow;
            userToUpdate.ModifiedBy = "UpdatedUser";

            // Act
            bool result = await repository.UpdateUserAsync(userToUpdate);
            User updatedUser = await repository.GetUserByIdAsync(userToUpdate.Id) ?? new();

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeTrue();
                updatedUser.Id.Should().Be(userToUpdate.Id);
                updatedUser.FirstName.Should().Be("UpdatedFirstName");
                updatedUser.LastName.Should().Be("UpdatedLastName");
                updatedUser.Email.Should().Be("updated@example.com");
                updatedUser.Created.Should().Be(userToUpdate.Created);
                updatedUser.CreatedBy.Should().Be(userToUpdate.CreatedBy);
                updatedUser.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                updatedUser.ModifiedBy.Should().Be("UpdatedUser");
                updatedUser.UserIdentityId.Should().Be(userToUpdate.UserIdentityId);
            }
        }

        [Fact]
        public async Task UpdateUserAsync_ReturnsFalse_WhenSqlExceptionThrown()
        {
            // Arrange
            Mock<DbSet<User>> dbSetMock = new();
            SqlException dbException = TestHelper.CreateSqlException("Sql Error");

            Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
            dbContextMock.Setup(x => x.User).Returns(dbSetMock.Object);
            dbContextMock.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(dbException);

            UserRepository repository = new(_loggerMock.Object, dbContextMock.Object);
            User userToUpdate = TestHelper.GenerateUsers().Single();
            userToUpdate.FirstName = "UpdatedFirstName";

            // Act
            bool result = await repository.UpdateUserAsync(userToUpdate);

            // Assert
            result.Should().BeFalse();

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"Error updating user with ID {userToUpdate.Id}")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ReturnsTrue_WhenUserIsDeleted()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            UserRepository repository = new(_loggerMock.Object, dbContext);

            User userToDelete = TestHelper.GenerateUsers().Single();
            await repository.AddUserAsync(userToDelete);

            // Act
            bool result = await repository.DeleteUserAsync(userToDelete);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteUserAsync_ReturnsFalse_WhenSqlExceptionThrown()
        {
            // Arrange
            Mock<DbSet<User>> dbSetMock = new();
            SqlException dbException = TestHelper.CreateSqlException("Sql Error");

            Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
            dbContextMock.Setup(x => x.User).Returns(dbSetMock.Object);
            dbContextMock.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(dbException);

            UserRepository repository = new(_loggerMock.Object, dbContextMock.Object);
            User userToDelete = TestHelper.GenerateUsers().Single();

            // Act
            bool result = await repository.DeleteUserAsync(userToDelete);

            // Assert
            result.Should().BeFalse();

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"Error deleting user with ID {userToDelete.Id}")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }


        private static IdentityProvider GenerateIdentityProvider()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Name = "TestProvider",
                IssuerUrl = "https://testprovider.com",
                ClientId = "test-client-id",
                Secret = "test-secret",
                Created = DateTime.UtcNow,
                CreatedBy = "UnitTestUser",
                Modified = DateTime.UtcNow,
                ModifiedBy = "UnitTestUser"
            };
        }

        private static UserIdentity GenerateUserIdentity(Guid identityProviderId)
        {
            return new UserIdentity
            {
                Id = Guid.NewGuid(),
                IdentityProviderUserId = "one-login-user-id",
                IdentityProviderId = identityProviderId,
                Created = DateTime.UtcNow,
                CreatedBy = "UnitTestUser",
                Modified = DateTime.UtcNow,
                ModifiedBy = "UnitTestUser"
            };
        }
    }
}
