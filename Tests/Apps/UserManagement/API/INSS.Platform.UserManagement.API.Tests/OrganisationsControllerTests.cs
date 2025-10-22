using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.UserManagement.API.Controllers;
using INSS.Platform.UserManagement.Core.Entities;
using INSS.Platform.UserManagement.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace INSS.Platform.UserManagement.API.Tests
{
    public class OrganisationsControllerTests
    {
        private readonly Mock<ILogger<OrganisationsController>> _loggerMock = new();
        private readonly Mock<IOrganisationRepository> _organisationRepositoryMock = new();
        private readonly Mock<IApplicationRepository> _applicationRepositoryMock = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IRoleRepository> _roleRepositoryMock = new();

        [Fact]
        public async Task Get_ReturnsOk_WhenOrganisationExists()
        {
            Guid organisationId = Guid.NewGuid();
            Organisation organisation = new() { Id = organisationId, Name = "TestOrg" };
            _organisationRepositoryMock.Setup(r => r.GetOrganisationByIdAsync(organisationId)).ReturnsAsync(organisation);

            OrganisationsController controller = CreateController();

            IActionResult result = await controller.Get(organisationId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(organisation);
            }
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenOrganisationDoesNotExist()
        {
            Guid organisationId = Guid.NewGuid();
            _organisationRepositoryMock.Setup(r => r.GetOrganisationByIdAsync(organisationId)).ReturnsAsync((Organisation?)null);

            OrganisationsController controller = CreateController();

            IActionResult result = await controller.Get(organisationId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be("Organisation not found");
            }
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithOrganisations()
        {
            var organisations = new List<Organisation>
            {
                new() { Id = Guid.NewGuid(), Name = "Org1" },
                new() { Id = Guid.NewGuid(), Name = "Org2" }
            };
            _organisationRepositoryMock.Setup(r => r.GetOrganisationsAsync()).ReturnsAsync(organisations);

            OrganisationsController controller = CreateController();

            IActionResult result = await controller.Get();

            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().BeEquivalentTo(organisations);
            }
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenOrganisationIsCreated()
        {
            Organisation organisation = new() { Id = Guid.NewGuid(), Name = "NewOrg" };
            _organisationRepositoryMock.Setup(r => r.AddOrganisationAsync(organisation)).ReturnsAsync(true);

            OrganisationsController controller = CreateController("Creator UserName");

            IActionResult result = await controller.Create(organisation);

            using (new AssertionScope())
            {
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdResult = (CreatedAtActionResult)result;
                createdResult.Value.Should().Be(organisation);
                createdResult.RouteValues.Should().NotBeNull();
                createdResult.RouteValues!["organisationId"].Should().Be(organisation.Id);
                organisation.CreatedBy.Should().Be("Creator UserName");
                organisation.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                organisation.ModifiedBy.Should().BeNull();
                organisation.Modified.Should().BeNull();
            }
        }

        [Fact]
        public async Task Create_ReturnsStatusCode500_WhenOrganisationCreationFails()
        {
            Organisation organisation = new() { Id = Guid.NewGuid(), Name = "NewOrg" };
            _organisationRepositoryMock.Setup(r => r.AddOrganisationAsync(organisation)).ReturnsAsync(false);

            OrganisationsController controller = CreateController();

            IActionResult result = await controller.Create(organisation);

            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                var objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to create organisation.");
            }
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenOrganisationIsUpdated()
        {
            Organisation organisation = new() { Id = Guid.NewGuid(), Name = "UpdatedOrg" };
            _organisationRepositoryMock.Setup(r => r.UpdateOrganisationAsync(organisation)).ReturnsAsync(true);

            OrganisationsController controller = CreateController("Modifier UserName");

            IActionResult result = await controller.Update(organisation);

            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(organisation);
                organisation.ModifiedBy.Should().Be("Modifier UserName");
                organisation.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            }
        }

        [Fact]
        public async Task Update_ReturnsStatusCode500_WhenOrganisationUpdateFails()
        {
            Organisation organisation = new() { Id = Guid.NewGuid(), Name = "UpdatedOrg" };
            _organisationRepositoryMock.Setup(r => r.UpdateOrganisationAsync(organisation)).ReturnsAsync(false);

            OrganisationsController controller = CreateController();

            IActionResult result = await controller.Update(organisation);

            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                var objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to update organisation.");
            }
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenOrganisationIsDeleted()
        {
            Guid organisationId = Guid.NewGuid();
            Organisation organisation = new() { Id = organisationId, Name = "OrgToDelete" };
            _organisationRepositoryMock.Setup(r => r.GetOrganisationByIdAsync(organisationId)).ReturnsAsync(organisation);
            _organisationRepositoryMock.Setup(r => r.DeleteOrganisationAsync(organisation)).ReturnsAsync(true);

            OrganisationsController controller = CreateController();

            IActionResult result = await controller.Delete(organisationId);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenOrganisationDoesNotExist()
        {
            Guid organisationId = Guid.NewGuid();
            _organisationRepositoryMock.Setup(r => r.GetOrganisationByIdAsync(organisationId)).ReturnsAsync((Organisation?)null);

            OrganisationsController controller = CreateController();

            IActionResult result = await controller.Delete(organisationId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be("Organisation not found");
            }
        }

        [Fact]
        public async Task Delete_ReturnsStatusCode500_WhenOrganisationDeleteFails()
        {
            Guid organisationId = Guid.NewGuid();
            Organisation organisation = new() { Id = organisationId, Name = "OrgToDelete" };
            _organisationRepositoryMock.Setup(r => r.GetOrganisationByIdAsync(organisationId)).ReturnsAsync(organisation);
            _organisationRepositoryMock.Setup(r => r.DeleteOrganisationAsync(organisation)).ReturnsAsync(false);

            OrganisationsController controller = CreateController();

            IActionResult result = await controller.Delete(organisationId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                var objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to delete organisation.");
            }
        }

        [Fact]
        public async Task GetUsers_ReturnsOk_WithUsers()
        {
            Guid organisationId = Guid.NewGuid();
            var users = new List<User> { new() { Id = Guid.NewGuid(), Email = "a@b.com" } };
            _userRepositoryMock.Setup(r => r.GetUsersByOrganisationAsync(organisationId)).ReturnsAsync(users);

            var controller = CreateController();
            var result = await controller.GetUsers(organisationId);

            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().BeEquivalentTo(users);
        }

        [Fact]
        public async Task GetApplicationsByOrganisationUser_ReturnsOk_WithApplications()
        {
            Guid organisationId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            var applications = new List<Application> { new() { Id = Guid.NewGuid(), Name = "App" } };
            _applicationRepositoryMock.Setup(r => r.GetApplicationsByOrganisationUserAsync(organisationId, userId)).ReturnsAsync(applications);

            var controller = CreateController();
            var result = await controller.GetApplicationsByOrganisationUser(organisationId, userId);

            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().BeEquivalentTo(applications);
        }

        [Fact]
        public async Task GetRolesByOrganisationUserApplication_ReturnsOk_WithRoles()
        {
            Guid organisationId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Guid applicationId = Guid.NewGuid();
            var roles = new List<Role> { new() { Id = Guid.NewGuid(), Name = "Role" } };
            _roleRepositoryMock.Setup(r => r.GetRolesByOrganisationUserApplicationAsync(organisationId, userId, applicationId)).ReturnsAsync(roles);

            var controller = CreateController();
            var result = await controller.GetRolesByOrganisationUserApplication(organisationId, userId, applicationId);

            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().BeEquivalentTo(roles);
        }

        [Fact]
        public async Task AddUser_ReturnsCreated_WhenUserAdded()
        {
            Guid organisationId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            _organisationRepositoryMock.Setup(r => r.OrganisationUserExistsAsync(organisationId, userId)).ReturnsAsync(false);
            _organisationRepositoryMock.Setup(r => r.AddOrganisationUserAsync(It.IsAny<OrganisationUser>())).ReturnsAsync(true);

            var controller = CreateController();
            var result = await controller.AddUser(organisationId, userId);

            result.Should().BeOfType<CreatedResult>();
        }

        [Fact]
        public async Task AddUser_ReturnsConflict_WhenUserAlreadyExists()
        {
            Guid organisationId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            _organisationRepositoryMock.Setup(r => r.OrganisationUserExistsAsync(organisationId, userId)).ReturnsAsync(true);

            var controller = CreateController();
            var result = await controller.AddUser(organisationId, userId);

            result.Should().BeOfType<ConflictObjectResult>();
        }

        [Fact]
        public async Task AddUser_ReturnsStatusCode500_WhenAddFails()
        {
            Guid organisationId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            _organisationRepositoryMock.Setup(r => r.OrganisationUserExistsAsync(organisationId, userId)).ReturnsAsync(false);
            _organisationRepositoryMock.Setup(r => r.AddOrganisationUserAsync(It.IsAny<OrganisationUser>())).ReturnsAsync(false);

            var controller = CreateController();
            var result = await controller.AddUser(organisationId, userId);

            result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task RemoveUser_ReturnsNoContent_WhenUserRemoved()
        {
            Guid organisationId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            _organisationRepositoryMock.Setup(r => r.OrganisationUserExistsAsync(organisationId, userId)).ReturnsAsync(true);
            _organisationRepositoryMock.Setup(r => r.RemoveOrganisationUserAsync(organisationId, userId)).ReturnsAsync(true);

            var controller = CreateController();
            var result = await controller.RemoveUser(organisationId, userId);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task RemoveUser_ReturnsNotFound_WhenUserNotFound()
        {
            Guid organisationId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            _organisationRepositoryMock.Setup(r => r.OrganisationUserExistsAsync(organisationId, userId)).ReturnsAsync(false);

            var controller = CreateController();
            var result = await controller.RemoveUser(organisationId, userId);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task RemoveUser_ReturnsStatusCode500_WhenRemoveFails()
        {
            Guid organisationId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            _organisationRepositoryMock.Setup(r => r.OrganisationUserExistsAsync(organisationId, userId)).ReturnsAsync(true);
            _organisationRepositoryMock.Setup(r => r.RemoveOrganisationUserAsync(organisationId, userId)).ReturnsAsync(false);

            var controller = CreateController();
            var result = await controller.RemoveUser(organisationId, userId);

            result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task RemoveUsers_ReturnsNoContent_WhenAllUsersRemoved()
        {
            Guid organisationId = Guid.NewGuid();
            _organisationRepositoryMock.Setup(r => r.RemoveAllOrganisationUsersAsync(organisationId)).ReturnsAsync(true);

            var controller = CreateController();
            var result = await controller.RemoveUsers(organisationId);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task RemoveUsers_ReturnsStatusCode500_WhenRemoveFails()
        {
            Guid organisationId = Guid.NewGuid();
            _organisationRepositoryMock.Setup(r => r.RemoveAllOrganisationUsersAsync(organisationId)).ReturnsAsync(false);

            var controller = CreateController();
            var result = await controller.RemoveUsers(organisationId);

            result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task AddApplicationRoleToOrganisationUser_ReturnsCreated_WhenRoleAssigned()
        {
            Guid organisationId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Guid applicationId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();

            _organisationRepositoryMock.Setup(r => r.OrganisationUserExistsAsync(organisationId, userId)).ReturnsAsync(true);
            _applicationRepositoryMock.Setup(r => r.ApplicationRoleExistsAsync(applicationId, roleId)).ReturnsAsync(true);
            _organisationRepositoryMock.Setup(r => r.GetOrganisationUserAsync(organisationId, userId)).ReturnsAsync(new OrganisationUser { Id = Guid.NewGuid() });
            _applicationRepositoryMock.Setup(r => r.GetApplicationRoleAsync(applicationId, roleId)).ReturnsAsync(new ApplicationRole { Id = Guid.NewGuid() });
            _organisationRepositoryMock.Setup(r => r.OrganisationUserApplicationRoleExistsAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(false);
            _organisationRepositoryMock.Setup(r => r.AddOrganisationUserApplicationRoleAsync(It.IsAny<OrganisationUserApplicationRole>())).ReturnsAsync(true);

            var controller = CreateController();
            var result = await controller.AddApplicationRoleToOrganisationUser(organisationId, userId, applicationId, roleId);

            result.Should().BeOfType<CreatedResult>();
        }

        [Fact]
        public async Task AddApplicationRoleToOrganisationUser_ReturnsNotFound_WhenOrganisationUserNotFound()
        {
            Guid organisationId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Guid applicationId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();

            _organisationRepositoryMock.Setup(r => r.OrganisationUserExistsAsync(organisationId, userId)).ReturnsAsync(false);

            var controller = CreateController();
            var result = await controller.AddApplicationRoleToOrganisationUser(organisationId, userId, applicationId, roleId);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task AddApplicationRoleToOrganisationUser_ReturnsNotFound_WhenApplicationRoleNotFound()
        {
            Guid organisationId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Guid applicationId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();

            _organisationRepositoryMock.Setup(r => r.OrganisationUserExistsAsync(organisationId, userId)).ReturnsAsync(true);
            _applicationRepositoryMock.Setup(r => r.ApplicationRoleExistsAsync(applicationId, roleId)).ReturnsAsync(false);

            var controller = CreateController();
            var result = await controller.AddApplicationRoleToOrganisationUser(organisationId, userId, applicationId, roleId);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task AddApplicationRoleToOrganisationUser_ReturnsConflict_WhenAssociationExists()
        {
            Guid organisationId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Guid applicationId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();

            _organisationRepositoryMock.Setup(r => r.OrganisationUserExistsAsync(organisationId, userId)).ReturnsAsync(true);
            _applicationRepositoryMock.Setup(r => r.ApplicationRoleExistsAsync(applicationId, roleId)).ReturnsAsync(true);
            _organisationRepositoryMock.Setup(r => r.GetOrganisationUserAsync(organisationId, userId)).ReturnsAsync(new OrganisationUser { Id = Guid.NewGuid() });
            _applicationRepositoryMock.Setup(r => r.GetApplicationRoleAsync(applicationId, roleId)).ReturnsAsync(new ApplicationRole { Id = Guid.NewGuid() });
            _organisationRepositoryMock.Setup(r => r.OrganisationUserApplicationRoleExistsAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(true);

            var controller = CreateController();
            var result = await controller.AddApplicationRoleToOrganisationUser(organisationId, userId, applicationId, roleId);

            result.Should().BeOfType<ConflictObjectResult>();
        }

        [Fact]
        public async Task AddApplicationRoleToOrganisationUser_ReturnsStatusCode500_WhenAddFails()
        {
            Guid organisationId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Guid applicationId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();

            _organisationRepositoryMock.Setup(r => r.OrganisationUserExistsAsync(organisationId, userId)).ReturnsAsync(true);
            _applicationRepositoryMock.Setup(r => r.ApplicationRoleExistsAsync(applicationId, roleId)).ReturnsAsync(true);
            _organisationRepositoryMock.Setup(r => r.GetOrganisationUserAsync(organisationId, userId)).ReturnsAsync(new OrganisationUser { Id = Guid.NewGuid() });
            _applicationRepositoryMock.Setup(r => r.GetApplicationRoleAsync(applicationId, roleId)).ReturnsAsync(new ApplicationRole { Id = Guid.NewGuid() });
            _organisationRepositoryMock.Setup(r => r.OrganisationUserApplicationRoleExistsAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(false);
            _organisationRepositoryMock.Setup(r => r.AddOrganisationUserApplicationRoleAsync(It.IsAny<OrganisationUserApplicationRole>())).ReturnsAsync(false);

            var controller = CreateController();
            var result = await controller.AddApplicationRoleToOrganisationUser(organisationId, userId, applicationId, roleId);

            result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task RemoveApplicationRoleFromOrganisationUser_ReturnsNoContent_WhenRoleRemoved()
        {
            Guid organisationId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Guid applicationId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();

            _organisationRepositoryMock.Setup(r => r.OrganisationUserExistsAsync(organisationId, userId)).ReturnsAsync(true);
            _applicationRepositoryMock.Setup(r => r.ApplicationRoleExistsAsync(applicationId, roleId)).ReturnsAsync(true);
            _organisationRepositoryMock.Setup(r => r.GetOrganisationUserAsync(organisationId, userId)).ReturnsAsync(new OrganisationUser { Id = Guid.NewGuid() });
            _applicationRepositoryMock.Setup(r => r.GetApplicationRoleAsync(applicationId, roleId)).ReturnsAsync(new ApplicationRole { Id = Guid.NewGuid() });
            _organisationRepositoryMock.Setup(r => r.OrganisationUserApplicationRoleExistsAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(true);
            _organisationRepositoryMock.Setup(r => r.RemoveOrganisationUserApplicationRoleAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(true);

            var controller = CreateController();
            var result = await controller.RemoveApplicationRoleFromOrganisationUser(organisationId, userId, applicationId, roleId);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task RemoveApplicationRoleFromOrganisationUser_ReturnsNotFound_WhenOrganisationUserNotFound()
        {
            Guid organisationId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Guid applicationId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();

            _organisationRepositoryMock.Setup(r => r.OrganisationUserExistsAsync(organisationId, userId)).ReturnsAsync(false);

            var controller = CreateController();
            var result = await controller.RemoveApplicationRoleFromOrganisationUser(organisationId, userId, applicationId, roleId);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task RemoveApplicationRoleFromOrganisationUser_ReturnsNotFound_WhenApplicationRoleNotFound()
        {
            Guid organisationId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Guid applicationId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();

            _organisationRepositoryMock.Setup(r => r.OrganisationUserExistsAsync(organisationId, userId)).ReturnsAsync(true);
            _applicationRepositoryMock.Setup(r => r.ApplicationRoleExistsAsync(applicationId, roleId)).ReturnsAsync(false);

            var controller = CreateController();
            var result = await controller.RemoveApplicationRoleFromOrganisationUser(organisationId, userId, applicationId, roleId);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task RemoveApplicationRoleFromOrganisationUser_ReturnsNotFound_WhenAssociationNotFound()
        {
            Guid organisationId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Guid applicationId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();

            _organisationRepositoryMock.Setup(r => r.OrganisationUserExistsAsync(organisationId, userId)).ReturnsAsync(true);
            _applicationRepositoryMock.Setup(r => r.ApplicationRoleExistsAsync(applicationId, roleId)).ReturnsAsync(true);
            _organisationRepositoryMock.Setup(r => r.GetOrganisationUserAsync(organisationId, userId)).ReturnsAsync(new OrganisationUser { Id = Guid.NewGuid() });
            _applicationRepositoryMock.Setup(r => r.GetApplicationRoleAsync(applicationId, roleId)).ReturnsAsync(new ApplicationRole { Id = Guid.NewGuid() });
            _organisationRepositoryMock.Setup(r => r.OrganisationUserApplicationRoleExistsAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(false);

            var controller = CreateController();
            var result = await controller.RemoveApplicationRoleFromOrganisationUser(organisationId, userId, applicationId, roleId);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task RemoveApplicationRoleFromOrganisationUser_ReturnsStatusCode500_WhenRemoveFails()
        {
            Guid organisationId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Guid applicationId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();

            _organisationRepositoryMock.Setup(r => r.OrganisationUserExistsAsync(organisationId, userId)).ReturnsAsync(true);
            _applicationRepositoryMock.Setup(r => r.ApplicationRoleExistsAsync(applicationId, roleId)).ReturnsAsync(true);
            _organisationRepositoryMock.Setup(r => r.GetOrganisationUserAsync(organisationId, userId)).ReturnsAsync(new OrganisationUser { Id = Guid.NewGuid() });
            _applicationRepositoryMock.Setup(r => r.GetApplicationRoleAsync(applicationId, roleId)).ReturnsAsync(new ApplicationRole { Id = Guid.NewGuid() });
            _organisationRepositoryMock.Setup(r => r.OrganisationUserApplicationRoleExistsAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(true);
            _organisationRepositoryMock.Setup(r => r.RemoveOrganisationUserApplicationRoleAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(false);

            var controller = CreateController();
            var result = await controller.RemoveApplicationRoleFromOrganisationUser(organisationId, userId, applicationId, roleId);

            result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be(500);
        }

        private OrganisationsController CreateController(string? userName = null)
        {
            var controller = new OrganisationsController(
                _loggerMock.Object,
                _organisationRepositoryMock.Object,
                _userRepositoryMock.Object,
                _applicationRepositoryMock.Object,
                _roleRepositoryMock.Object)
            {
                ControllerContext = TestHelper.CreateControllerContext(userName)
            };
            return controller;
        }
    }
}