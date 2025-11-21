using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.UserManagement.Abstractions.Results;
using INSS.Platform.UserManagement.Abstractions.Services;
using INSS.Platform.UserManagement.Entities;
using INSS.Platform.UserManagement.Repository.Repositories;
using INSS.Platform.UserManagement.Repository.Tests.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace INSS.Platform.UserManagement.Repository.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="OrganisationRepository"/> class.
    /// </summary>
    public class OrganisationRepositoryTests
    {
        private readonly Mock<ILogger<OrganisationRepository>> _loggerMock = new();
        private readonly Mock<IRelationshipTypeLookupService> _relationshipTypeLookupServiceMock = new();

        private static UserManagementDbContext CreateDbContext()
        {
            DbContextOptions<UserManagementDbContext> options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new UserManagementDbContext(options);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsOrganisation_WhenOrganisationExists()
        {
            using UserManagementDbContext dbContext = CreateDbContext();
            Organisation organisation = TestHelper.GenerateOrganisation();
            dbContext.Organisation.Add(organisation);
            dbContext.SaveChanges();

            OrganisationRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            OperationResult<Organisation> result = await repository.GetByIdAsync(organisation.Id, TestContext.Current.CancellationToken);

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Success.Should().BeTrue();
                result.Entity.Should().NotBeNull();
                result.Entity.Id.Should().Be(organisation.Id);
                result.Entity.Party.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNotFound_WhenOrganisationDoesNotExist()
        {
            using UserManagementDbContext dbContext = CreateDbContext();
            OrganisationRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);
            Guid organisationId = Guid.NewGuid();

            OperationResult<Organisation> result = await repository.GetByIdAsync(organisationId, TestContext.Current.CancellationToken);

            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
                result.ErrorMessage.Should().Be($"Entity of type Organisation with ID {organisationId} not found.");
                result.Entity.Should().BeNull();
            }

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"Entity of type Organisation with ID {organisationId} not found")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsSqlException_WhenQueryFails()
        {
            Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
            dbContextMock.Setup(db => db.Organisation).Throws(TestHelper.CreateSqlException("Sql Error"));

            Guid organisationId = Guid.NewGuid();
            OrganisationRepository repository = new(_loggerMock.Object, dbContextMock.Object, _relationshipTypeLookupServiceMock.Object);

            OperationResult<Organisation> result = await repository.GetByIdAsync(organisationId, TestContext.Current.CancellationToken);

            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
                result.ErrorMessage.Should().Be($"A database error occurred. Error details: Sql Error While retrieving entity of type Organisation. Entity ID: {organisationId}.");
            }

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"A database error occurred. Error details: Sql Error While retrieving entity of type Organisation. Entity ID: {organisationId}.")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetGroupsForOrganisationAsync_ReturnsGroups_WhenOrganisationAndRelationshipTypeExist()
        {
            using UserManagementDbContext dbContext = CreateDbContext();
            Organisation organisation = TestHelper.GenerateOrganisation();
            Group group = TestHelper.GenerateGroups(1).Single();
            RelationshipType relationshipType = new()
            {
                Id = Guid.NewGuid(),
                Name = "MemberOf",
                Description = "Indicates that an organisation is a member of a group"
            };

            dbContext.Organisation.Add(organisation);
            dbContext.Group.Add(group);
            dbContext.RelationshipType.Add(relationshipType);
            dbContext.Party.Add(group.Party!);
            dbContext.PartyRelationship.Add(new PartyRelationship
            {
                Id = Guid.NewGuid(),
                FromPartyId = organisation.PartyId,
                ToPartyId = group.Party!.Id,
                RelationshipTypeId = relationshipType.Id,
                StartDate = DateTime.UtcNow
            });

            dbContext.SaveChanges();

            _relationshipTypeLookupServiceMock
                .Setup(s => s.GetByNameAsync("MemberOf", It.IsAny<CancellationToken>()))
                .ReturnsAsync(relationshipType);

            OrganisationRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            OperationResult<IReadOnlyList<Group>> result = await repository.GetGroupsForOrganisationAsync(organisation.Id, TestContext.Current.CancellationToken);

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
        public async Task GetGroupsForOrganisationAsync_ReturnsNotFound_WhenOrganisationDoesNotExist()
        {
            using UserManagementDbContext dbContext = CreateDbContext();
            OrganisationRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            OperationResult<IReadOnlyList<Group>> result = await repository.GetGroupsForOrganisationAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            }
        }

        [Fact]
        public async Task GetGroupsForOrganisationAsync_ReturnsNotFound_WhenRelationshipTypeDoesNotExist()
        {
            using UserManagementDbContext dbContext = CreateDbContext();
            Organisation organisation = TestHelper.GenerateOrganisation();
            dbContext.Organisation.Add(organisation);
            dbContext.SaveChanges();

            _relationshipTypeLookupServiceMock
                .Setup(s => s.GetByNameAsync("MemberOf", It.IsAny<CancellationToken>()))
                .ReturnsAsync((RelationshipType?)null);

            OrganisationRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            OperationResult<IReadOnlyList<Group>> result = await repository.GetGroupsForOrganisationAsync(organisation.Id, TestContext.Current.CancellationToken);

            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            }
        }

        [Fact]
        public async Task GetGroupsForOrganisationAsync_ThrowsSqlException_WhenQueryFails()
        {
            Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
            dbContextMock.Setup(db => db.Organisation).Throws(TestHelper.CreateSqlException("Sql Error"));

            Guid organisationId = Guid.NewGuid();
            OrganisationRepository repository = new(_loggerMock.Object, dbContextMock.Object, _relationshipTypeLookupServiceMock.Object);

            OperationResult<IReadOnlyList<Group>> result = await repository.GetGroupsForOrganisationAsync(organisationId, TestContext.Current.CancellationToken);

            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
                result.ErrorMessage.Should().Contain("An error occurred while retrieving Groups for Organisation with ID");
            }

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains("An error occurred while retrieving Groups for Organisation with ID")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetIndividualsForOrganisationAsync_ReturnsIndividuals_WhenOrganisationAndRelationshipTypeExist()
        {
            using UserManagementDbContext dbContext = CreateDbContext();
            Organisation organisation = TestHelper.GenerateOrganisation();
            Individual individual = TestHelper.GenerateIndividual();
            RelationshipType relationshipType = new()
            {
                Id = Guid.NewGuid(),
                Name = "Employs",
                Description = "Indicates that an organisation employs an individual"
            };

            dbContext.Organisation.Add(organisation);
            dbContext.Individual.Add(individual);
            dbContext.RelationshipType.Add(relationshipType);
            dbContext.Party.Add(individual.Party!);
            dbContext.PartyRelationship.Add(new PartyRelationship
            {
                Id = Guid.NewGuid(),
                FromPartyId = organisation.PartyId,
                ToPartyId = individual.Party!.Id,
                RelationshipTypeId = relationshipType.Id,
                StartDate = DateTime.UtcNow
            });

            dbContext.SaveChanges();

            _relationshipTypeLookupServiceMock
                .Setup(s => s.GetByNameAsync("Employs", It.IsAny<CancellationToken>()))
                .ReturnsAsync(relationshipType);

            OrganisationRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            OperationResult<IReadOnlyList<Individual>> result = await repository.GetIndividualsForOrganisationAsync(organisation.Id, TestContext.Current.CancellationToken);

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
        public async Task GetIndividualsForOrganisationAsync_ReturnsNotFound_WhenOrganisationDoesNotExist()
        {
            using UserManagementDbContext dbContext = CreateDbContext();
            OrganisationRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            OperationResult<IReadOnlyList<Individual>> result = await repository.GetIndividualsForOrganisationAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            }
        }

        [Fact]
        public async Task GetIndividualsForOrganisationAsync_ReturnsNotFound_WhenRelationshipTypeDoesNotExist()
        {
            using UserManagementDbContext dbContext = CreateDbContext();
            Organisation organisation = TestHelper.GenerateOrganisation();
            dbContext.Organisation.Add(organisation);
            dbContext.SaveChanges();

            _relationshipTypeLookupServiceMock
                .Setup(s => s.GetByNameAsync("Employs", It.IsAny<CancellationToken>()))
                .ReturnsAsync((RelationshipType?)null);

            OrganisationRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            OperationResult<IReadOnlyList<Individual>> result = await repository.GetIndividualsForOrganisationAsync(organisation.Id, TestContext.Current.CancellationToken);

            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            }
        }

        [Fact]
        public async Task GetIndividualsForOrganisationAsync_ThrowsSqlException_WhenQueryFails()
        {
            Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
            dbContextMock.Setup(db => db.Organisation).Throws(TestHelper.CreateSqlException("Sql Error"));

            Guid organisationId = Guid.NewGuid();
            OrganisationRepository repository = new(_loggerMock.Object, dbContextMock.Object, _relationshipTypeLookupServiceMock.Object);

            OperationResult<IReadOnlyList<Individual>> result = await repository.GetIndividualsForOrganisationAsync(organisationId, TestContext.Current.CancellationToken);

            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
                result.ErrorMessage.Should().Contain("An error occurred while retrieving Individuals for Organisation with ID");
            }

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains("An error occurred while retrieving Individuals for Organisation with ID")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetPartiesForOrganisationAsync_ReturnsParties_WhenOrganisationExists()
        {
            using UserManagementDbContext dbContext = CreateDbContext();
            Organisation organisation = TestHelper.GenerateOrganisation();
            Party party = TestHelper.GenerateParty();

            dbContext.Organisation.Add(organisation);
            dbContext.Party.Add(party);
            dbContext.PartyRelationship.Add(new PartyRelationship
            {
                Id = Guid.NewGuid(),
                FromPartyId = organisation.PartyId,
                ToPartyId = party.Id,
                StartDate = DateTime.UtcNow
            });

            dbContext.SaveChanges();

            OrganisationRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            OperationResult<IReadOnlyList<Party>> result = await repository.GetPartiesForOrganisationAsync(organisation.Id, TestContext.Current.CancellationToken);

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Success.Should().BeTrue();
                result.Entity.Should().NotBeNull();
                result.Entity.Should().ContainSingle();
                result.Entity[0].Id.Should().Be(party.Id);
            }
        }

        [Fact]
        public async Task GetPartiesForOrganisationAsync_ReturnsNotFound_WhenOrganisationDoesNotExist()
        {
            using UserManagementDbContext dbContext = CreateDbContext();
            OrganisationRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            OperationResult<IReadOnlyList<Party>> result = await repository.GetPartiesForOrganisationAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            }
        }

        [Fact]
        public async Task GetPartiesForOrganisationAsync_ThrowsSqlException_WhenQueryFails()
        {
            Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
            dbContextMock.Setup(db => db.Organisation).Throws(TestHelper.CreateSqlException("Sql Error"));

            Guid organisationId = Guid.NewGuid();
            OrganisationRepository repository = new(_loggerMock.Object, dbContextMock.Object, _relationshipTypeLookupServiceMock.Object);

            OperationResult<IReadOnlyList<Party>> result = await repository.GetPartiesForOrganisationAsync(organisationId, TestContext.Current.CancellationToken);

            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
                result.ErrorMessage.Should().Contain("An error occurred while retrieving Parties for Organisation with ID");
            }

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains("An error occurred while retrieving Parties for Organisation with ID")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}