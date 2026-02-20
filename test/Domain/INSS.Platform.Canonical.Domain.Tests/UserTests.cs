using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.Events.Domain;

namespace INSS.Platform.Canonical.Domain.Tests;

public class UserTests
{
    [Fact]
    public void User_CanBeInstantiated()
    {
        // Arrange & Act
        User user = new ();

        // Assert
        using (new AssertionScope())
        {
            user.Should().NotBeNull();
            user.FullName.Should().BeNull();
            user.DateOfBirth.Should().Be(default);
            user.TelephoneNumber.Should().BeNull();
            user.EmailAddress.Should().BeNull();
            user.Addresses.Should().NotBeNull().And.BeEmpty();
            user.Incomes.Should().NotBeNull().And.BeEmpty();
            user.BankDetails.Should().NotBeNull().And.BeEmpty();
        }
    }

    [Fact]
    public void User_Properties_CanBeSet()
    {
        // Arrange
        User user = new ();
        string fullName = "John Smith";
        DateOnly dateOfBirth = new (1980, 5, 15);
        string telephoneNumber = "07700900123";
        string emailAddress = "john.smith@example.com";

        // Act
        user.FullName = fullName;
        user.DateOfBirth = dateOfBirth;
        user.TelephoneNumber = telephoneNumber;
        user.EmailAddress = emailAddress;

        // Assert
        using (new AssertionScope())
        {
            user.FullName.Should().Be(fullName);
            user.DateOfBirth.Should().Be(dateOfBirth);
            user.TelephoneNumber.Should().Be(telephoneNumber);
            user.EmailAddress.Should().Be(emailAddress);
        }
    }

    [Fact]
    public void User_Addresses_CanBePopulated()
    {
        // Arrange
        User user = new ();
        Address address = new ()
        {
            AddressLine1 = "123 Main Street",
            TownCity = "London",
            Postcode = "SW1A 1AA"
        };

        // Act
        user.Addresses.Add(address);

        // Assert
        using (new AssertionScope())
        {
            user.Addresses.Should().HaveCount(1);
            user.Addresses.Should().Contain(address);
        }
    }

    [Fact]
    public void User_Incomes_CanBePopulated()
    {
        // Arrange
        User user = new ();
        Income income = new ()
        {
            SourceOfIncome = "Employment",
            GrossIncome = 2500.00m,
            PaymentFrequency = "Monthly",
            IncomeProvider = "ABC Ltd"
        };

        // Act
        user.Incomes.Add(income);

        // Assert
        using (new AssertionScope())
        {
            user.Incomes.Should().HaveCount(1);
            user.Incomes.Should().Contain(income);
        }
    }

    [Fact]
    public void User_BankDetails_CanBePopulated()
    {
        // Arrange
        User user = new ();
        BankDetails bankDetails = new ()
        {
            AccountName = "John Smith",
            SortCode = "12-34-56",
            AccountNumber = "12345678"
        };

        // Act
        user.BankDetails.Add(bankDetails);

        // Assert
        using (new AssertionScope())
        {
            user.BankDetails.Should().HaveCount(1);
            user.BankDetails.Should().Contain(bankDetails);
        }
    }

    [Fact]
    public void UserDetailsAdded_RaisesDomainEvent()
    {
        // Arrange
        User user = new () { Id = Guid.NewGuid() };
        string actor = "TestUser";
        Guid correlationId = Guid.NewGuid();
        string fullName = "Jane Doe";
        DateOnly dateOfBirth = new (1990, 3, 20);
        string telephoneNumber = "07700900456";
        string emailAddress = "jane.doe@example.com";

        // Act
        user.UserDetailsAdded(actor, correlationId, fullName, dateOfBirth, telephoneNumber, emailAddress);

        // Assert
        using (new AssertionScope())
        {
            user.DomainEvents.Should().HaveCount(1);
            IDomainEvent domainEvent = user.DomainEvents[0];
            domainEvent.Should().BeOfType<UserDetailsAddedEvent>();
            
            UserDetailsAddedEvent userEvent = (UserDetailsAddedEvent)domainEvent;
            userEvent.Actor.Should().Be(actor);
            userEvent.AggregateRootId.Should().Be(user.Id);
            userEvent.CorrelationId.Should().Be(correlationId);
            userEvent.FullName.Should().Be(fullName);
            userEvent.DateOfBirth.Should().Be(dateOfBirth);
            userEvent.TelephoneNumber.Should().Be(telephoneNumber);
            userEvent.EmailAddress.Should().Be(emailAddress);
        }
    }

