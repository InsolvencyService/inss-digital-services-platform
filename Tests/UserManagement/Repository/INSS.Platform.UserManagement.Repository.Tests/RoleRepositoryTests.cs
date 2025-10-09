using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.UserManagement.Core.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace INSS.Platform.UserManagement.Repository.Tests
{
    public class RoleRepositoryTests
    {
        private readonly Mock<ILogger<RoleRepository>> _loggerMock = new();

        [Fact]
        public async Task GetRoleByIdAsync_ReturnsRole_WhenRoleExists()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Role newRole = TestHelper.GenerateRoles().Single();
            using UserManagementDbContext dbContext = new(_options);
            dbContext.Role.Add(newRole);
            dbContext.SaveChanges();

            RoleRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            Role? foundRole = await repository.GetRoleByIdAsync(newRole.Id);

            // Assert
            using (new AssertionScope())
            {
                foundRole.Should().NotBeNull();
                foundRole.Id.Should().Be(newRole.Id);
                foundRole.Name.Should().Be(newRole.Name);
                foundRole.Description.Should().Be(newRole.Description);
                foundRole.Created.Should().BeCloseTo(newRole.Created ?? new(), TimeSpan.FromSeconds(5));
                foundRole.CreatedBy.Should().Be(newRole.CreatedBy);
                foundRole.Modified.Should().BeCloseTo(newRole.Modified ?? new(), TimeSpan.FromSeconds(5));
                foundRole.ModifiedBy.Should().Be(newRole.ModifiedBy);
            }
        }

        [Fact]
        public async Task GetRoleByIdAsync_ReturnsNull_WhenRoleNotExist()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            RoleRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            Role? notFoundRole = await repository.GetRoleByIdAsync(Guid.NewGuid());

            // Assert
            notFoundRole.Should().BeNull();
        }

        [Fact]
        public async Task GetRolesByOrganisationUserApplicationAsync_ReturnsEmptyList_WhenNoRoles()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using UserManagementDbContext dbContext = new(_options);
            RoleRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            IEnumerable<Role> noRoles = await repository.GetRolesByOrganisationUserApplicationAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            // Assert
            noRoles.Should().NotBeNull();
            noRoles.Count().Should().Be(0);
        }

        [Fact]
        public async Task GetRolesByOrganisationUserApplicationAsync_ReturnsRoles_WhenRolesExist()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            List<Application> applications = TestHelper.GenerateApplications(3).ToList();
            List<Role> roles = TestHelper.GenerateRoles(3).ToList();
            List<ApplicationRole> applicationRoles = [];
            for (int index = 0; index < 3; index++)
            {
                applicationRoles.Add(TestHelper.GenerateApplicationRole(applications[index].Id, roles[index].Id));
            }

            List<Organisation> organisations = TestHelper.GenerateOrganisations(3).ToList();
            List<User> users = TestHelper.GenerateUsers(3).ToList();
            List<OrganisationUser> organisationUsers = [];
            for (int index = 0; index < 3; index++)
            {
                organisationUsers.Add(TestHelper.GenerateOrganisationUser(users[index].Id, organisations[index].Id));
            }

            List<OrganisationUserApplicationRole> organisationUserApplicationRole = [];
            for (int index = 0; index < 3; index++)
            {
                organisationUserApplicationRole.Add(TestHelper.GenerateOrganisationUserApplicationRole(organisationUsers[index].Id, applicationRoles[index].Id));
            }

            using UserManagementDbContext dbContext = new(_options);
            dbContext.Application.AddRange(applications);
            dbContext.Role.AddRange(roles);
            dbContext.ApplicationRole.AddRange(applicationRoles);
            dbContext.Organisation.AddRange(organisations);
            dbContext.User.AddRange(users);
            dbContext.OrganisationUser.AddRange(organisationUsers);
            dbContext.OrganisationUserApplicationRole.AddRange(organisationUserApplicationRole);
            dbContext.SaveChanges();

            RoleRepository repository = new(_loggerMock.Object, dbContext);

            // Act & Assert
            using (new AssertionScope())
            {
                for (int index = 0; index < organisationUsers.Count; index++)
                {
                    OrganisationUser ou = organisationUsers[index];
                    Application app = applications[index];
                    IEnumerable<Role> foundRoles = await repository.GetRolesByOrganisationUserApplicationAsync(ou.OrganisationId, ou.UserId, app.Id);

                    // Assert
                    foundRoles.Should().NotBeNull();
                    foundRoles.Count().Should().Be(1);
                    foundRoles.First().Should().Be(roles[index]);
                    foundRoles.First().Name.Should().Be($"TestRole{index}");
                }
            }
        }

        [Fact]
        public async Task GetRolesByApplicationAsync_ReturnsEmptyList_WhenNoRoles()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            RoleRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            IEnumerable<Role> noRoles = await repository.GetRolesByApplicationAsync(Guid.NewGuid());

            // Assert
            using(new AssertionScope())
            {
                noRoles.Should().NotBeNull();
                noRoles.Count().Should().Be(0);
            }
        }

        [Fact]
        public async Task GetRolesByApplicationAsync_ReturnsRoles_WhenRolesExist()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            List<Application> applications = TestHelper.GenerateApplications(3).ToList();
            List<Role> roles = TestHelper.GenerateRoles(3).ToList();
            List<ApplicationRole> applicationRoles = [];

            for (int index = 0; index < 3; index++)
            {
                applicationRoles.Add(TestHelper.GenerateApplicationRole(applications[index].Id, roles[index].Id));
            }

            using UserManagementDbContext dbContext = new(_options);
            dbContext.Application.AddRange(applications);
            dbContext.Role.AddRange(roles);
            dbContext.ApplicationRole.AddRange(applicationRoles);
            dbContext.SaveChanges();

            RoleRepository repository = new(_loggerMock.Object, dbContext);

            // Act & Assert
            using (new AssertionScope())
            {
                for (int index = 0; index < applications.Count; index++)
                {
                    Application app = applications[index];
                    IEnumerable<Role> foundRoles = await repository.GetRolesByApplicationAsync(app.Id);

                    // Assert
                    foundRoles.Should().NotBeNull();
                    foundRoles.Count().Should().Be(1);
                    foundRoles.First().Should().Be(roles[index]);
                    foundRoles.First().Name.Should().Be($"TestRole{index}");
                }
            }
        }

        [Fact]
        public async Task AddRoleAsync_ReturnsTrue_WhenRoleIsAdded()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            RoleRepository repository = new(_loggerMock.Object, dbContext);
            Role newRole = TestHelper.GenerateRoles().Single();

            // Act
            bool result = await repository.AddRoleAsync(newRole);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AddRoleAsync_ReturnsFalse_WhenSqlExceptionThrown()
        {
            // Arrange
            Mock<DbSet<Role>> dbSetMock = new();
            SqlException dbException = TestHelper.CreateSqlException("Sql Error");

            Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
            dbContextMock.Setup(x => x.Role).Returns(dbSetMock.Object);
            dbContextMock.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(dbException);

            RoleRepository repository = new(_loggerMock.Object, dbContextMock.Object);
            Role newRole = TestHelper.GenerateRoles().Single();

            // Act
            bool result = await repository.AddRoleAsync(newRole);

            // Assert
            result.Should().BeFalse();

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"Error adding role with name {newRole.Name}")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateRoleAsync_ReturnsTrue_WhenRoleIsUpdated()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            RoleRepository repository = new(_loggerMock.Object, dbContext);

            Role newRole = TestHelper.GenerateRoles().Single();
            await repository.AddRoleAsync(newRole);

            Role roleToUpdate = await repository.GetRoleByIdAsync(newRole.Id) ?? new();
            roleToUpdate.Name = "UpdatedName";
            roleToUpdate.Description = "UpdatedDescription";
            roleToUpdate.Modified = DateTime.UtcNow;
            roleToUpdate.ModifiedBy = "UpdatedRoleUser";

            // Act
            bool result = await repository.UpdateRoleAsync(roleToUpdate);
            Role updatedRole = await repository.GetRoleByIdAsync(roleToUpdate.Id) ?? new();

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeTrue();
                updatedRole.Id.Should().Be(roleToUpdate.Id);
                updatedRole.Name.Should().Be("UpdatedName");
                updatedRole.Description.Should().Be("UpdatedDescription");
                updatedRole.Created.Should().Be(roleToUpdate.Created);
                updatedRole.CreatedBy.Should().Be(roleToUpdate.CreatedBy);
                updatedRole.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                updatedRole.ModifiedBy.Should().Be("UpdatedRoleUser");
            }
        }

        [Fact]
        public async Task UpdateRoleAsync_ReturnsFalse_WhenSqlExceptionThrown()
        {
            // Arrange
            Mock<DbSet<Role>> dbSetMock = new();
            SqlException dbException = TestHelper.CreateSqlException("Sql Error");

            Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
            dbContextMock.Setup(x => x.Role).Returns(dbSetMock.Object);
            dbContextMock.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(dbException);

            RoleRepository repository = new(_loggerMock.Object, dbContextMock.Object);
            Role roleToUpdate = TestHelper.GenerateRoles().Single();
            roleToUpdate.Name = "UpdatedName";

            // Act
            bool result = await repository.UpdateRoleAsync(roleToUpdate);

            // Assert
            result.Should().BeFalse();

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"Error updating role with ID {roleToUpdate.Id}")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteRoleAsync_ReturnsTrue_WhenRoleIsDeleted()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            RoleRepository repository = new(_loggerMock.Object, dbContext);

            Role roleToDelete = TestHelper.GenerateRoles().Single();
            await repository.AddRoleAsync(roleToDelete);

            // Act
            bool result = await repository.DeleteRoleAsync(roleToDelete);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteRoleAsync_ReturnsFalse_WhenSqlExceptionThrown()
        {
            // Arrange
            Mock<DbSet<Role>> dbSetMock = new();
            SqlException dbException = TestHelper.CreateSqlException("Sql Error");

            Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
            dbContextMock.Setup(x => x.Role).Returns(dbSetMock.Object);
            dbContextMock.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(dbException);

            RoleRepository repository = new(_loggerMock.Object, dbContextMock.Object);
            Role roleToDelete = TestHelper.GenerateRoles().Single();

            // Act
            bool result = await repository.DeleteRoleAsync(roleToDelete);

            // Assert
            result.Should().BeFalse();

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"Error deleting role with ID {roleToDelete.Id}")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
