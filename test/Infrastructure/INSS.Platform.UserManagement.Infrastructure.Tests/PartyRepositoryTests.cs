using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;
using INSS.Platform.UserManagement.Infrastructure.Repositories;
using INSS.Platform.UserManagement.Infrastructure.Tests.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace INSS.Platform.UserManagement.Infrastructure.Tests;

/// <summary>
/// Unit tests for the <see cref="PartyRepository"/> class.
/// </summary>
public class PartyRepositoryTests
{
    private readonly Mock<ILogger<PartyRepository>> _loggerMock = new();

    private static UserManagementDbContext CreateDbContext()
    {
        DbContextOptions<UserManagementDbContext> options = new DbContextOptionsBuilder<UserManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new UserManagementDbContext(options);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsParty_WhenPartyExists()
    {
        using UserManagementDbContext dbContext = CreateDbContext();
        Party party = TestHelper.GenerateParty();
        dbContext.Party.Add(party);
        dbContext.SaveChanges();

        PartyRepository repository = new(_loggerMock.Object, dbContext);

        OperationResult<Party> result = await repository.GetByIdAsync(party.Id, TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Entity.Should().NotBeNull();
            result.Entity.Id.Should().Be(party.Id);
            result.Entity.Addresses.Should().NotBeNull();
            result.Entity.PartyType.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNotFound_WhenPartyDoesNotExist()
    {
        using UserManagementDbContext dbContext = CreateDbContext();
        PartyRepository repository = new(_loggerMock.Object, dbContext);
        Guid partyId = Guid.NewGuid();

        OperationResult<Party> result = await repository.GetByIdAsync(partyId, TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            result.ErrorMessage.Should().Be($"Entity of type Party with ID {partyId} not found.");
            result.Entity.Should().BeNull();
        }

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"Entity of type Party with ID {partyId} not found")),
                It.IsAny<SqlException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsSqlException_WhenQueryFails()
    {
        Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
        dbContextMock.Setup(db => db.Party).Throws(TestHelper.CreateSqlException("Sql Error"));

        Guid partyId = Guid.NewGuid();
        PartyRepository repository = new(_loggerMock.Object, dbContextMock.Object);

        OperationResult<Party> result = await repository.GetByIdAsync(partyId, TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Be($"A database error occurred. Error details: Sql Error While retrieving entity of type Party. Entity ID: {partyId}.");
        }

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"A database error occurred. Error details: Sql Error While retrieving entity of type Party. Entity ID: {partyId}.")),
                It.IsAny<SqlException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAsync_ReturnsAllParties()
    {
        using UserManagementDbContext dbContext = CreateDbContext();
        List<Party> parties = [.. TestHelper.GenerateParties(3)];
        dbContext.Party.AddRange(parties);
        dbContext.SaveChanges();

        PartyRepository repository = new(_loggerMock.Object, dbContext);

        OperationResult<IEnumerable<Party>> result = await repository.GetAsync(TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Entity.Should().NotBeNull();
            result.Entity.Should().HaveCount(3);
        }
    }

    [Fact]
    public async Task GetAsync_ThrowsSqlException_WhenQueryFails()
    {
        Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
        dbContextMock.Setup(db => db.Party).Throws(TestHelper.CreateSqlException("Sql Error"));

        PartyRepository repository = new(_loggerMock.Object, dbContextMock.Object);

        OperationResult<IEnumerable<Party>> result = await repository.GetAsync(TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain("A database error occurred. Error details: Sql Error While retrieving entity of type Party.");
        }
    }

    [Fact]
    public async Task GetGroupsForPartyAsync_ReturnsGroups_WhenPartyExists()
    {
        using UserManagementDbContext dbContext = CreateDbContext();
        Party party = TestHelper.GenerateParty();
        Group group = TestHelper.GenerateGroups(1).Single();

        dbContext.Party.Add(party);
        dbContext.Group.Add(group);
        dbContext.PartyRelationship.Add(new PartyRelationship
        {
            Id = Guid.NewGuid(),
            FromPartyId = party.Id,
            ToPartyId = group.Id,
            StartDate = DateTime.UtcNow
        });
        dbContext.SaveChanges();

        PartyRepository repository = new(_loggerMock.Object, dbContext);

        OperationResult<IReadOnlyList<Group>> result = await repository.GetGroupsForPartyAsync(party.Id, TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Entity.Should().NotBeNull();
            result.Entity.Should().ContainSingle();
            result.Entity[0].Id.Should().Be(group.Id);
        }
    }

    [Fact]
    public async Task GetGroupsForPartyAsync_ReturnsNotFound_WhenPartyDoesNotExist()
    {
        using UserManagementDbContext dbContext = CreateDbContext();
        PartyRepository repository = new(_loggerMock.Object, dbContext);

        OperationResult<IReadOnlyList<Group>> result = await repository.GetGroupsForPartyAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        }
    }

