using FluentAssertions;
using FluentAssertions.Execution;

namespace INSS.Platform.Canonical.Domain.Tests;

public class IncomeTests
{
    [Fact]
    public void Income_CanBeInstantiated()
    {
        // Arrange & Act
        Income income = new ();

        // Assert
        using (new AssertionScope())
        {
            income.Should().NotBeNull();
            income.UserId.Should().Be(Guid.Empty);
            income.SourceOfIncome.Should().BeNull();
            income.GrossIncome.Should().Be(0m);
            income.PaymentFrequency.Should().BeNull();
            income.IncomeProvider.Should().BeNull();
            income.User.Should().BeNull();
        }
    }

    [Fact]
    public void Income_Properties_CanBeSet()
    {
        // Arrange
        Income income = new ();
        Guid userId = Guid.NewGuid();
        string sourceOfIncome = "Employment";
        decimal grossIncome = 2500.50m;
        string paymentFrequency = "Monthly";
        string incomeProvider = "ABC Corporation Ltd";

        // Act
        income.UserId = userId;
        income.SourceOfIncome = sourceOfIncome;
        income.GrossIncome = grossIncome;
        income.PaymentFrequency = paymentFrequency;
        income.IncomeProvider = incomeProvider;

        // Assert
        using (new AssertionScope())
        {
            income.UserId.Should().Be(userId);
            income.SourceOfIncome.Should().Be(sourceOfIncome);
            income.GrossIncome.Should().Be(grossIncome);
            income.PaymentFrequency.Should().Be(paymentFrequency);
            income.IncomeProvider.Should().Be(incomeProvider);
        }
    }

    [Fact]
    public void Income_User_CanBeSet()
    {
        // Arrange
        Income income = new ();
        User user = new ()
        {
            Id = Guid.NewGuid(),
            FullName = "Jane Smith"
        };

        // Act
        income.User = user;
        income.UserId = user.Id;

        // Assert
        using (new AssertionScope())
        {
            income.User.Should().NotBeNull();
            income.User.Should().Be(user);
            income.UserId.Should().Be(user.Id);
        }
    }

    [Fact]
    public void Income_InheritsFromBaseEntity()
    {
        // Arrange & Act
        Income income = new ();

        // Assert
        income.Should().BeAssignableTo<BaseEntity>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100.50)]
    [InlineData(2500.99)]
    [InlineData(50000.00)]
    public void Income_GrossIncome_AcceptsVariousAmounts(decimal amount)
    {
        // Arrange
        Income income = new()
        {
            // Act
            GrossIncome = amount
        };

        // Assert
        income.GrossIncome.Should().Be(amount);
    }

    [Theory]
    [InlineData("Weekly")]
    [InlineData("Monthly")]
    [InlineData("Annually")]
    [InlineData("Quarterly")]
    public void Income_PaymentFrequency_AcceptsVariousFrequencies(string frequency)
    {
        // Arrange
        Income income = new()
        {
            // Act
            PaymentFrequency = frequency
        };

        // Assert
        income.PaymentFrequency.Should().Be(frequency);
    }

    [Theory]
    [InlineData("Employment")]
    [InlineData("Pension")]
    [InlineData("Self-Employment")]
    [InlineData("Benefits")]
    public void Income_SourceOfIncome_AcceptsVariousSources(string source)
    {
        // Arrange
        Income income = new()
        {
            // Act
            SourceOfIncome = source
        };

        // Assert
        income.SourceOfIncome.Should().Be(source);
    }

    [Fact]
    public void Income_WithAllProperties_RetainsValues()
    {
        // Arrange & Act
        Income income = new ()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            SourceOfIncome = "Pension",
            GrossIncome = 1500.75m,
            PaymentFrequency = "Monthly",
            IncomeProvider = "State Pension",
            Created = DateTime.UtcNow,
            CreatedBy = "TestUser"
        };

        // Assert
        using (new AssertionScope())
        {
            income.Id.Should().NotBe(Guid.Empty);
            income.UserId.Should().NotBe(Guid.Empty);
            income.SourceOfIncome.Should().Be("Pension");
            income.GrossIncome.Should().Be(1500.75m);
            income.PaymentFrequency.Should().Be("Monthly");
            income.IncomeProvider.Should().Be("State Pension");
            income.Created.Should().NotBeNull();
            income.CreatedBy.Should().Be("TestUser");
        }
    }

    [Fact]
    public void Income_Navigation_MaintainsRelationship()
    {
        // Arrange
        User user = new()
        {
            Id = Guid.NewGuid(),
            FullName = "Test User"
        };

        Income income = new()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            SourceOfIncome = "Employment",
            GrossIncome = 3000m,
            PaymentFrequency = "Monthly",
            IncomeProvider = "Test Company",
            User = user
        };

        // Act
        user.Incomes.Add(income);

        // Assert
        using (new AssertionScope())
        {
            income.User.Should().Be(user);
            income.UserId.Should().Be(user.Id);
            user.Incomes.Should().Contain(income);
        }
    }

    [Fact]
    public void Income_GrossIncome_SupportsDecimalPrecision()
    {
        // Arrange
        Income income = new();
        decimal preciseAmount = 1234.567m;

        // Act
        income.GrossIncome = preciseAmount;

        // Assert
        income.GrossIncome.Should().Be(preciseAmount);
    }
}