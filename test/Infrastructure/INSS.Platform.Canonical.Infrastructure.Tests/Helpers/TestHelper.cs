using INSS.Platform.Canonical.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace INSS.Platform.Canonical.Infrastructure.Tests.Helpers;

internal static class TestHelper
{
    internal static IEnumerable<User> GenerateUsers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return GenerateUser(i);
        }
    }

    internal static User GenerateUser(int index = 1)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            FullName = $"Test User {index}",
            DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            TelephoneNumber = $"0123456789{index}",
            EmailAddress = $"user{index}@test.com",
            Addresses = new List<Address>(),
            Incomes = new List<Income>(),
            BankDetails = new List<BankDetails>()
        };
    }

    internal static Address GenerateAddress(Guid userId, int index = 1)
    {
        return new Address
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AddressLine1 = $"1 Test Street {index}",
            AddressLine2 = null,
            TownCity = "TestTown",
            County = null,
            Postcode = $"TE{index} 1ST"
        };
    }

    internal static BankDetails GenerateBankDetails(Guid userId, int index = 1)
    {
        return new BankDetails
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AccountName = $"Test User {index}",
            SortCode = "123456",
            AccountNumber = $"00000{index}",
            BuildingSocietyRollNumber = null
        };
    }

    internal static Income GenerateIncome(Guid userId, int index = 1)
    {
        return new Income
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SourceOfIncome = $"Job {index}",
            GrossIncome = 1000 + index,
            PaymentFrequency = "Monthly",
            IncomeProvider = $"Employer {index}"
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

    internal static CanonicalDbContext CreateDbContext()
    {
        DbContextOptions<CanonicalDbContext> options = new DbContextOptionsBuilder<CanonicalDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new CanonicalDbContext(options);
    }
}
