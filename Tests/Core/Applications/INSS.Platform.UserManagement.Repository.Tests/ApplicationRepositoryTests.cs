using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.UserManagement.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace INSS.Platform.UserManagement.Repository.Tests
{
    public class ApplicationRepositoryTests
    {
        private readonly Mock<ILogger<ApplicationRepository>> _loggerMock = new();

        [Fact]
        public async Task GetApplicationByIdAsync_ReturnsApplication_WhenApplicationExists()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Application newApplication = TestHelper.GenerateApplications().Single();
            using UserManagementDbContext dbContext = new(_options);
            dbContext.Application.Add(newApplication);
            dbContext.SaveChanges();

            ApplicationRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            Application? foundApplication = await repository.GetApplicationByIdAsync(newApplication.Id);

            // Assert
            using (new AssertionScope())
            {
                foundApplication.Should().NotBeNull();
                foundApplication.Id.Should().Be(newApplication.Id);
                foundApplication.Name.Should().Be(newApplication.Name);
                foundApplication.Created.Should().BeCloseTo(newApplication.Created ?? new(), TimeSpan.FromSeconds(5));
                foundApplication.CreatedBy.Should().Be(newApplication.CreatedBy);
                foundApplication.Modified.Should().BeCloseTo(newApplication.Modified ?? new(), TimeSpan.FromSeconds(5));
                foundApplication.ModifiedBy.Should().Be(newApplication.ModifiedBy);
            }
        }

        [Fact]
        public async Task GetApplicationByIdAsync_ReturnsNull_WhenApplicationNotExist()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            ApplicationRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            Application? notFoundApplication = await repository.GetApplicationByIdAsync(Guid.NewGuid());

            // Assert
            notFoundApplication.Should().BeNull();
        }

        [Fact]
        public async Task GetApplicationsAsync_ReturnsApplications_WhenApplicationsExist()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            List<Application> applications = TestHelper.GenerateApplications(5).ToList();

            using UserManagementDbContext dbContext = new(_options);
            dbContext.Application.AddRange(applications);
            dbContext.SaveChanges();

            ApplicationRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            IEnumerable<Application> allApplications = await repository.GetApplicationsAsync();

            // Assert
            allApplications.Should().NotBeNull();
            allApplications.Count().Should().Be(5);
        }

        [Fact]
        public async Task GetApplicationsAsync_ReturnsEmptyList_WhenNoApplications()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            ApplicationRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            IEnumerable<Application> noApplications = await repository.GetApplicationsAsync();

            // Assert
            noApplications.Should().NotBeNull();
            noApplications.Count().Should().Be(0);
        }

        [Fact]
        public async Task GetApplicationsByOrganisationUserAsync_ReturnsApplications_WhenApplicationsExist()
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

            ApplicationRepository repository = new(_loggerMock.Object, dbContext);

            // Act & Assert
            using (new AssertionScope())
            {
                for (int index = 0; index < organisationUsers.Count; index++)
                {
                    OrganisationUser ou = organisationUsers[index];
                    IEnumerable<Application> foundApplications = await repository.GetApplicationsByOrganisationUserAsync(ou.OrganisationId, ou.UserId);

                    // Assert
                    foundApplications.Should().NotBeNull();
                    foundApplications.Count().Should().Be(1);
                    foundApplications.First().Should().Be(applications[index]);
                    foundApplications.First().Name.Should().Be($"TestApplication{index}");
                }
            }
        }

        [Fact]
        public async Task GetApplicationRoleAsync_ReturnsApplicationRole_WhenApplicationRoleExists()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Role role = TestHelper.GenerateRoles().Single();
            Application application = TestHelper.GenerateApplications().Single();
            ApplicationRole applicationRole = TestHelper.GenerateApplicationRole(application.Id, role.Id);

            using UserManagementDbContext dbContext = new(_options);
            dbContext.Role.Add(role);
            dbContext.Application.Add(application);
            dbContext.ApplicationRole.Add(applicationRole);
            dbContext.SaveChanges();

            ApplicationRepository repository = new(_loggerMock.Object, dbContext);
            // Act

            ApplicationRole? foundApplicationRole = await repository.GetApplicationRoleAsync(application.Id, role.Id);

            // Assert
            using (new AssertionScope())
            {
                foundApplicationRole.Should().NotBeNull();
                foundApplicationRole.Id.Should().Be(applicationRole.Id);
                foundApplicationRole.ApplicationId.Should().Be(applicationRole.ApplicationId);
                foundApplicationRole.RoleId.Should().Be(applicationRole.RoleId);
                foundApplicationRole.Created.Should().BeCloseTo(applicationRole.Created ?? new(), TimeSpan.FromSeconds(5));
                foundApplicationRole.CreatedBy.Should().Be(applicationRole.CreatedBy);
                foundApplicationRole.Modified.Should().BeCloseTo(applicationRole.Modified ?? new(), TimeSpan.FromSeconds(5));
                foundApplicationRole.ModifiedBy.Should().Be(applicationRole.ModifiedBy);
            }
        }

        [Fact]
        public async Task GetApplicationRoleAsync_ReturnsNull_WhenApplicationRoleNotExist()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            ApplicationRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            ApplicationRole? notFoundApplicationRole = await repository.GetApplicationRoleAsync(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            notFoundApplicationRole.Should().BeNull();
        }

        [Fact]
        public async Task AddApplicationAsync_ReturnsTrue_WhenApplicationIsAdded()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            ApplicationRepository repository = new(_loggerMock.Object, dbContext);
            Application newApplication = TestHelper.GenerateApplications().Single();

            // Act
            bool result = await repository.AddApplicationAsync(newApplication);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AddApplicationAsync_ReturnsFalse_WhenSqlExceptionThrown()
        {
            // Arrange
            Mock<DbSet<Application>> dbSetMock = new();
            SqlException dbException = TestHelper.CreateSqlException("Sql Error");

            Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
            dbContextMock.Setup(x => x.Application).Returns(dbSetMock.Object);
            dbContextMock.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(dbException);

            ApplicationRepository repository = new(_loggerMock.Object, dbContextMock.Object);
            Application newApplication = TestHelper.GenerateApplications().Single();

            // Act
            bool result = await repository.AddApplicationAsync(newApplication);

            // Assert
            result.Should().BeFalse();

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"Error adding application with name {newApplication.Name}")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateApplicationAsync_ReturnsTrue_WhenApplicationIsUpdated()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            ApplicationRepository repository = new(_loggerMock.Object, dbContext);

            Application newApplication = TestHelper.GenerateApplications().Single();
            await repository.AddApplicationAsync(newApplication);

            Application ApplicationToUpdate = await repository.GetApplicationByIdAsync(newApplication.Id) ?? new();
            ApplicationToUpdate.Name = "UpdatedName";
            ApplicationToUpdate.Modified = DateTime.UtcNow;
            ApplicationToUpdate.ModifiedBy = "UpdatedApplicationUser";

            // Act
            bool result = await repository.UpdateApplicationAsync(ApplicationToUpdate);
            Application updatedApplication = await repository.GetApplicationByIdAsync(ApplicationToUpdate.Id) ?? new();

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeTrue();
                updatedApplication.Id.Should().Be(ApplicationToUpdate.Id);
                updatedApplication.Name.Should().Be("UpdatedName");
                updatedApplication.Created.Should().Be(ApplicationToUpdate.Created);
                updatedApplication.CreatedBy.Should().Be(ApplicationToUpdate.CreatedBy);
                updatedApplication.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                updatedApplication.ModifiedBy.Should().Be("UpdatedApplicationUser");
            }
        }

        [Fact]
        public async Task UpdateApplicationAsync_ReturnsFalse_WhenSqlExceptionThrown()
        {
            // Arrange
            Mock<DbSet<Application>> dbSetMock = new();
            SqlException dbException = TestHelper.CreateSqlException("Sql Error");

            Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
            dbContextMock.Setup(x => x.Application).Returns(dbSetMock.Object);
            dbContextMock.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(dbException);

            ApplicationRepository repository = new(_loggerMock.Object, dbContextMock.Object);
            Application ApplicationToUpdate = TestHelper.GenerateApplications().Single();
            ApplicationToUpdate.Name = "UpdatedName";

            // Act
            bool result = await repository.UpdateApplicationAsync(ApplicationToUpdate);

            // Assert
            result.Should().BeFalse();

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"Error updating application with ID {ApplicationToUpdate.Id}")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteApplicationAsync_ReturnsTrue_WhenApplicationIsDeleted()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            ApplicationRepository repository = new(_loggerMock.Object, dbContext);

            Application ApplicationToDelete = TestHelper.GenerateApplications().Single();
            await repository.AddApplicationAsync(ApplicationToDelete);

            // Act
            bool result = await repository.DeleteApplicationAsync(ApplicationToDelete);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteApplicationAsync_ReturnsFalse_WhenSqlExceptionThrown()
        {
            // Arrange
            Mock<DbSet<Application>> dbSetMock = new();
            SqlException dbException = TestHelper.CreateSqlException("Sql Error");

            Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
            dbContextMock.Setup(x => x.Application).Returns(dbSetMock.Object);
            dbContextMock.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(dbException);

            ApplicationRepository repository = new(_loggerMock.Object, dbContextMock.Object);
            Application ApplicationToDelete = TestHelper.GenerateApplications().Single();

            // Act
            bool result = await repository.DeleteApplicationAsync(ApplicationToDelete);

            // Assert
            result.Should().BeFalse();

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"Error deleting application with ID {ApplicationToDelete.Id}")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ApplicationRoleExistsAsync_ReturnsTrue_WhenApplicationRoleExists()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Role role = TestHelper.GenerateRoles().Single();
            Application application = TestHelper.GenerateApplications().Single();
            ApplicationRole ApplicationRole = TestHelper.GenerateApplicationRole(application.Id, role.Id);

            using UserManagementDbContext dbContext = new(_options);
            dbContext.Role.Add(role);
            dbContext.Application.Add(application);
            dbContext.ApplicationRole.Add(ApplicationRole);
            dbContext.SaveChanges();

            ApplicationRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            bool applicationRoleExists = await repository.ApplicationRoleExistsAsync(application.Id, role.Id);

            // Assert
            applicationRoleExists.Should().BeTrue();
        }

        [Fact]
        public async Task ApplicationRoleExistsAsync_ReturnsFalse_WhenApplicationRoleNotExists()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            ApplicationRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            bool applicationRoleExists = await repository.ApplicationRoleExistsAsync(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            applicationRoleExists.Should().BeFalse();
        }

        [Fact]
        public async Task RemoveApplicationRoleAsync_ReturnsTrue_WhenApplicationRoleIsRemoved()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Role role = TestHelper.GenerateRoles().Single();
            Application application = TestHelper.GenerateApplications().Single();
            ApplicationRole ApplicationRole = TestHelper.GenerateApplicationRole(application.Id, role.Id);

            using UserManagementDbContext dbContext = new(_options);
            dbContext.Role.Add(role);
            dbContext.Application.Add(application);
            dbContext.ApplicationRole.Add(ApplicationRole);
            dbContext.SaveChanges();

            ApplicationRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            bool result = await repository.RemoveApplicationRoleAsync(application.Id, role.Id);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeTrue();
                dbContext.ApplicationRole.Count(ar => ar.ApplicationId == application.Id && ar.RoleId == role.Id).Should().Be(0);
            }
        }

        [Fact]
        public async Task RemoveApplicationRoleAsync_ReturnsFalse_ApplicationRoleNotFound()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using UserManagementDbContext dbContext = new(_options);
            ApplicationRepository repository = new(_loggerMock.Object, dbContext);

            Guid applicationId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();

            // Act
            bool result = await repository.RemoveApplicationRoleAsync(applicationId, roleId);

            // Assert
            result.Should().BeFalse();
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"Application role not found with role ID {roleId} application ID {applicationId}")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task RemoveAllApplicationRolesAsync_ReturnsTrue_WhenApplicationRolesAreRemoved()
        {
            // Arrange
            DbContextOptions<UserManagementDbContext> _options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Guid applicationId = Guid.NewGuid();
            List<ApplicationRole> applicationRoles = TestHelper.GenerateApplicationRoles(applicationId, 3).ToList();

            using UserManagementDbContext dbContext = new(_options);
            dbContext.ApplicationRole.AddRange(applicationRoles);
            dbContext.SaveChanges();

            ApplicationRepository repository = new(_loggerMock.Object, dbContext);

            // Act
            bool result = await repository.RemoveAllApplicationRolesAsync(applicationId);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeTrue();
                dbContext.ApplicationRole.Count(ar => ar.ApplicationId == applicationId).Should().Be(0);
            }
        }
    }
}
