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
    /// Unit tests for the <see cref="GroupRepository"/> class.
    /// </summary>
    public class GroupRepositoryTests
    {
        private readonly Mock<ILogger<GroupRepository>> _loggerMock = new();
        private readonly Mock<IRelationshipTypeLookupService> _relationshipTypeLookupServiceMock = new();

        private static UserManagementDbContext CreateDbContext()
        {
            DbContextOptions<UserManagementDbContext> options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new UserManagementDbContext(options);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsGroup_WhenGroupExists()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            Group group = TestHelper.GenerateGroups(1).Single();
            dbContext.Group.Add(group);
            dbContext.SaveChanges();

            GroupRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<Group> result = await repository.GetByIdAsync(group.Id, TestContext.Current.CancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Success.Should().BeTrue();
                result.Entity.Should().NotBeNull();
                result.Entity.Id.Should().Be(group.Id);
                result.Entity.Name.Should().Be(group.Name);
                result.Entity.Party.Should().NotBeNull();
                result.Entity.Party.SourceOfIntroduction.Should().Be(group.Party!.SourceOfIntroduction);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNotFound_WhenGroupDoesNotExist()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            GroupRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);
            Guid groupId = Guid.NewGuid();

            // Act
            OperationResult<Group> result = await repository.GetByIdAsync(groupId, TestContext.Current.CancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
                result.ErrorMessage.Should().Be($"Entity of type Group with ID {groupId} not found.");
                result.Entity.Should().BeNull();
            }

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"Entity of type Group with ID {groupId} not found")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsSqlException_WhenQueryFails()
        {
            // Arrange
            Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
            dbContextMock.Setup(db => db.Group).Throws(TestHelper.CreateSqlException("Sql Error"));

            Guid groupId = Guid.NewGuid();
            GroupRepository repository = new(_loggerMock.Object, dbContextMock.Object, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<Group> result = await repository.GetByIdAsync(groupId, TestContext.Current.CancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
                result.ErrorMessage.Should().Be($"A database error occurred. Error details: Sql Error While retrieving entity of type Group. Entity ID: {groupId}.");
            }

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"A database error occurred. Error details: Sql Error While retrieving entity of type Group. Entity ID: {groupId}.")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetIndividualsForGroupAsync_ReturnsIndividuals_WhenGroupAndRelationshipTypeExist()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            Group group = TestHelper.GenerateGroups(1).Single();
            Individual individual = TestHelper.GenerateIndividual();
            RelationshipType relationshipType = new()
            {
                Id = Guid.NewGuid(),
                Name = "HasMember",
                Description = "Indicates that a group has a member"
            };

            dbContext.Group.Add(group);
            dbContext.RelationshipType.Add(relationshipType);
            dbContext.Party.Add(individual.Party!);
            dbContext.Individual.Add(individual);
            dbContext.PartyRelationship.Add(new PartyRelationship
            {
                Id = Guid.NewGuid(),
                FromPartyId = group.PartyId,
                ToPartyId = individual.Party!.Id,
                RelationshipTypeId = relationshipType.Id,
                StartDate = DateTime.UtcNow
            });

            dbContext.SaveChanges();

            _relationshipTypeLookupServiceMock
                .Setup(s => s.GetByNameAsync("HasMember", It.IsAny<CancellationToken>()))
                .ReturnsAsync(relationshipType);

            GroupRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Individual>> result = await repository.GetIndividualsForGroupAsync(group.Id, TestContext.Current.CancellationToken);

            // Assert
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
        public async Task GetIndividualsForGroupAsync_ReturnsNotFound_WhenGroupDoesNotExist()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            GroupRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Individual>> result = await repository.GetIndividualsForGroupAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            }
        }

        [Fact]
        public async Task GetIndividualsForGroupAsync_ReturnsNotFound_WhenRelationshipTypeDoesNotExist()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            Group group = TestHelper.GenerateGroups(1).Single();
            dbContext.Group.Add(group);
            dbContext.SaveChanges();

            _relationshipTypeLookupServiceMock
                .Setup(s => s.GetByNameAsync("HasMember", It.IsAny<CancellationToken>()))
                .ReturnsAsync((RelationshipType?)null);

            GroupRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Individual>> result = await repository.GetIndividualsForGroupAsync(group.Id, TestContext.Current.CancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            }
        }

        [Fact]
        public async Task GetOrganisationsForGroupAsync_ReturnsOrganisations_WhenGroupAndRelationshipTypeExist()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            Group group = TestHelper.GenerateGroups(1).Single();
            Organisation organisation = TestHelper.GenerateOrganisation();
            RelationshipType relationshipType = new()
            {
                Id = Guid.NewGuid(),
                Name = "EmployedBy",
                Description = "Indicates that a group has an organisation member"
            };

            dbContext.Group.Add(group);
            dbContext.RelationshipType.Add(relationshipType);
            dbContext.Party.Add(organisation.Party!);
            dbContext.Organisation.Add(organisation);
            dbContext.PartyRelationship.Add(new PartyRelationship
            {
                Id = Guid.NewGuid(),
                FromPartyId = group.PartyId,
                ToPartyId = organisation.Party!.Id,
                RelationshipTypeId = relationshipType.Id,
                StartDate = DateTime.UtcNow
            });

            dbContext.SaveChanges();

            _relationshipTypeLookupServiceMock
                .Setup(s => s.GetByNameAsync("EmployedBy", It.IsAny<CancellationToken>()))
                .ReturnsAsync(relationshipType);

            GroupRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Organisation>> result = await repository.GetOrganisationsForGroupAsync(group.Id, TestContext.Current.CancellationToken);

            // Assert
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
        public async Task GetOrganisationsForGroupAsync_ReturnsNotFound_WhenGroupDoesNotExist()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            GroupRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Organisation>> result = await repository.GetOrganisationsForGroupAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            }
        }

        [Fact]
        public async Task GetOrganisationsForGroupAsync_ReturnsNotFound_WhenRelationshipTypeDoesNotExist()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            Group group = TestHelper.GenerateGroups(1).Single();
            dbContext.Group.Add(group);
            dbContext.SaveChanges();

            _relationshipTypeLookupServiceMock
                .Setup(s => s.GetByNameAsync("HasOrganisationMember", It.IsAny<CancellationToken>()))
                .ReturnsAsync((RelationshipType?)null);

            GroupRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Organisation>> result = await repository.GetOrganisationsForGroupAsync(group.Id, TestContext.Current.CancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            }
        }

        [Fact]
        public async Task GetPartiesForGroupAsync_ReturnsParties_WhenGroupAndRelationshipTypeExist()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            Group group = TestHelper.GenerateGroups(1).Single();
            Party party = TestHelper.GenerateParty();
            RelationshipType relationshipType = new()
            {
                Id = Guid.NewGuid(),
                Name = "HasPartyMember",
                Description = "Indicates that a group has a party member"
            };

            dbContext.Group.Add(group);
            dbContext.RelationshipType.Add(relationshipType);
            dbContext.Party.Add(party);
            dbContext.PartyRelationship.Add(new PartyRelationship
            {
                Id = Guid.NewGuid(),
                FromPartyId = group.PartyId,
                ToPartyId = party.Id,
                RelationshipTypeId = relationshipType.Id,
                StartDate = DateTime.UtcNow
            });

            dbContext.SaveChanges();

            _relationshipTypeLookupServiceMock
                .Setup(s => s.GetByNameAsync("HasPartyMember", It.IsAny<CancellationToken>()))
                .ReturnsAsync(relationshipType);

            GroupRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Party>> result = await repository.GetPartiesForGroupAsync(group.Id, TestContext.Current.CancellationToken);

            // Assert
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
        public async Task GetPartiesForGroupAsync_ReturnsNotFound_WhenGroupDoesNotExist()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            GroupRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Party>> result = await repository.GetPartiesForGroupAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            }
        }
    }
}