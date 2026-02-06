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

public class BankDetailsControllerTests
{
    private readonly Mock<ILogger<BankDetailsController>> _loggerMock = new();
    private readonly Mock<IBankDetailsRepository> _repositoryMock = new();

    [Fact]
    public async Task Get_ReturnsOk_WhenBankDetailsFound()
    {
        Guid id = Guid.NewGuid();
        BankDetails entity = new() { Id = id };
        
        _repositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Ok(entity));

        BankDetailsController controller = CreateController();

        IActionResult result = await controller.Get(id, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(entity);
        }
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenNotFound()
    {
        Guid id = Guid.NewGuid();
        
        _repositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Fail<BankDetails>("not found", OperationErrorCode.NotFound));
        
        BankDetailsController controller = CreateController();

        IActionResult result = await controller.Get(id, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }

    [Fact]
    public async Task Get_ReturnsServerError_WhenRepositoryFails()
    {
        Guid id = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Fail<BankDetails>("error", OperationErrorCode.SqlError));

        BankDetailsController controller = CreateController();

        IActionResult result = await controller.Get(id, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be(500);
        }
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WhenSuccess()
    {
        IEnumerable<BankDetails> entities = new List<BankDetails> { new() { Id = Guid.NewGuid() } };
        
        _repositoryMock.Setup(r => r.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Ok(entities));
        
        BankDetailsController controller = CreateController();

        IActionResult result = await controller.Get(CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(entities);
        }
    }

    [Fact]
    public async Task Create_ReturnsCreated_WhenSuccess()
    {
        BankDetails entity = new() { Id = Guid.NewGuid() };
        
        _repositoryMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Ok(entity));
        
        BankDetailsController controller = CreateController("Creator Username");

        IActionResult result = await controller.Create(entity, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<CreatedAtActionResult>();
            CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
            createdResult.Value.Should().Be(entity);
            createdResult.RouteValues.Should().NotBeNull();
            createdResult.RouteValues["bankDetailsId"].Should().Be(entity.Id);
            entity.CreatedBy.Should().Be("Creator Username");
            entity.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            entity.ModifiedBy.Should().BeNull();
            entity.Modified.Should().BeNull();
        }
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenSuccess()
    {
        BankDetails entity = new() { Id = Guid.NewGuid() };
        
        _repositoryMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Ok(entity));
        
        BankDetailsController controller = CreateController("Modifier Username");

        IActionResult result = await controller.Update(entity, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(entity);
            entity.ModifiedBy.Should().Be("Modifier Username");
            entity.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenSuccess()
    {
        Guid id = Guid.NewGuid();
        BankDetails entity = new() { Id = id };
        
        _repositoryMock.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Ok(entity));
        
        BankDetailsController controller = CreateController();

        IActionResult result = await controller.Delete(id, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<NoContentResult>();
        }
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenNotFound()
    {
        Guid id = Guid.NewGuid();

        _repositoryMock.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Fail<BankDetails>("not found", OperationErrorCode.NotFound));

        BankDetailsController controller = CreateController();

        IActionResult result = await controller.Delete(id, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }

    [Fact]
    public async Task Delete_ReturnsServerError_WhenRepositoryFails()
    {
        Guid id = Guid.NewGuid();
        
        _repositoryMock.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Fail<BankDetails>("error", OperationErrorCode.SqlError));
        
        BankDetailsController controller = CreateController();

        IActionResult result = await controller.Delete(id, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be(500);
        }
    }

    private BankDetailsController CreateController(string? userName = null)
    {
        BankDetailsController controller = new(_loggerMock.Object, _repositoryMock.Object)
        {
            ControllerContext = TestHelper.CreateControllerContext(userName)
        };

        return controller;
    }
}
