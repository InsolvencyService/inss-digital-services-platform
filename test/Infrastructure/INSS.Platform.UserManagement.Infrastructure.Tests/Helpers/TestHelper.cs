using INSS.Platform.UserManagement.Domain;
using Microsoft.Data.SqlClient;
using System.Reflection;

namespace INSS.Platform.UserManagement.Infrastructure.Tests.Helpers;

/// <summary>
/// Provides helper methods for generating test data and simulating exceptions for unit tests.
/// </summary>
internal static class TestHelper
{
    /// <summary>
    /// Generates a collection of <see cref="Party"/> instances for testing purposes.
    /// </summary>
    /// <param name="count">The number of <see cref="Party"/> instances to generate. Defaults to 1.</param>
    /// <returns>An <see cref="IEnumerable{Party}"/> containing the generated parties.</returns>
    internal static IEnumerable<Party> GenerateParties(int count = 1)
    {
        for (int index = 0; index < count; index++)
        {
            yield return GenerateParty(index);
        }
    }

    /// <summary>
    /// Generates a single <see cref="Party"/> instance with test data.
    /// </summary>
    /// <param name="index">The index to use for generating unique property values. Defaults to 1.</param>
    /// <returns>A new <see cref="Party"/> instance populated with test data.</returns>
    internal static Party GenerateParty(int index = 1)
    {
        PartyType partyType = new()
        {
            Id = Guid.NewGuid(),
            Name = $"TestPartyType{index}",
            Description = $"TestDescription{index}",
            Created = DateTime.UtcNow,
            CreatedBy = $"UnitTestPartyTypeUser{index}",
            Modified = DateTime.UtcNow,
            ModifiedBy = $"UnitTestPartyTypeUser{index}"
        };

        return new Party
        {
            Id = Guid.NewGuid(),
            PartyTypeId = partyType.Id,
            PartyType = partyType,
            SourceOfIntroduction = $"TestSource{index}",
            Created = DateTime.UtcNow,
            CreatedBy = $"UnitTestPartyUser{index}",
            Modified = DateTime.UtcNow,
            ModifiedBy = $"UnitTestPartyUser{index}"
        };
    }

    /// <summary>
    /// Generates a collection of <see cref="Group"/> instances for testing purposes.
    /// </summary>
    /// <param name="count">The number of <see cref="Group"/> instances to generate.</param>
    /// <returns>An <see cref="IEnumerable{Group}"/> containing the generated groups.</returns>
    internal static IEnumerable<Group> GenerateGroups(int count)
    {
        for (int index = 0; index < count; index++)
        {
            Party party = GenerateParties().Single();

            yield return GenerateGroup(index, party);
        }
    }

    /// <summary>
    /// Generates a single <see cref="Group"/> instance with test data.
    /// </summary>
    /// <param name="index">The index to use for generating unique property values. Defaults to 1.</param>
    /// <param name="party">An optional <see cref="Party"/> to associate with the group. If null, a new party is generated.</param>
    /// <returns>A new <see cref="Group"/> instance populated with test data.</returns>
    internal static Group GenerateGroup(int index = 1, Party? party = null)
    {
        party ??= GenerateParties().Single();

        return new Group
        {
            Id = Guid.NewGuid(),
            PartyId = party.Id,
            Party = party,
            Name = $"TestGroup{index}",
            Description = $"TestDescription{index}",
            Created = DateTime.UtcNow,
            CreatedBy = $"UnitTestGroupUser{index}",
            Modified = DateTime.UtcNow,
            ModifiedBy = $"UnitTestGroupUser{index}"
        };
    }

    /// <summary>
    /// Generates a collection of <see cref="Individual"/> instances for testing purposes.
    /// </summary>
    /// <param name="count">The number of <see cref="Individual"/> instances to generate.</param>
    /// <returns>An <see cref="IEnumerable{Individual}"/> containing the generated individuals.</returns>
    internal static IEnumerable<Individual> GenerateIndividuals(int count)
    {
        for (int index = 0; index < count; index++)
        {
            Party party = GenerateParties().Single();
            yield return GenerateIndividual(index, party);
        }
    }

