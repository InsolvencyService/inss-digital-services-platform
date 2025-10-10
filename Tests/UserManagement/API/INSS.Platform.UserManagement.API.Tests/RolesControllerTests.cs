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
    public class RolesControllerTests
    {
        private readonly Mock<ILogger<RolesController>> _loggerMock = new();
        private readonly Mock<IRoleRepository> _roleRepositoryMock = new();

        [Fact]
        public async Task Get_ReturnsOk_WhenRoleExists()
        {
            // Arrange
            Guid roleId = Guid.NewGuid();
            Role role = new() { Id = roleId, Name = "TestRole", Description = "TestDesc" };
            _roleRepositoryMock.Setup(r => r.GetRoleByIdAsync(roleId)).ReturnsAsync(role);

            RolesController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(roleId);

            // Assert
            using(new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(role);
            }
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenRoleDoesNotExist()
        {
            // Arrange
            Guid roleId = Guid.NewGuid();
            _roleRepositoryMock.Setup(r => r.GetRoleByIdAsync(roleId)).ReturnsAsync((Role?)null);

            RolesController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(roleId);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be("Role not found");
            }
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenRoleIsCreated()
        {
            // Arrange
            Role role = new() { Id = Guid.NewGuid(), Name = "NewRole", Description = "Desc" };
            _roleRepositoryMock.Setup(r => r.AddRoleAsync(role)).ReturnsAsync(true);

            RolesController controller = CreateController("Creator UserName");

            // Act
            IActionResult result = await controller.Create(role);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<CreatedAtActionResult>();
                CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
                createdResult.Value.Should().Be(role);
                createdResult.RouteValues.Should().NotBeNull();
                createdResult.RouteValues!["roleId"].Should().Be(role.Id);
                role.CreatedBy.Should().Be("Creator UserName");
                role.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                role.ModifiedBy.Should().BeNull();
                role.Modified.Should().BeNull();
            }
        }

        [Fact]
        public async Task Create_ReturnsStatusCode500_WhenRoleCreationFails()
        {
            // Arrange
            Role role = new() { Id = Guid.NewGuid(), Name = "NewRole", Description = "Desc" };
            _roleRepositoryMock.Setup(r => r.AddRoleAsync(role)).ReturnsAsync(false);

            RolesController controller = CreateController();

            // Act
            IActionResult result = await controller.Create(role);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to create role.");
            }
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenRoleIsUpdated()
        {
            // Arrange
            Role role = new() { Id = Guid.NewGuid(), Name = "UpdatedRole", Description = "Desc" };
            _roleRepositoryMock.Setup(r => r.UpdateRoleAsync(role)).ReturnsAsync(true);

            RolesController controller = CreateController("Modifier UserName");

            // Act
            IActionResult result = await controller.Update(role);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(role);
                role.ModifiedBy.Should().Be("Modifier UserName");
                role.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            }
        }

        [Fact]
        public async Task Update_ReturnsStatusCode500_WhenRoleUpdateFails()
        {
            // Arrange
            Role role = new() { Id = Guid.NewGuid(), Name = "UpdatedRole", Description = "Desc" };
            _roleRepositoryMock.Setup(r => r.UpdateRoleAsync(role)).ReturnsAsync(false);

            RolesController controller = CreateController();

            // Act
            IActionResult result = await controller.Update(role);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to update role.");
            }
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenRoleIsDeleted()
        {
            // Arrange
            Guid roleId = Guid.NewGuid();
            Role role = new() { Id = roleId, Name = "RoleToDelete", Description = "Desc" };
            _roleRepositoryMock.Setup(r => r.GetRoleByIdAsync(roleId)).ReturnsAsync(role);
            _roleRepositoryMock.Setup(r => r.DeleteRoleAsync(role)).ReturnsAsync(true);

            RolesController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(roleId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenRoleDoesNotExist()
        {
            // Arrange
            Guid roleId = Guid.NewGuid();
            _roleRepositoryMock.Setup(r => r.GetRoleByIdAsync(roleId)).ReturnsAsync((Role?)null);

            RolesController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(roleId);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be("Role not found");
            }
        }

        [Fact]
        public async Task Delete_ReturnsStatusCode500_WhenRoleDeleteFails()
        {
            // Arrange
            Guid roleId = Guid.NewGuid();
            Role role = new() { Id = roleId, Name = "RoleToDelete", Description = "Desc" };
            _roleRepositoryMock.Setup(r => r.GetRoleByIdAsync(roleId)).ReturnsAsync(role);
            _roleRepositoryMock.Setup(r => r.DeleteRoleAsync(role)).ReturnsAsync(false);

            RolesController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(roleId);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to delete role.");
            }
        }

        private RolesController CreateController(string? userName = null)
        {
            RolesController controller = new(_loggerMock.Object, _roleRepositoryMock.Object)
            {
                ControllerContext = TestHelper.CreateControllerContext(userName)
            };

            return controller;
        }
    }
}