    [Fact]
    public void UserIncomeAdded_RaisesDomainEvent()
    {
        // Arrange
        User user = new () { Id = Guid.NewGuid() };
        string actor = "TestUser";
        Guid correlationId = Guid.NewGuid();
        decimal grossIncome = 3000.00m;
        string incomeProvider = "XYZ Corporation";

        // Act
        user.UserIncomeAdded(actor, correlationId, grossIncome, incomeProvider);

        // Assert
        using (new AssertionScope())
        {
            user.DomainEvents.Should().HaveCount(1);
            IDomainEvent domainEvent = user.DomainEvents[0];
            domainEvent.Should().BeOfType<UserIncomeAddedEvent>();
            
            UserIncomeAddedEvent incomeEvent = (UserIncomeAddedEvent)domainEvent;
            incomeEvent.Actor.Should().Be(actor);
            incomeEvent.AggregateRootId.Should().Be(user.Id);
            incomeEvent.CorrelationId.Should().Be(correlationId);
            incomeEvent.GrossIncome.Should().Be(grossIncome);
            incomeEvent.IncomeProvider.Should().Be(incomeProvider);
        }
    }

    [Fact]
    public void UserBankDetailsAdded_RaisesDomainEvent()
    {
        // Arrange
        User user = new () { Id = Guid.NewGuid() };
        string actor = "TestUser";
        Guid correlationId = Guid.NewGuid();
        string accountName = "Mr Test User";
        string sortCode = "20-00-00";

        // Act
        user.UserBankDetailsAdded(actor, correlationId, accountName, sortCode);

        // Assert
        using (new AssertionScope())
        {
            user.DomainEvents.Should().HaveCount(1);
            IDomainEvent domainEvent = user.DomainEvents[0];
            domainEvent.Should().BeOfType<UserBankDetailsAddedEvent>();
            
            UserBankDetailsAddedEvent bankEvent = (UserBankDetailsAddedEvent)domainEvent;
            bankEvent.Actor.Should().Be(actor);
            bankEvent.AggregateRootId.Should().Be(user.Id);
            bankEvent.CorrelationId.Should().Be(correlationId);
            bankEvent.AccountName.Should().Be(accountName);
            bankEvent.SortCode.Should().Be(sortCode);
        }
    }

    [Fact]
    public void User_CanRaiseMultipleDomainEvents()
    {
        // Arrange
        User user = new () { Id = Guid.NewGuid() };
        string actor = "TestUser";
        Guid correlationId = Guid.NewGuid();

        // Act
        user.UserDetailsAdded(actor, correlationId, "Name", new DateOnly(1985, 1, 1), "Phone", "Email");
        user.UserIncomeAdded(actor, correlationId, 2000m, "Provider");
        user.UserBankDetailsAdded(actor, correlationId, "Account", "SortCode");

        // Assert
        using (new AssertionScope())
        {
            user.DomainEvents.Should().HaveCount(3);
            user.DomainEvents[0].Should().BeOfType<UserDetailsAddedEvent>();
            user.DomainEvents[1].Should().BeOfType<UserIncomeAddedEvent>();
            user.DomainEvents[2].Should().BeOfType<UserBankDetailsAddedEvent>();
        }
    }

    [Fact]
    public void User_InheritsFromBaseEntity()
    {
        // Arrange & Act
        User user = new ();

        // Assert
        user.Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void User_WithMultipleAddresses_MaintainsCollection()
    {
        // Arrange
        User user = new ();
        Address address1 = new () { AddressLine1 = "Address 1", TownCity = "City1", Postcode = "PC1" };
        Address address2 = new () { AddressLine1 = "Address 2", TownCity = "City2", Postcode = "PC2" };

        // Act
        user.Addresses.Add(address1);
        user.Addresses.Add(address2);

        // Assert
        using (new AssertionScope())
        {
            user.Addresses.Should().HaveCount(2);
            user.Addresses.Should().Contain([address1, address2]);
        }
    }

    [Fact]
    public void User_WithMultipleIncomes_MaintainsCollection()
    {
        // Arrange
        User user = new ();
        Income income1 = new () { SourceOfIncome = "Job1", GrossIncome = 1000m, PaymentFrequency = "Monthly", IncomeProvider = "P1" };
        Income income2 = new () { SourceOfIncome = "Job2", GrossIncome = 500m, PaymentFrequency = "Weekly", IncomeProvider = "P2" };

        // Act
        user.Incomes.Add(income1);
        user.Incomes.Add(income2);

        // Assert
        using (new AssertionScope())
        {
            user.Incomes.Should().HaveCount(2);
            user.Incomes.Should().Contain([income1, income2]);
        }
    }

    [Fact]
    public void User_WithMultipleBankDetails_MaintainsCollection()
    {
        // Arrange
        User user = new ();
        BankDetails bank1 = new () { AccountName = "Account1", SortCode = "11-11-11", AccountNumber = "11111111" };
        BankDetails bank2 = new () { AccountName = "Account2", SortCode = "22-22-22", AccountNumber = "22222222" };

        // Act
        user.BankDetails.Add(bank1);
        user.BankDetails.Add(bank2);

        // Assert
        using (new AssertionScope())
        {
            user.BankDetails.Should().HaveCount(2);
            user.BankDetails.Should().Contain([bank1, bank2]);
        }
    }
}