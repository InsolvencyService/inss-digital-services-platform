using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.Canonical.Application.Results;
using INSS.Platform.Canonical.Domain;
using INSS.Platform.Canonical.Infrastructure.Repositories;
using INSS.Platform.Canonical.Infrastructure.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace INSS.Platform.Canonical.Infrastructure.Tests;

public class AddressRepositoryTests
{
    [Fact]
    public async Task AddAsync_AddsAddressSuccessfully()
    {
        // Arrange
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();
        Address address = TestHelper.GenerateAddress(Guid.NewGuid());
        Mock<ILogger<AddressRepository>> loggerMock = new ();
        AddressRepository repository = new (loggerMock.Object, dbContext);

        // Act
        OperationResult<Address> result = await repository.AddAsync(address, CancellationToken.None);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Entity.Should().NotBeNull();
            result.Entity.Id.Should().Be(address.Id);
        }
    }
}
