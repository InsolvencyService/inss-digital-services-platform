using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.UserManagement.Core.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace INSS.Platform.UserManagement.Repository.Tests
{
    public class OrganisationRepositoryTests
    {
        private readonly Mock<ILogger<OrganisationRepository>> _loggerMock = new();

        [Fact]
        public async Task GetOrganisationByIdAsync_ReturnsOrganisation_WhenOrganisationExists()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Organisation newOrganisation = TestHelper.GenerateOrganisations().Single();
            using UserManagementDbContext dbContext = new(_options);
            dbContext.Organisation.Add(newOrganisation);
            dbContext.SaveChanges();

            OrganisationRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            Organisation? foundOrganisation = await repository.GetOrganisationByIdAsync(newOrganisation.Id);

            // Assert
            using (new AssertionScope())
            {
                foundOrganisation.Should().NotBeNull();
                foundOrganisation.Id.Should().Be(newOrganisation.Id);
                foundOrganisation.Name.Should().Be(newOrganisation.Name);
                foundOrganisation.Created.Should().BeCloseTo(newOrganisation.Created ?? new(), TimeSpan.FromSeconds(5));
                foundOrganisation.CreatedBy.Should().Be(newOrganisation.CreatedBy);
                foundOrganisation.Modified.Should().BeCloseTo(newOrganisation.Modified ?? new(), TimeSpan.FromSeconds(5));
                foundOrganisation.ModifiedBy.Should().Be(newOrganisation.ModifiedBy);
            }
        }

        [Fact]
        public async Task GetOrganisationByIdAsync_ReturnsNull_WhenOrganisationNotExist()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            OrganisationRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            Organisation? notFoundOrganisation = await repository.GetOrganisationByIdAsync(Guid.NewGuid());

            // Assert
            notFoundOrganisation.Should().BeNull();
        }

        [Fact]
        public async Task GetOrganisationsByUserIdAsync_ReturnsOrganisations_WhenOrganisationsExist()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Guid userId = Guid.NewGuid();
            List<Organisation> organisations = [.. TestHelper.GenerateOrganisations(3)];

            List<OrganisationUser> organisationUsers = organisations.Select(o => new OrganisationUser
            {
                Id = Guid.NewGuid(),
                OrganisationId = o.Id,
                UserId = userId,
                Created = DateTime.UtcNow,
                CreatedBy = "TestUser"
            }).ToList();

            using UserManagementDbContext dbContext = new(_options);
            dbContext.Organisation.AddRange(organisations);
            dbContext.OrganisationUser.AddRange(organisationUsers);
            dbContext.SaveChanges();
            OrganisationRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            IEnumerable<Organisation> foundOrganisations = await repository.GetOrganisationsByUserIdAsync(userId);

            // Assert
            foundOrganisations.Should().NotBeNull();
            foundOrganisations.Count().Should().Be(3);
        }

        [Fact]
        public async Task GetOrganisationsByUserIdAsync_ReturnsEmptyList_WhenNoOrganisations()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            OrganisationRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            IEnumerable<Organisation> noOrganisations = await repository.GetOrganisationsByUserIdAsync(Guid.NewGuid());

            // Assert
            noOrganisations.Should().NotBeNull();
            noOrganisations.Count().Should().Be(0);
        }

        [Fact]
        public async Task GetOrganisationsAsync_ReturnsAllOrganisations()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            List<Organisation> organisations = [.. TestHelper.GenerateOrganisations(5)];
            using UserManagementDbContext dbContext = new(_options);
            dbContext.Organisation.AddRange(organisations);
            dbContext.SaveChanges();
            OrganisationRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            IEnumerable<Organisation> allOrganisations = await repository.GetOrganisationsAsync();

            // Assert
            allOrganisations.Should().NotBeNull();
            allOrganisations.Count().Should().Be(5);
        }

        [Fact]
        public async Task GetOrganisationUserAsync_ReturnsOrganisationUser_WhenOrganisationUserExists()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Guid userId = Guid.NewGuid();
            Guid organisationId = Guid.NewGuid();
            OrganisationUser organisationUser = TestHelper.GenerateOrganisationUser(organisationId, userId);

            using UserManagementDbContext dbContext = new(_options);
            dbContext.OrganisationUser.Add(organisationUser);
            dbContext.SaveChanges();

            OrganisationRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            OrganisationUser? foundOrganisationUser = await repository.GetOrganisationUserAsync(organisationId, userId);

            // Assert
            using (new AssertionScope())
            {
                foundOrganisationUser.Should().NotBeNull();
                foundOrganisationUser.Id.Should().Be(organisationUser.Id);
                foundOrganisationUser.OrganisationId.Should().Be(organisationUser.OrganisationId);
                foundOrganisationUser.UserId.Should().Be(organisationUser.UserId);
                foundOrganisationUser.Created.Should().BeCloseTo(organisationUser.Created ?? new(), TimeSpan.FromSeconds(5));
                foundOrganisationUser.CreatedBy.Should().Be(organisationUser.CreatedBy);
                foundOrganisationUser.Modified.Should().BeCloseTo(organisationUser.Modified ?? new(), TimeSpan.FromSeconds(5));
                foundOrganisationUser.ModifiedBy.Should().Be(organisationUser.ModifiedBy);
            }
        }

        [Fact]
        public async Task GetOrganisationUserAsync_ReturnsNull_WhenOrganisationUserNotExist()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            OrganisationRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            OrganisationUser? notFoundOrganisationUser = await repository.GetOrganisationUserAsync(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            notFoundOrganisationUser.Should().BeNull();
        }

        [Fact]
        public async Task AddOrganisationAsync_ReturnsTrue_WhenOrganisationIsAdded()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            OrganisationRepository repository = new(_loggerMock.Object, dbContext);
            Organisation newOrganisation = TestHelper.GenerateOrganisations().Single();

            // Act
            bool result = await repository.AddOrganisationAsync(newOrganisation);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AddOrganisationUserAsync_ReturnsTrue_WhenOrganisationUserIsAdded()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            OrganisationRepository repository = new(_loggerMock.Object, dbContext);
            OrganisationUser newOrganisationUser = TestHelper.GenerateOrganisationUser();

            // Act
            bool result = await repository.AddOrganisationUserAsync(newOrganisationUser);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AddOrganisationAsync_ReturnsFalse_WhenSqlExceptionThrown()
        {
            // Arrange
            Mock<DbSet<Organisation>> dbSetMock = new();
            SqlException dbException = TestHelper.CreateSqlException("Sql Error");

            Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
            dbContextMock.Setup(x => x.Organisation).Returns(dbSetMock.Object);
            dbContextMock.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(dbException);

            OrganisationRepository repository = new(_loggerMock.Object, dbContextMock.Object);
            Organisation newOrganisation = TestHelper.GenerateOrganisations().Single();

            // Act
            bool result = await repository.AddOrganisationAsync(newOrganisation);

            // Assert
            result.Should().BeFalse();

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"Error adding organisation with name {newOrganisation.Name}")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateOrganisationAsync_ReturnsTrue_WhenOrganisationIsUpdated()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            OrganisationRepository repository = new(_loggerMock.Object, dbContext);

            Organisation newOrganisation = TestHelper.GenerateOrganisations().Single();
            await repository.AddOrganisationAsync(newOrganisation);

            Organisation OrganisationToUpdate = await repository.GetOrganisationByIdAsync(newOrganisation.Id) ?? new();
            OrganisationToUpdate.Name = "UpdatedName";
            OrganisationToUpdate.Modified = DateTime.UtcNow;
            OrganisationToUpdate.ModifiedBy = "UpdatedOrganisationUser";

            // Act
            bool result = await repository.UpdateOrganisationAsync(OrganisationToUpdate);
            Organisation updatedOrganisation = await repository.GetOrganisationByIdAsync(OrganisationToUpdate.Id) ?? new();

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeTrue();
                updatedOrganisation.Id.Should().Be(OrganisationToUpdate.Id);
                updatedOrganisation.Name.Should().Be("UpdatedName");
                updatedOrganisation.Created.Should().Be(OrganisationToUpdate.Created);
                updatedOrganisation.CreatedBy.Should().Be(OrganisationToUpdate.CreatedBy);
                updatedOrganisation.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                updatedOrganisation.ModifiedBy.Should().Be("UpdatedOrganisationUser");
            }
        }

        [Fact]
        public async Task UpdateOrganisationAsync_ReturnsFalse_WhenSqlExceptionThrown()
        {
            // Arrange
            Mock<DbSet<Organisation>> dbSetMock = new();
            SqlException dbException = TestHelper.CreateSqlException("Sql Error");

            Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
            dbContextMock.Setup(x => x.Organisation).Returns(dbSetMock.Object);
            dbContextMock.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(dbException);

            OrganisationRepository repository = new(_loggerMock.Object, dbContextMock.Object);
            Organisation OrganisationToUpdate = TestHelper.GenerateOrganisations().Single();
            OrganisationToUpdate.Name = "UpdatedName";

            // Act
            bool result = await repository.UpdateOrganisationAsync(OrganisationToUpdate);

            // Assert
            result.Should().BeFalse();

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"Error updating organisation with ID {OrganisationToUpdate.Id}")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteOrganisationAsync_ReturnsTrue_WhenOrganisationIsDeleted()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            OrganisationRepository repository = new(_loggerMock.Object, dbContext);

            Organisation OrganisationToDelete = TestHelper.GenerateOrganisations().Single();
            await repository.AddOrganisationAsync(OrganisationToDelete);

            // Act
            bool result = await repository.DeleteOrganisationAsync(OrganisationToDelete);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteOrganisationAsync_ReturnsFalse_WhenSqlExceptionThrown()
        {
            // Arrange
            Mock<DbSet<Organisation>> dbSetMock = new();
            SqlException dbException = TestHelper.CreateSqlException("Sql Error");

            Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
            dbContextMock.Setup(x => x.Organisation).Returns(dbSetMock.Object);
            dbContextMock.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(dbException);

            OrganisationRepository repository = new(_loggerMock.Object, dbContextMock.Object);
            Organisation OrganisationToDelete = TestHelper.GenerateOrganisations().Single();

            // Act
            bool result = await repository.DeleteOrganisationAsync(OrganisationToDelete);

            // Assert
            result.Should().BeFalse();

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"Error deleting organisation with ID {OrganisationToDelete.Id}")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task OrganisationUserExistsAsync_ReturnsTrue_WhenOrganisationUserExists()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            User user = TestHelper.GenerateUsers().Single();
            Organisation organisation = TestHelper.GenerateOrganisations().Single();
            OrganisationUser organisationUser = TestHelper.GenerateOrganisationUser(organisation.Id, user.Id);

            using UserManagementDbContext dbContext = new(_options);
            dbContext.User.Add(user);
            dbContext.Organisation.Add(organisation);
            dbContext.OrganisationUser.Add(organisationUser);
            dbContext.SaveChanges();

            OrganisationRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            bool organisationUserExists = await repository.OrganisationUserExistsAsync(organisation.Id, user.Id);

            // Assert
            organisationUserExists.Should().BeTrue();
        }

        [Fact]
        public async Task OrganisationUserExistsAsync_ReturnsFalse_WhenOrganisationUserNotExists()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            OrganisationRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            bool organisationUserExists = await repository.OrganisationUserExistsAsync(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            organisationUserExists.Should().BeFalse();
        }

        [Fact]
        public async Task RemoveOrganisationUserAsync_ReturnsTrue_WhenOrganisationUserIsRemoved()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            User user = TestHelper.GenerateUsers().Single();
            Organisation organisation = TestHelper.GenerateOrganisations().Single();
            OrganisationUser organisationUser = TestHelper.GenerateOrganisationUser(organisation.Id, user.Id);

            using UserManagementDbContext dbContext = new(_options);
            dbContext.User.Add(user);
            dbContext.Organisation.Add(organisation);
            dbContext.OrganisationUser.Add(organisationUser);
            dbContext.SaveChanges();

            OrganisationRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            bool result = await repository.RemoveOrganisationUserAsync(organisation.Id, user.Id);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeTrue();
                dbContext.OrganisationUser.Count(ou => ou.OrganisationId == organisation.Id && ou.UserId == user.Id).Should().Be(0);
            }
        }

        [Fact]
        public async Task RemoveOrganisationUserAsync_ReturnsFalse_OrganisationUserNotFound()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            OrganisationRepository repository = new(_loggerMock.Object, dbContext);

            Guid organisationId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            // Act
            bool result = await repository.RemoveOrganisationUserAsync(organisationId, userId);

            // Assert
            result.Should().BeFalse();
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"Organisation user not found with user ID {userId} organisation ID {organisationId}")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task RemoveAllOrganisationUsersAsync_ReturnsTrue_WhenOrganisationUsersAreRemoved()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Guid organisationId = Guid.NewGuid();
            List<OrganisationUser> organisationUsers = TestHelper.GenerateOrganisationUsers(organisationId, 3).ToList();

            using UserManagementDbContext dbContext = new(_options);
            dbContext.OrganisationUser.AddRange(organisationUsers);
            dbContext.SaveChanges();

            OrganisationRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            bool result = await repository.RemoveAllOrganisationUsersAsync(organisationId);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeTrue();
                dbContext.OrganisationUser.Count(ou => ou.OrganisationId == organisationId).Should().Be(0);
            }
        }

        [Fact]
        public async Task AddOrganisationUserApplicationRoleAsync_ReturnsTrue_WhenOrganisationUserApplicationRoleIsAdded()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            OrganisationRepository repository = new(_loggerMock.Object, dbContext);
            OrganisationUserApplicationRole newOrganisationUserApplicationRole = TestHelper.GenerateOrganisationUserApplicationRole();

            // Act
            bool result = await repository.AddOrganisationUserApplicationRoleAsync(newOrganisationUserApplicationRole);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeTrue();
                dbContext.OrganisationUserApplicationRole.Count(ouar => ouar.OrganisationUserId == newOrganisationUserApplicationRole.OrganisationUserId 
                    && ouar.ApplicationRoleId == newOrganisationUserApplicationRole.ApplicationRoleId)
                    .Should().Be(1);
            }
        }

        [Fact]
        public async Task RemoveOrganisationUserApplicationRoleAsync_ReturnsTrue_WhenOrganisationUserApplicationRoleIsRemoved()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            OrganisationUserApplicationRole organisationUserApplicationRole = TestHelper.GenerateOrganisationUserApplicationRole();
            using UserManagementDbContext dbContext = new(_options);
            dbContext.OrganisationUserApplicationRole.Add(organisationUserApplicationRole);
            dbContext.SaveChanges();

            OrganisationRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            bool result = await repository.RemoveOrganisationUserApplicationRoleAsync(organisationUserApplicationRole.OrganisationUserId, organisationUserApplicationRole.ApplicationRoleId);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeTrue();
                dbContext.OrganisationUserApplicationRole.Count(ouar => ouar.OrganisationUserId == organisationUserApplicationRole.OrganisationUserId 
                && ouar.ApplicationRoleId == organisationUserApplicationRole.ApplicationRoleId).Should().Be(0);
            }
        }
    }
}