    /// <summary>
    /// Generates a single <see cref="Individual"/> instance with test data.
    /// </summary>
    /// <param name="index">The index to use for generating unique property values. Defaults to 1.</param>
    /// <param name="party">An optional <see cref="Party"/> to associate with the individual. If null, a new party is generated.</param>
    /// <returns>A new <see cref="Individual"/> instance populated with test data.</returns>
    internal static Individual GenerateIndividual(int index = 1, Party? party = null)
    {
        party ??= GenerateParties().Single();

        return new Individual
        {
            Id = Guid.NewGuid(),
            PartyId = party.Id,
            Party = party,
            FirstName = $"Test{index}",
            LastName = $"User{index}",
            Created = DateTime.UtcNow,
            CreatedBy = $"UnitTestUser{index}",
            Modified = DateTime.UtcNow,
            ModifiedBy = $"UnitTestUser{index}"
        };
    }

    /// <summary>
    /// Generates a collection of <see cref="Organisation"/> instances for testing purposes.
    /// </summary>
    /// <param name="count">The number of <see cref="Organisation"/> instances to generate.</param>
    /// <returns>An <see cref="IEnumerable{Organisation}"/> containing the generated organisations.</returns>
    internal static IEnumerable<Organisation> GenerateOrganisations(int count)
    {
        for (int index = 0; index < count; index++)
        {
            Party party = GenerateParties().Single();

            yield return GenerateOrganisation(index, party);
        }
    }

    /// <summary>
    /// Generates a test <see cref="Organisation"/> instance with the specified index and optional <see cref="Party"/>.
    /// </summary>
    /// <param name="index">The index to use for generating unique property values. Defaults to 1.</param>
    /// <param name="party">An optional <see cref="Party"/> to associate with the organisation. If null, a new party is generated.</param>
    /// <returns>A new <see cref="Organisation"/> instance populated with test data.</returns>
    internal static Organisation GenerateOrganisation(int index = 1, Party? party = null)
    {
        party ??= GenerateParties().Single();

        return new Organisation
        {
            Id = Guid.NewGuid(),
            PartyId = party.Id,
            Party = party,
            Name = $"TestOrganisation{index}",
            Description = $"TestDescription{index}",
            Created = DateTime.UtcNow,
            CreatedBy = $"UnitTestOrganisationUser{index}",
            Modified = DateTime.UtcNow,
            ModifiedBy = $"UnitTestOrganisationUser{index}"
        };
    }

    /// <summary>
    /// Generates a collection of <see cref="TestEntity"/> instances for testing purposes.
    /// </summary>
    /// <param name="count">The number of <see cref="TestEntity"/> instances to generate.</param>
    /// <returns>An <see cref="IEnumerable{TestEntity}"/> containing the generated test entities.</returns>
    internal static IEnumerable<TestEntity> GenerateTestEntities(int count)
    {
        for (int index = 0; index < count; index++)
        {
            yield return GenerateTestEntity(index);
        }
    }

    /// <summary>
    /// Generates a single <see cref="TestEntity"/> instance with test data.
    /// </summary>
    /// <param name="index">The index to use for generating unique property values. Defaults to 1.</param>
    /// <returns>A new <see cref="TestEntity"/> instance populated with test data.</returns>
    internal static TestEntity GenerateTestEntity(int index = 1, string? name = null)
    {
        return new TestEntity
        {
            Id = Guid.NewGuid(),
            Name = name ?? $"TestEntity{index}",
            Description = $"TestDescription{index}",
            Created = DateTime.UtcNow,
            CreatedBy = $"UnitTestUser{index}",
            Modified = DateTime.UtcNow,
            ModifiedBy = $"UnitTestUser{index}"
        };
    }

    /// <summary>
    /// Creates a <see cref="SqlException"/> instance with a custom message for testing purposes.
    /// </summary>
    /// <param name="message">The error message for the <see cref="SqlException"/>.</param>
    /// <returns>A <see cref="SqlException"/> instance with the specified message.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the required constructor cannot be found via reflection.</exception>
    internal static SqlException CreateSqlException(string message)
    {
        ConstructorInfo? sqlErrorCollectionConstructor = typeof(SqlErrorCollection)
            .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)
            ?? throw new InvalidOperationException("Could not find SqlErrorCollection constructor.");

        object sqlErrorCollection = sqlErrorCollectionConstructor.Invoke(null);

        ConstructorInfo sqlExceptionConstructor = typeof(SqlException)
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
            .First(c =>
            {
                ParameterInfo[] parameters = c.GetParameters();
                return parameters.Length > 0 && parameters[0].ParameterType == typeof(string);
            });

        SqlException sqlException = (SqlException)sqlExceptionConstructor.Invoke([message, sqlErrorCollection, null, Guid.NewGuid()]);
        return sqlException;
    }
}
