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
    public class ApplicationsControllerTests
    {
        private readonly Mock<ILogger<ApplicationsController>> _loggerMock = new();
        private readonly Mock<IApplicationRepository> _applicationRepositoryMock = new();
        private readonly Mock<IRoleRepository> _roleRepositoryMock = new();

        [Fact]
        public async Task Get_ReturnsOk_WhenApplicationExists()
        {
            Guid appId = Guid.NewGuid();
            Application app = new() { Id = appId, Name = "TestApp", IdentityProviderId = Guid.NewGuid() };
            _applicationRepositoryMock.Setup(r => r.GetApplicationByIdAsync(appId)).ReturnsAsync(app);

            var controller = CreateController();

            IActionResult result = await controller.Get(appId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(app);
            }
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenApplicationDoesNotExist()
        {
            Guid appId = Guid.NewGuid();
            _applicationRepositoryMock.Setup(r => r.GetApplicationByIdAsync(appId)).ReturnsAsync((Application?)null);

            var controller = CreateController();

            IActionResult result = await controller.Get(appId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be("Application not found");
            }
        }

        [Fact]
        public async Task GetRoles_ReturnsOk_WithRoles()
        {
            Guid appId = Guid.NewGuid();
            var roles = new List<Role> { new() { Id = Guid.NewGuid(), Name = "Role1", Description = "Desc" } };
            _roleRepositoryMock.Setup(r => r.GetRolesByApplicationAsync(appId)).ReturnsAsync(roles);

            var controller = CreateController();

            IActionResult result = await controller.GetRoles(appId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().BeEquivalentTo(roles);
            }
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenApplicationIsCreated()
        {
            var app = new Application { Id = Guid.NewGuid(), Name = "NewApp", IdentityProviderId = Guid.NewGuid() };
            _applicationRepositoryMock.Setup(r => r.AddApplicationAsync(app)).ReturnsAsync(true);

            var controller = CreateController("Creator UserName");

            IActionResult result = await controller.Create(app);

            using (new AssertionScope())
            {
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdResult = (CreatedAtActionResult)result;
                createdResult.Value.Should().Be(app);
                createdResult.RouteValues.Should().NotBeNull();
                createdResult.RouteValues!["applicationId"].Should().Be(app.Id);
                app.CreatedBy.Should().Be("Creator UserName");
                app.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                app.ModifiedBy.Should().BeNull();
                app.Modified.Should().BeNull();
            }
        }

        [Fact]
        public async Task Create_ReturnsStatusCode500_WhenApplicationCreationFails()
        {
            var app = new Application { Id = Guid.NewGuid(), Name = "NewApp", IdentityProviderId = Guid.NewGuid() };
            _applicationRepositoryMock.Setup(r => r.AddApplicationAsync(app)).ReturnsAsync(false);

            var controller = CreateController();

            IActionResult result = await controller.Create(app);

            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                var objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to create application.");
            }
        }

        [Fact]
        public async Task AddRole_ReturnsCreated_WhenRoleAdded()
        {
            Guid appId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();
            _applicationRepositoryMock.Setup(r => r.ApplicationRoleExistsAsync(appId, roleId)).ReturnsAsync(false);
            _applicationRepositoryMock.Setup(r => r.AddApplicationRoleAsync(It.IsAny<ApplicationRole>())).ReturnsAsync(true);

            var controller = CreateController("Creator UserName");

            IActionResult result = await controller.AddRole(appId, roleId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<CreatedResult>();
            }
        }

        [Fact]
        public async Task AddRole_ReturnsConflict_WhenRoleAlreadyExists()
        {
            Guid appId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();
            _applicationRepositoryMock.Setup(r => r.ApplicationRoleExistsAsync(appId, roleId)).ReturnsAsync(true);

            var controller = CreateController();

            IActionResult result = await controller.AddRole(appId, roleId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<ConflictObjectResult>();
                ((ConflictObjectResult)result).Value.Should().Be("Role already exists in application.");
            }
        }

        [Fact]
        public async Task AddRole_ReturnsStatusCode500_WhenAddFails()
        {
            Guid appId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();
            _applicationRepositoryMock.Setup(r => r.ApplicationRoleExistsAsync(appId, roleId)).ReturnsAsync(false);
            _applicationRepositoryMock.Setup(r => r.AddApplicationRoleAsync(It.IsAny<ApplicationRole>())).ReturnsAsync(false);

            var controller = CreateController();

            IActionResult result = await controller.AddRole(appId, roleId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                var objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to add role to application.");
            }
        }

        [Fact]
        public async Task RemoveRole_ReturnsNoContent_WhenRoleRemoved()
        {
            Guid appId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();
            _applicationRepositoryMock.Setup(r => r.ApplicationRoleExistsAsync(appId, roleId)).ReturnsAsync(true);
            _applicationRepositoryMock.Setup(r => r.RemoveApplicationRoleAsync(appId, roleId)).ReturnsAsync(true);

            var controller = CreateController();

            IActionResult result = await controller.RemoveRole(appId, roleId);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task RemoveRole_ReturnsNotFound_WhenRoleNotFound()
        {
            Guid appId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();
            _applicationRepositoryMock.Setup(r => r.ApplicationRoleExistsAsync(appId, roleId)).ReturnsAsync(false);

            var controller = CreateController();

            IActionResult result = await controller.RemoveRole(appId, roleId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be("Role not found in application");
            }
        }

        [Fact]
        public async Task RemoveRole_ReturnsStatusCode500_WhenRemoveFails()
        {
            Guid appId = Guid.NewGuid();
            Guid roleId = Guid.NewGuid();
            _applicationRepositoryMock.Setup(r => r.ApplicationRoleExistsAsync(appId, roleId)).ReturnsAsync(true);
            _applicationRepositoryMock.Setup(r => r.RemoveApplicationRoleAsync(appId, roleId)).ReturnsAsync(false);

            var controller = CreateController();

            IActionResult result = await controller.RemoveRole(appId, roleId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                var objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to remove role from application.");
            }
        }

        [Fact]
        public async Task RemoveRoles_ReturnsNoContent_WhenAllRolesRemoved()
        {
            Guid appId = Guid.NewGuid();
            _applicationRepositoryMock.Setup(r => r.RemoveAllApplicationRolesAsync(appId)).ReturnsAsync(true);

            var controller = CreateController();

            IActionResult result = await controller.RemoveRoles(appId);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task RemoveRoles_ReturnsStatusCode500_WhenRemoveFails()
        {
            Guid appId = Guid.NewGuid();
            _applicationRepositoryMock.Setup(r => r.RemoveAllApplicationRolesAsync(appId)).ReturnsAsync(false);

            var controller = CreateController();

            IActionResult result = await controller.RemoveRoles(appId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                var objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to remove all roles from application.");
            }
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenApplicationIsUpdated()
        {
            var app = new Application { Id = Guid.NewGuid(), Name = "UpdatedApp", IdentityProviderId = Guid.NewGuid() };
            _applicationRepositoryMock.Setup(r => r.UpdateApplicationAsync(app)).ReturnsAsync(true);

            var controller = CreateController("Modifier UserName");

            IActionResult result = await controller.Update(app);

            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(app);
                app.ModifiedBy.Should().Be("Modifier UserName");
                app.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            }
        }

        [Fact]
        public async Task Update_ReturnsStatusCode500_WhenUpdateFails()
        {
            var app = new Application { Id = Guid.NewGuid(), Name = "UpdatedApp", IdentityProviderId = Guid.NewGuid() };
            _applicationRepositoryMock.Setup(r => r.UpdateApplicationAsync(app)).ReturnsAsync(false);

            var controller = CreateController();

            IActionResult result = await controller.Update(app);

            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                var objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to update application.");
            }
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenApplicationIsDeleted()
        {
            Guid appId = Guid.NewGuid();
            var app = new Application { Id = appId, Name = "AppToDelete", IdentityProviderId = Guid.NewGuid() };
            _applicationRepositoryMock.Setup(r => r.GetApplicationByIdAsync(appId)).ReturnsAsync(app);
            _applicationRepositoryMock.Setup(r => r.DeleteApplicationAsync(app)).ReturnsAsync(true);

            var controller = CreateController();

            IActionResult result = await controller.Delete(appId);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenApplicationDoesNotExist()
        {
            Guid appId = Guid.NewGuid();
            _applicationRepositoryMock.Setup(r => r.GetApplicationByIdAsync(appId)).ReturnsAsync((Application?)null);

            var controller = CreateController();

            IActionResult result = await controller.Delete(appId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be("Application not found");
            }
        }

        [Fact]
        public async Task Delete_ReturnsStatusCode500_WhenDeleteFails()
        {
            Guid appId = Guid.NewGuid();
            var app = new Application { Id = appId, Name = "AppToDelete", IdentityProviderId = Guid.NewGuid() };
            _applicationRepositoryMock.Setup(r => r.GetApplicationByIdAsync(appId)).ReturnsAsync(app);
            _applicationRepositoryMock.Setup(r => r.DeleteApplicationAsync(app)).ReturnsAsync(false);

            var controller = CreateController();

            IActionResult result = await controller.Delete(appId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                var objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to delete application.");
            }
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithApplications()
        {
            var applications = new List<Application>
            {
                new() { Id = Guid.NewGuid(), Name = "App1", IdentityProviderId = Guid.NewGuid() },
                new() { Id = Guid.NewGuid(), Name = "App2", IdentityProviderId = Guid.NewGuid() }
            };
            _applicationRepositoryMock.Setup(r => r.GetApplicationsAsync()).ReturnsAsync(applications);

            var controller = CreateController();

            IActionResult result = await controller.GetApplicationsAsync();

            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().BeEquivalentTo(applications);
            }
        }

        private ApplicationsController CreateController(string? userName = null)
        {
            var controller = new ApplicationsController(_loggerMock.Object, _applicationRepositoryMock.Object, _roleRepositoryMock.Object)
            {
                ControllerContext = TestHelper.CreateControllerContext(userName)
            };
            return controller;
        }
    }
}