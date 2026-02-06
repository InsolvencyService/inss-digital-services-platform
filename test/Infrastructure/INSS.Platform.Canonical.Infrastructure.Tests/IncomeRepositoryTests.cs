using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.Canonical.Application.Results;
using INSS.Platform.Canonical.Domain;
using INSS.Platform.Canonical.Infrastructure.Repositories;
using INSS.Platform.Canonical.Infrastructure.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace INSS.Platform.Canonical.Infrastructure.Tests;

public class IncomeRepositoryTests
{
    [Fact]
    public async Task AddAsync_AddsIncomeSuccessfully()
    {
        // Arrange
        CanonicalDbContext dbContext = TestHelper.CreateDbContext();
        Income income = TestHelper.GenerateIncome(Guid.NewGuid());
        Mock<ILogger<IncomeRepository>> loggerMock = new ();
        IncomeRepository repository = new (loggerMock.Object, dbContext);

        // Act
        OperationResult<Income> result = await repository.AddAsync(income, CancellationToken.None);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Entity.Should().NotBeNull();
            result.Entity.Id.Should().Be(income.Id);
        }
    }
}
