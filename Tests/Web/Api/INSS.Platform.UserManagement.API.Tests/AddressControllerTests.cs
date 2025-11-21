using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Abstractions.Results;
using INSS.Platform.UserManagement.API.Controllers;
using INSS.Platform.UserManagement.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace INSS.Platform.UserManagement.API.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="AddressController"/> class.
    /// </summary>
    public class AddressControllerTests
    {
        private readonly Mock<ILogger<AddressController>> _loggerMock = new();
        private readonly Mock<IAddressRepository> _addressRepositoryMock = new();

        [Fact]
        public async Task GetById_ReturnsOk_WhenAddressExists()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid addressId = Guid.NewGuid();
            OperationResult<Address> operationResult = Operation.Ok(new Address { Id = addressId });
            _addressRepositoryMock.Setup(r => r.GetByIdAsync(addressId, cancellationToken)).ReturnsAsync(operationResult);

            AddressController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(addressId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(operationResult.Entity);
            }
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenAddressDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid addressId = Guid.NewGuid();
            OperationResult<Address> operationResult = Operation.Fail<Address>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _addressRepositoryMock.Setup(r => r.GetByIdAsync(addressId, cancellationToken)).ReturnsAsync(operationResult);

            AddressController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(addressId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Address not found with ID {addressId}");
            }
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WhenAddressesExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            List<Address> addresses = new() { new Address { Id = Guid.NewGuid() } };
            OperationResult<IEnumerable<Address>> operationResult = Operation.Ok<IEnumerable<Address>>(addresses);
            _addressRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            AddressController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().BeEquivalentTo(addresses);
            }
        }

        [Fact]
        public async Task GetAll_ReturnsStatusCode500_WhenRepositoryFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            OperationResult<IEnumerable<Address>> operationResult = Operation.Fail<IEnumerable<Address>>("Failed to get addresses.", OperationErrorCode.SqlError);
            _addressRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            AddressController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to get addresses.");
            }
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenAddressIsCreated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Address address = new() { Id = Guid.NewGuid() };
            OperationResult<Address> operationResult = Operation.Ok(address);
            _addressRepositoryMock.Setup(r => r.AddAsync(address, cancellationToken)).ReturnsAsync(operationResult);

            AddressController controller = CreateController("Creator UserName");

            // Act
            IActionResult result = await controller.Create(address, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<CreatedAtActionResult>();
                CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
                createdResult.Value.Should().Be(address);
                createdResult.RouteValues.Should().NotBeNull();
                createdResult.RouteValues["addressId"].Should().Be(address.Id);
                address.CreatedBy.Should().Be("Creator UserName");
                address.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                address.ModifiedBy.Should().BeNull();
                address.Modified.Should().BeNull();
            }
        }

        [Fact]
        public async Task Create_ReturnsStatusCode500_WhenAddressCreationFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Address address = new() { Id = Guid.NewGuid() };
            OperationResult<Address> operationResult = Operation.Fail<Address>("Failed to create address.", OperationErrorCode.SqlError);
            _addressRepositoryMock.Setup(r => r.AddAsync(address, cancellationToken)).ReturnsAsync(operationResult);

            AddressController controller = CreateController();

            // Act
            IActionResult result = await controller.Create(address, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to create address.");
            }
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenAddressIsUpdated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Address address = new() { Id = Guid.NewGuid() };
            OperationResult<Address> operationResult = Operation.Ok(address);
            _addressRepositoryMock.Setup(r => r.UpdateAsync(address, cancellationToken)).ReturnsAsync(operationResult);

            AddressController controller = CreateController("Modifier UserName");

            // Act
            IActionResult result = await controller.Update(address, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(address);
                address.ModifiedBy.Should().Be("Modifier UserName");
                address.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            }
        }

        [Fact]
        public async Task Update_ReturnsStatusCode500_WhenAddressUpdateFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Address address = new() { Id = Guid.NewGuid() };
            OperationResult<Address> operationResult = Operation.Fail<Address>("Failed to update address.", OperationErrorCode.SqlError);
            _addressRepositoryMock.Setup(r => r.UpdateAsync(address, cancellationToken)).ReturnsAsync(operationResult);

            AddressController controller = CreateController();

            // Act
            IActionResult result = await controller.Update(address, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to update address.");
            }
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenAddressIsDeleted()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid addressId = Guid.NewGuid();
            OperationResult<Address> operationResult = Operation.Ok(new Address { Id = addressId });
            _addressRepositoryMock.Setup(r => r.DeleteAsync(addressId, cancellationToken)).ReturnsAsync(operationResult);

            AddressController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(addressId, cancellationToken);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenAddressDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid addressId = Guid.NewGuid();
            OperationResult<Address> operationResult = Operation.Fail<Address>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _addressRepositoryMock.Setup(r => r.DeleteAsync(addressId, cancellationToken)).ReturnsAsync(operationResult);

            AddressController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(addressId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Address not found with ID {addressId}");
            }
        }

        [Fact]
        public async Task Delete_ReturnsStatusCode500_WhenAddressDeleteFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid addressId = Guid.NewGuid();
            OperationResult<Address> operationResult = Operation.Fail<Address>("Failed to delete address.", OperationErrorCode.SqlError);
            _addressRepositoryMock.Setup(r => r.DeleteAsync(addressId, cancellationToken)).ReturnsAsync(operationResult);

            AddressController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(addressId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to delete address.");
            }
        }

        private AddressController CreateController(string? userName = null)
        {
            AddressController controller = new(_loggerMock.Object, _addressRepositoryMock.Object)
            {
                ControllerContext = TestHelper.CreateControllerContext(userName)
            };

            return controller;
        }
    }
}