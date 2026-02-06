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

public class AddressControllerTests
{
    private readonly Mock<ILogger<AddressController>> _loggerMock = new();
    private readonly Mock<IAddressRepository> _repositoryMock = new();

    [Fact]
    public async Task Get_ReturnsOk_WhenAddressFound()
    {
        Guid addressId = Guid.NewGuid();
        Address address = new() { Id = addressId };
        
        _repositoryMock.Setup(r => r.GetByIdAsync(addressId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Ok(address));

        AddressController controller = CreateController();

        IActionResult result = await controller.Get(addressId, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(address);
        }
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenAddressNotFound()
    {
        Guid addressId = Guid.NewGuid();
        
        _repositoryMock.Setup(r => r.GetByIdAsync(addressId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Fail<Address>("not found", OperationErrorCode.NotFound));
        
        AddressController controller = CreateController();

        IActionResult result = await controller.Get(addressId, CancellationToken.None);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Get_ReturnsServerError_WhenRepositoryFails()
    {
        Guid addressId = Guid.NewGuid();
        
        _repositoryMock.Setup(r => r.GetByIdAsync(addressId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Fail<Address>("error", OperationErrorCode.SqlError));
        
        AddressController controller = CreateController();

        IActionResult result = await controller.Get(addressId, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be(500);
        }
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WhenSuccess()
    {
        IEnumerable<Address> addresses = new List<Address> { new() { Id = Guid.NewGuid() } };
        
        _repositoryMock.Setup(r => r.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Ok(addresses));
        
        AddressController controller = CreateController();

        IActionResult result = await controller.Get(CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(addresses);
        }
    }

    [Fact]
    public async Task Create_ReturnsCreated_WhenSuccess()
    {
        Address address = new() { Id = Guid.NewGuid() };
        
        _repositoryMock.Setup(r => r.AddAsync(address, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Ok(address));
        
        AddressController controller = CreateController("Creator Username");

        IActionResult result = await controller.Create(address, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<CreatedAtActionResult>();
            CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
            createdResult.Value.Should().Be(address);
            createdResult.RouteValues.Should().NotBeNull();
            createdResult.RouteValues["addressId"].Should().Be(address.Id);
            address.CreatedBy.Should().Be("Creator Username");
            address.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            address.ModifiedBy.Should().BeNull();
            address.Modified.Should().BeNull();
        }
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenSuccess()
    {
        Address address = new() { Id = Guid.NewGuid() };
        
        _repositoryMock.Setup(r => r.UpdateAsync(address, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Ok(address));

        AddressController controller = CreateController("Modifier Username");

        IActionResult result = await controller.Update(address, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(address);
            address.ModifiedBy.Should().Be("Modifier Username");
            address.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenSuccess()
    {
        Guid addressId = Guid.NewGuid();
        Address address = new() { Id = addressId };
        
        _repositoryMock.Setup(r => r.DeleteAsync(addressId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Ok(address));
        
        AddressController controller = CreateController();
        
        IActionResult result = await controller.Delete(addressId, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenNotFound()
    {
        Guid addressId = Guid.NewGuid();
        
        _repositoryMock.Setup(r => r.DeleteAsync(addressId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Fail<Address>("not found", OperationErrorCode.NotFound));
        
        AddressController controller = CreateController();

        IActionResult result = await controller.Delete(addressId, CancellationToken.None);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Delete_ReturnsServerError_WhenRepositoryFails()
    {
        Guid addressId = Guid.NewGuid();
        
        _repositoryMock.Setup(r => r.DeleteAsync(addressId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Operation.Fail<Address>("error", OperationErrorCode.SqlError));
        
        AddressController controller = CreateController();

        IActionResult result = await controller.Delete(addressId, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be(500);
        }
    }

    private AddressController CreateController(string? userName = null)
    {
        AddressController controller = new(_loggerMock.Object, _repositoryMock.Object)
        {
            ControllerContext = TestHelper.CreateControllerContext(userName)
        };

        return controller;
    }
}
