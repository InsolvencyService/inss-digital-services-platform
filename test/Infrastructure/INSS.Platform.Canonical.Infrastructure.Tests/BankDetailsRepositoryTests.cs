using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.Canonical.Application.Results;
using INSS.Platform.Canonical.Domain;
using INSS.Platform.Canonical.Infrastructure.Repositories;
using INSS.Platform.Canonical.Infrastructure.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace INSS.Platform.Canonical.Infrastructure.Tests;

public class BankDetailsRepositoryTests
{
    [Fact]
    public async Task AddAsync_AddsBankDetailsSuccessfully()
    {
        // Arrange
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();
        BankDetails bankDetails = TestHelper.GenerateBankDetails(Guid.NewGuid());
        Mock<ILogger<BankDetailsRepository>> loggerMock = new ();
        BankDetailsRepository repository = new (loggerMock.Object, dbContext);

        // Act
        OperationResult<BankDetails> result = await repository.AddAsync(bankDetails, CancellationToken.None);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Entity.Should().NotBeNull();
            result.Entity.Id.Should().Be(bankDetails.Id);
        }
    }
}