    [Fact]
    public async Task GetGroupsForPartyAsync_ThrowsSqlException_WhenQueryFails()
    {
        Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
        dbContextMock.Setup(db => db.Party).Throws(TestHelper.CreateSqlException("Sql Error"));

        PartyRepository repository = new(_loggerMock.Object, dbContextMock.Object);

        OperationResult<IReadOnlyList<Group>> result = await repository.GetGroupsForPartyAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain("An error occurred while retrieving Groups for Party with ID");
        }
    }

    [Fact]
    public async Task GetIndividualsForPartyAsync_ReturnsIndividuals_WhenPartyExists()
    {
        using UserManagementDbContext dbContext = CreateDbContext();
        Party party = TestHelper.GenerateParty();
        Individual individual = TestHelper.GenerateIndividual();

        dbContext.Party.Add(party);
        dbContext.Individual.Add(individual);
        dbContext.PartyRelationship.Add(new PartyRelationship
        {
            Id = Guid.NewGuid(),
            FromPartyId = party.Id,
            ToPartyId = individual.Id,
            StartDate = DateTime.UtcNow
        });
        dbContext.SaveChanges();

        PartyRepository repository = new(_loggerMock.Object, dbContext);

        OperationResult<IReadOnlyList<Individual>> result = await repository.GetIndividualsForPartyAsync(party.Id, TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Entity.Should().NotBeNull();
            result.Entity.Should().ContainSingle();
            result.Entity[0].Id.Should().Be(individual.Id);
        }
    }

    [Fact]
    public async Task GetIndividualsForPartyAsync_ReturnsNotFound_WhenPartyDoesNotExist()
    {
        using UserManagementDbContext dbContext = CreateDbContext();
        PartyRepository repository = new(_loggerMock.Object, dbContext);

        OperationResult<IReadOnlyList<Individual>> result = await repository.GetIndividualsForPartyAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        }
    }

    [Fact]
    public async Task GetIndividualsForPartyAsync_ThrowsSqlException_WhenQueryFails()
    {
        Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
        dbContextMock.Setup(db => db.Party).Throws(TestHelper.CreateSqlException("Sql Error"));

        PartyRepository repository = new(_loggerMock.Object, dbContextMock.Object);

        OperationResult<IReadOnlyList<Individual>> result = await repository.GetIndividualsForPartyAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain("An error occurred while retrieving Individuals for Party with ID");
        }
    }

    [Fact]
    public async Task GetOrganisationsForPartyAsync_ReturnsOrganisations_WhenPartyExists()
    {
        using UserManagementDbContext dbContext = CreateDbContext();
        Party party = TestHelper.GenerateParty();
        Organisation organisation = TestHelper.GenerateOrganisation();

        dbContext.Party.Add(party);
        dbContext.Organisation.Add(organisation);
        dbContext.PartyRelationship.Add(new PartyRelationship
        {
            Id = Guid.NewGuid(),
            FromPartyId = party.Id,
            ToPartyId = organisation.Id,
            StartDate = DateTime.UtcNow
        });
        dbContext.SaveChanges();

        PartyRepository repository = new(_loggerMock.Object, dbContext);

        OperationResult<IReadOnlyList<Organisation>> result = await repository.GetOrganisationsForPartyAsync(party.Id, TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Entity.Should().NotBeNull();
            result.Entity.Should().ContainSingle();
            result.Entity[0].Id.Should().Be(organisation.Id);
        }
    }

    [Fact]
    public async Task GetOrganisationsForPartyAsync_ReturnsNotFound_WhenPartyDoesNotExist()
    {
        using UserManagementDbContext dbContext = CreateDbContext();
        PartyRepository repository = new(_loggerMock.Object, dbContext);

        OperationResult<IReadOnlyList<Organisation>> result = await repository.GetOrganisationsForPartyAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        }
    }

    [Fact]
    public async Task GetOrganisationsForPartyAsync_ThrowsSqlException_WhenQueryFails()
    {
        Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
        dbContextMock.Setup(db => db.Party).Throws(TestHelper.CreateSqlException("Sql Error"));

        PartyRepository repository = new(_loggerMock.Object, dbContextMock.Object);

        OperationResult<IReadOnlyList<Organisation>> result = await repository.GetOrganisationsForPartyAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain("An error occurred while retrieving Organisations for Party with ID");
        }
    }

    [Fact]
    public async Task GetPartiesForPartyAsync_ReturnsParties_WhenPartyExists()
    {
        using UserManagementDbContext dbContext = CreateDbContext();
        Party party = TestHelper.GenerateParty();
        Party relatedParty = TestHelper.GenerateParty();

        dbContext.Party.Add(party);
        dbContext.Party.Add(relatedParty);
        dbContext.PartyRelationship.Add(new PartyRelationship
        {
            Id = Guid.NewGuid(),
            FromPartyId = party.Id,
            ToPartyId = relatedParty.Id,
            StartDate = DateTime.UtcNow
        });
        dbContext.SaveChanges();

        PartyRepository repository = new(_loggerMock.Object, dbContext);

        OperationResult<IReadOnlyList<Party>> result = await repository.GetPartiesForPartyAsync(party.Id, TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Entity.Should().NotBeNull();
            result.Entity.Should().ContainSingle();
            result.Entity[0].Id.Should().Be(relatedParty.Id);
        }
    }

    [Fact]
    public async Task GetPartiesForPartyAsync_ReturnsNotFound_WhenPartyDoesNotExist()
    {
        using UserManagementDbContext dbContext = CreateDbContext();
        PartyRepository repository = new(_loggerMock.Object, dbContext);

        OperationResult<IReadOnlyList<Party>> result = await repository.GetPartiesForPartyAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        }
    }

    [Fact]
    public async Task GetPartiesForPartyAsync_ThrowsSqlException_WhenQueryFails()
    {
        Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
        dbContextMock.Setup(db => db.Party).Throws(TestHelper.CreateSqlException("Sql Error"));

        PartyRepository repository = new(_loggerMock.Object, dbContextMock.Object);

        OperationResult<IReadOnlyList<Party>> result = await repository.GetPartiesForPartyAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
            result.ErrorMessage.Should().Contain("An error occurred while retrieving Parties for Party with ID");
        }
    }
}