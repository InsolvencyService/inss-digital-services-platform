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
    public class UsersControllerTests
    {
        private readonly Mock<ILogger<UsersController>> _loggerMock = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IOrganisationRepository> _organisationRepositoryMock = new();

        [Fact]
        public async Task Get_ReturnsOk_WhenUserExists()
        {
            Guid userId = Guid.NewGuid();
            User user = new() { Id = userId, Email = "test@example.com", FirstName = "Test", LastName = "User" };
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);

            UsersController controller = CreateController();

            IActionResult result = await controller.Get(userId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(user);
            }
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenUserDoesNotExist()
        {
            Guid userId = Guid.NewGuid();
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

            UsersController controller = CreateController();

            IActionResult result = await controller.Get(userId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be("User not found");
            }
        }

        [Fact]
        public async Task GetUserOrganisations_ReturnsOk_WithOrganisations()
        {
            Guid userId = Guid.NewGuid();
            var organisations = new List<Organisation>
            {
                new() { Id = Guid.NewGuid(), Name = "Org1" },
                new() { Id = Guid.NewGuid(), Name = "Org2" }
            };
            _organisationRepositoryMock.Setup(r => r.GetOrganisationsByUserIdAsync(userId)).ReturnsAsync(organisations);

            UsersController controller = CreateController();

            IActionResult result = await controller.GetUserOrganisations(userId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().BeEquivalentTo(organisations);
            }
        }

        [Fact]
        public async Task GetUserByIdentityProviderId_ReturnsOk_WhenUserExists()
        {
            Guid identityProviderId = Guid.NewGuid();
            string identityProviderUserId = "idp-user-123";
            User user = new() { Id = Guid.NewGuid(), Email = "idp@example.com", FirstName = "Idp", LastName = "User" };
            _userRepositoryMock.Setup(r => r.GetUserByIdentityProviderUserIdAsync(identityProviderUserId, identityProviderId)).ReturnsAsync(user);

            UsersController controller = CreateController();

            IActionResult result = await controller.GetUserByIdentityProviderId(identityProviderId, identityProviderUserId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(user);
            }
        }

        [Fact]
        public async Task GetUserByIdentityProviderId_ReturnsNotFound_WhenUserDoesNotExist()
        {
            Guid identityProviderId = Guid.NewGuid();
            string identityProviderUserId = "idp-user-123";
            _userRepositoryMock.Setup(r => r.GetUserByIdentityProviderUserIdAsync(identityProviderUserId, identityProviderId)).ReturnsAsync((User?)null);

            UsersController controller = CreateController();

            IActionResult result = await controller.GetUserByIdentityProviderId(identityProviderId, identityProviderUserId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be("User not found");
            }
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithUsers()
        {
            var users = new List<User>
            {
                new() { Id = Guid.NewGuid(), Email = "user1@example.com", FirstName = "User1", LastName = "Test" },
                new() { Id = Guid.NewGuid(), Email = "user2@example.com", FirstName = "User2", LastName = "Test" }
            };
            _userRepositoryMock.Setup(r => r.GetUsersAsync()).ReturnsAsync(users);

            var controller = CreateController();

            var result = await controller.GetAll();

            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().BeEquivalentTo(users);
            }
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenUserIsCreated()
        {
            User user = new() { Id = Guid.NewGuid(), Email = "new@example.com", FirstName = "New", LastName = "User" };
            _userRepositoryMock.Setup(r => r.AddUserAsync(user)).ReturnsAsync(true);

            UsersController controller = CreateController("Creator UserName");

            IActionResult result = await controller.Create(user);

            using (new AssertionScope())
            {
                result.Should().BeOfType<CreatedAtActionResult>();
                CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
                createdResult.Value.Should().Be(user);
                createdResult.RouteValues.Should().NotBeNull();
                createdResult.RouteValues!["userId"].Should().Be(user.Id);
                user.CreatedBy.Should().Be("Creator UserName");
                user.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                user.ModifiedBy.Should().BeNull();
                user.Modified.Should().BeNull();
            }
        }

        [Fact]
        public async Task Create_ReturnsStatusCode500_WhenUserCreationFails()
        {
            User user = new() { Id = Guid.NewGuid(), Email = "fail@example.com", FirstName = "Fail", LastName = "User" };
            _userRepositoryMock.Setup(r => r.AddUserAsync(user)).ReturnsAsync(false);

            UsersController controller = CreateController();

            IActionResult result = await controller.Create(user);

            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to create user.");
            }
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenUserIsUpdated()
        {
            User user = new() { Id = Guid.NewGuid(), Email = "update@example.com", FirstName = "Update", LastName = "User" };
            _userRepositoryMock.Setup(r => r.UpdateUserAsync(user)).ReturnsAsync(true);

            UsersController controller = CreateController("Modifier UserName");

            IActionResult result = await controller.Update(user);

            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(user);
                user.ModifiedBy.Should().Be("Modifier UserName");
                user.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            }
        }

        [Fact]
        public async Task Update_ReturnsStatusCode500_WhenUserUpdateFails()
        {
            User user = new() { Id = Guid.NewGuid(), Email = "failupdate@example.com", FirstName = "Fail", LastName = "User" };
            _userRepositoryMock.Setup(r => r.UpdateUserAsync(user)).ReturnsAsync(false);

            UsersController controller = CreateController();

            IActionResult result = await controller.Update(user);

            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to update user.");
            }
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenUserIsDeleted()
        {
            Guid userId = Guid.NewGuid();
            User user = new() { Id = userId, Email = "delete@example.com", FirstName = "Delete", LastName = "User" };
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.DeleteUserAsync(user)).ReturnsAsync(true);

            UsersController controller = CreateController();

            IActionResult result = await controller.Delete(userId);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenUserDoesNotExist()
        {
            Guid userId = Guid.NewGuid();
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

            UsersController controller = CreateController();

            IActionResult result = await controller.Delete(userId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be("User not found");
            }
        }

        [Fact]
        public async Task Delete_ReturnsStatusCode500_WhenUserDeleteFails()
        {
            Guid userId = Guid.NewGuid();
            User user = new() { Id = userId, Email = "faildelete@example.com", FirstName = "Fail", LastName = "User" };
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.DeleteUserAsync(user)).ReturnsAsync(false);

            UsersController controller = CreateController();

            IActionResult result = await controller.Delete(userId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to delete user.");
            }
        }

        private UsersController CreateController(string? userName = null)
        {
            UsersController controller = new(_loggerMock.Object, _userRepositoryMock.Object, _organisationRepositoryMock.Object)
            {
                ControllerContext = TestHelper.CreateControllerContext(userName)
            };

            return controller;
        }
    }
}