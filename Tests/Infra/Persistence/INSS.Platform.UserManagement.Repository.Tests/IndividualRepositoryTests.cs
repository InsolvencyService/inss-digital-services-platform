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
    /// Unit tests for the <see cref="IndividualRepository"/> class.
    /// </summary>
    public class IndividualRepositoryTests
    {
        private readonly Mock<ILogger<IndividualRepository>> _loggerMock = new();
        private readonly Mock<IRelationshipTypeLookupService> _relationshipTypeLookupServiceMock = new();

        private static UserManagementDbContext CreateDbContext()
        {
            DbContextOptions<UserManagementDbContext> options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new UserManagementDbContext(options);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsIndividual_WhenIndividualExists()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            Individual individual = TestHelper.GenerateIndividual();
            dbContext.Individual.Add(individual);
            dbContext.SaveChanges();

            IndividualRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<Individual> result = await repository.GetByIdAsync(individual.Id, TestContext.Current.CancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Success.Should().BeTrue();
                result.Entity.Should().NotBeNull();
                result.Entity.Id.Should().Be(individual.Id);
                result.Entity.Party.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNotFound_WhenIndividualDoesNotExist()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            IndividualRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);
            Guid individualId = Guid.NewGuid();

            // Act
            OperationResult<Individual> result = await repository.GetByIdAsync(individualId, TestContext.Current.CancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
                result.ErrorMessage.Should().Be($"Entity of type Individual with ID {individualId} not found.");
                result.Entity.Should().BeNull();
            }

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"Entity of type Individual with ID {individualId} not found")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsSqlException_WhenQueryFails()
        {
            // Arrange
            Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
            dbContextMock.Setup(db => db.Individual).Throws(TestHelper.CreateSqlException("Sql Error"));

            Guid individualId = Guid.NewGuid();
            IndividualRepository repository = new(_loggerMock.Object, dbContextMock.Object, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<Individual> result = await repository.GetByIdAsync(individualId, TestContext.Current.CancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.SqlError);
                result.ErrorMessage.Should().Be($"A database error occurred. Error details: Sql Error While retrieving entity of type Individual. Entity ID: {individualId}.");
            }

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"A database error occurred. Error details: Sql Error While retrieving entity of type Individual. Entity ID: {individualId}.")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetOrganisationsForIndividualAsync_ReturnsOrganisations_WhenIndividualAndRelationshipTypeExist()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            Individual individual = TestHelper.GenerateIndividual();
            Organisation organisation = TestHelper.GenerateOrganisation();
            RelationshipType relationshipType = new()
            {
                Id = Guid.NewGuid(),
                Name = "EmployedBy",
                Description = "Indicates that an individual is employed by an organisation"
            };

            dbContext.Individual.Add(individual);
            dbContext.Organisation.Add(organisation);
            dbContext.RelationshipType.Add(relationshipType);
            dbContext.Party.Add(organisation.Party!);
            dbContext.PartyRelationship.Add(new PartyRelationship
            {
                Id = Guid.NewGuid(),
                FromPartyId = individual.PartyId,
                ToPartyId = organisation.Party!.Id,
                RelationshipTypeId = relationshipType.Id,
                StartDate = DateTime.UtcNow
            });

            dbContext.SaveChanges();

            _relationshipTypeLookupServiceMock
                .Setup(s => s.GetByNameAsync("EmployedBy", It.IsAny<CancellationToken>()))
                .ReturnsAsync(relationshipType);

            IndividualRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Organisation>> result = await repository.GetOrganisationsForIndividualAsync(individual.Id, TestContext.Current.CancellationToken);

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
        public async Task GetOrganisationsForIndividualAsync_ReturnsNotFound_WhenIndividualDoesNotExist()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            IndividualRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Organisation>> result = await repository.GetOrganisationsForIndividualAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            }
        }

        [Fact]
        public async Task GetOrganisationsForIndividualAsync_ReturnsNotFound_WhenRelationshipTypeDoesNotExist()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            Individual individual = TestHelper.GenerateIndividual();
            dbContext.Individual.Add(individual);
            dbContext.SaveChanges();

            _relationshipTypeLookupServiceMock
                .Setup(s => s.GetByNameAsync("EmployedBy", It.IsAny<CancellationToken>()))
                .ReturnsAsync((RelationshipType?)null);

            IndividualRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Organisation>> result = await repository.GetOrganisationsForIndividualAsync(individual.Id, TestContext.Current.CancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            }
        }

        [Fact]
        public async Task GetGroupsForIndividualAsync_ReturnsGroups_WhenIndividualAndRelationshipTypeExist()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            Individual individual = TestHelper.GenerateIndividual();
            Group group = TestHelper.GenerateGroups(1).Single();
            RelationshipType relationshipType = new()
            {
                Id = Guid.NewGuid(),
                Name = "MemberOf",
                Description = "Indicates that an individual is a member of a group"
            };

            dbContext.Individual.Add(individual);
            dbContext.Group.Add(group);
            dbContext.RelationshipType.Add(relationshipType);
            dbContext.Party.Add(group.Party!);
            dbContext.PartyRelationship.Add(new PartyRelationship
            {
                Id = Guid.NewGuid(),
                FromPartyId = individual.PartyId,
                ToPartyId = group.Party!.Id,
                RelationshipTypeId = relationshipType.Id,
                StartDate = DateTime.UtcNow
            });

            dbContext.SaveChanges();

            _relationshipTypeLookupServiceMock
                .Setup(s => s.GetByNameAsync("MemberOf", It.IsAny<CancellationToken>()))
                .ReturnsAsync(relationshipType);

            IndividualRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Group>> result = await repository.GetGroupsForIndividualAsync(individual.Id, TestContext.Current.CancellationToken);

            // Assert
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
        public async Task GetGroupsForIndividualAsync_ReturnsNotFound_WhenIndividualDoesNotExist()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            IndividualRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Group>> result = await repository.GetGroupsForIndividualAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            }
        }

        [Fact]
        public async Task GetGroupsForIndividualAsync_ReturnsNotFound_WhenRelationshipTypeDoesNotExist()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            Individual individual = TestHelper.GenerateIndividual();
            dbContext.Individual.Add(individual);
            dbContext.SaveChanges();

            _relationshipTypeLookupServiceMock
                .Setup(s => s.GetByNameAsync("MemberOf", It.IsAny<CancellationToken>()))
                .ReturnsAsync((RelationshipType?)null);

            IndividualRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Group>> result = await repository.GetGroupsForIndividualAsync(individual.Id, TestContext.Current.CancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            }
        }

        [Fact]
        public async Task GetPartiesForIndividualAsync_ReturnsParties_WhenIndividualExists()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            Individual individual = TestHelper.GenerateIndividual();
            Party party = TestHelper.GenerateParty();

            dbContext.Individual.Add(individual);
            dbContext.Party.Add(party);
            dbContext.PartyRelationship.Add(new PartyRelationship
            {
                Id = Guid.NewGuid(),
                FromPartyId = individual.PartyId,
                ToPartyId = party.Id,
                StartDate = DateTime.UtcNow
            });

            dbContext.SaveChanges();

            IndividualRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Party>> result = await repository.GetPartiesForIndividualAsync(individual.Id, TestContext.Current.CancellationToken);

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
        public async Task GetPartiesForIndividualAsync_ReturnsNotFound_WhenIndividualDoesNotExist()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            IndividualRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Party>> result = await repository.GetPartiesForIndividualAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsIndividual_WithCorrectProperties_WhenIndividualExists()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            Individual individual = TestHelper.GenerateIndividual();
            dbContext.Individual.Add(individual);
            dbContext.SaveChanges();

            IndividualRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<Individual> result = await repository.GetByIdAsync(individual.Id, TestContext.Current.CancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Success.Should().BeTrue();
                result.Entity.Should().NotBeNull();
                result.Entity.Id.Should().Be(individual.Id);
                result.Entity.FirstName.Should().Be(individual.FirstName);
                result.Entity.LastName.Should().Be(individual.LastName);
                result.Entity.Party.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task GetOrganisationsForIndividualAsync_ReturnsOrganisations_WithCorrectProperties_WhenIndividualAndRelationshipTypeExist()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            Individual individual = TestHelper.GenerateIndividual();
            Organisation organisation = TestHelper.GenerateOrganisation();
            RelationshipType relationshipType = new()
            {
                Id = Guid.NewGuid(),
                Name = "EmployedBy",
                Description = "Indicates that an individual is employed by an organisation"
            };

            dbContext.Individual.Add(individual);
            dbContext.Organisation.Add(organisation);
            dbContext.RelationshipType.Add(relationshipType);
            dbContext.Party.Add(organisation.Party!);
            dbContext.PartyRelationship.Add(new PartyRelationship
            {
                Id = Guid.NewGuid(),
                FromPartyId = individual.PartyId,
                ToPartyId = organisation.Party!.Id,
                RelationshipTypeId = relationshipType.Id,
                StartDate = DateTime.UtcNow
            });

            dbContext.SaveChanges();

            _relationshipTypeLookupServiceMock
                .Setup(s => s.GetByNameAsync("EmployedBy", It.IsAny<CancellationToken>()))
                .ReturnsAsync(relationshipType);

            IndividualRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Organisation>> result = await repository.GetOrganisationsForIndividualAsync(individual.Id, TestContext.Current.CancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Success.Should().BeTrue();
                result.Entity.Should().NotBeNull();
                result.Entity.Should().ContainSingle();
                result.Entity[0].Id.Should().Be(organisation.Id);
                result.Entity[0].Name.Should().Be(organisation.Name);
                result.Entity[0].Party.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task GetGroupsForIndividualAsync_ReturnsGroups_WithCorrectProperties_WhenIndividualAndRelationshipTypeExist()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            Individual individual = TestHelper.GenerateIndividual();
            Group group = TestHelper.GenerateGroups(1).Single();
            RelationshipType relationshipType = new()
            {
                Id = Guid.NewGuid(),
                Name = "MemberOf",
                Description = "Indicates that an individual is a member of a group"
            };

            dbContext.Individual.Add(individual);
            dbContext.Group.Add(group);
            dbContext.RelationshipType.Add(relationshipType);
            dbContext.Party.Add(group.Party!);
            dbContext.PartyRelationship.Add(new PartyRelationship
            {
                Id = Guid.NewGuid(),
                FromPartyId = individual.PartyId,
                ToPartyId = group.Party!.Id,
                RelationshipTypeId = relationshipType.Id,
                StartDate = DateTime.UtcNow
            });

            dbContext.SaveChanges();

            _relationshipTypeLookupServiceMock
                .Setup(s => s.GetByNameAsync("MemberOf", It.IsAny<CancellationToken>()))
                .ReturnsAsync(relationshipType);

            IndividualRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Group>> result = await repository.GetGroupsForIndividualAsync(individual.Id, TestContext.Current.CancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Success.Should().BeTrue();
                result.Entity.Should().NotBeNull();
                result.Entity.Should().ContainSingle();
                result.Entity[0].Id.Should().Be(group.Id);
                result.Entity[0].Name.Should().Be(group.Name);
                result.Entity[0].Party.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task GetPartiesForIndividualAsync_ReturnsParties_WithCorrectProperties_WhenIndividualExists()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            Individual individual = TestHelper.GenerateIndividual();
            Party party = TestHelper.GenerateParty();

            dbContext.Individual.Add(individual);
            dbContext.Party.Add(party);
            dbContext.PartyRelationship.Add(new PartyRelationship
            {
                Id = Guid.NewGuid(),
                FromPartyId = individual.PartyId,
                ToPartyId = party.Id,
                StartDate = DateTime.UtcNow
            });

            dbContext.SaveChanges();

            IndividualRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Party>> result = await repository.GetPartiesForIndividualAsync(individual.Id, TestContext.Current.CancellationToken);

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
        public async Task GetByIdAsync_LogsWarning_WhenIndividualNotFound()
        {
            // Arrange
            using UserManagementDbContext dbContext = CreateDbContext();
            IndividualRepository repository = new(_loggerMock.Object, dbContext, _relationshipTypeLookupServiceMock.Object);
            Guid individualId = Guid.NewGuid();

            // Act
            OperationResult<Individual> result = await repository.GetByIdAsync(individualId, TestContext.Current.CancellationToken);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains($"Entity of type Individual with ID {individualId} not found")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetOrganisationsForIndividualAsync_LogsError_WhenQueryFails()
        {
            // Arrange
            Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
            dbContextMock.Setup(db => db.Individual).Throws(TestHelper.CreateSqlException("Sql Error"));

            Guid individualId = Guid.NewGuid();
            IndividualRepository repository = new(_loggerMock.Object, dbContextMock.Object, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Organisation>> result = await repository.GetOrganisationsForIndividualAsync(individualId, TestContext.Current.CancellationToken);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains("An error occurred while retrieving Organisations for Individual with ID")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetGroupsForIndividualAsync_LogsError_WhenQueryFails()
        {
            // Arrange
            Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
            dbContextMock.Setup(db => db.Individual).Throws(TestHelper.CreateSqlException("Sql Error"));

            Guid individualId = Guid.NewGuid();
            IndividualRepository repository = new(_loggerMock.Object, dbContextMock.Object, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Group>> result = await repository.GetGroupsForIndividualAsync(individualId, TestContext.Current.CancellationToken);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains("An error occurred while retrieving Groups for Individual with ID")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetPartiesForIndividualAsync_LogsError_WhenQueryFails()
        {
            // Arrange
            Mock<UserManagementDbContext> dbContextMock = new(new DbContextOptions<UserManagementDbContext>());
            dbContextMock.Setup(db => db.Individual).Throws(TestHelper.CreateSqlException("Sql Error"));

            Guid individualId = Guid.NewGuid();
            IndividualRepository repository = new(_loggerMock.Object, dbContextMock.Object, _relationshipTypeLookupServiceMock.Object);

            // Act
            OperationResult<IReadOnlyList<Party>> result = await repository.GetPartiesForIndividualAsync(individualId, TestContext.Current.CancellationToken);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v is object && v.ToString()!.Contains("An error occurred while retrieving Parties for Individual with ID")),
                    It.IsAny<SqlException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}