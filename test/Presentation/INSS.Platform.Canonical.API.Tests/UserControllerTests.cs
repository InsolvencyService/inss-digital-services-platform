using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.Canonical.API.Controllers;
using INSS.Platform.Canonical.Application.Repositories;
using INSS.Platform.Canonical.Application.Results;
using INSS.Platform.Canonical.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace INSS.Platform.Canonical.API.Tests;

public class UserControllerTests
{
    private readonly Mock<ILogger<UserController>> _loggerMock = new();
    private readonly Mock<IUserRepository> _repositoryMock = new();


    [Fact]
    public async Task Get_ReturnsOk_WhenUserFound()
    {
        Guid userId = Guid.NewGuid();
        User user = new() { Id = userId };


        _repositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Ok(user));
        
        UserController controller = CreateController();

        IActionResult result = await controller.Get(userId, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(user);
        }
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenUserNotFound()
    {
        Guid userId = Guid.NewGuid();
        
        _repositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Fail<User>("not found", OperationErrorCode.NotFound));
        
        UserController controller = CreateController();

        IActionResult result = await controller.Get(userId, CancellationToken.None);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Get_ReturnsServerError_WhenRepositoryFails()
    {
        Guid userId = Guid.NewGuid();
        
        _repositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Fail<User>("error", OperationErrorCode.SqlError));
        
        UserController controller = CreateController();

        IActionResult result = await controller.Get(userId, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be(500);
        }
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WhenSuccess()
    {
        IEnumerable<User> users = new List<User> { new() { Id = Guid.NewGuid() } };
        
        _repositoryMock.Setup(r => r.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Ok(users));
        
        UserController controller = CreateController();

        IActionResult result = await controller.Get(CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(users);
        }
    }

    [Fact]
    public async Task Create_ReturnsCreated_WhenSuccess()
    {
        User user = new() { Id = Guid.NewGuid() };
        
        _repositoryMock.Setup(r => r.AddAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Ok(user));
        
        UserController controller = CreateController("Creator Username");

        IActionResult result = await controller.Create(user, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<CreatedAtActionResult>();
            CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
            createdResult.Value.Should().Be(user);
            createdResult.RouteValues.Should().NotBeNull();
            createdResult.RouteValues["userId"].Should().Be(user.Id);
            user.CreatedBy.Should().Be("Creator Username");
            user.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            user.ModifiedBy.Should().BeNull();
            user.Modified.Should().BeNull();
        }
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenSuccess()
    {
        User user = new() { Id = Guid.NewGuid() };
        
        _repositoryMock.Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Ok(user));
        
        UserController controller = CreateController("Modifier Username");

        IActionResult result = await controller.Update(user, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(user);
            user.ModifiedBy.Should().Be("Modifier Username");
            user.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenSuccess()
    {
        Guid userId = Guid.NewGuid();
        User user = new() { Id = userId };
        
        _repositoryMock.Setup(r => r.DeleteAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Ok(user));
        
        UserController controller = CreateController();

        IActionResult result = await controller.Delete(userId, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<NoContentResult>();
        }
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenNotFound()
    {
        Guid userId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.DeleteAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Fail<User>("not found", OperationErrorCode.NotFound));

        UserController controller = CreateController();

        IActionResult result = await controller.Delete(userId, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }

    [Fact]
    public async Task Delete_ReturnsServerError_WhenRepositoryFails()
    {
        Guid userId = Guid.NewGuid();
        
        _repositoryMock.Setup(r => r.DeleteAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Fail<User>("error", OperationErrorCode.SqlError));
        
        UserController controller = CreateController();

        IActionResult result = await controller.Delete(userId, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be(500);
        }
    }

    private UserController CreateController(string? userName = null)
    {
        UserController controller = new(_loggerMock.Object, _repositoryMock.Object)
        {
            ControllerContext = TestHelper.CreateControllerContext(userName)
        };

        return controller;
    }
}
