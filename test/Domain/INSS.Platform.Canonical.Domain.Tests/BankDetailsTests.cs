using FluentAssertions;
using FluentAssertions.Execution;

namespace INSS.Platform.Canonical.Domain.Tests;

public class BankDetailsTests
{
    [Fact]
    public void BankDetails_CanBeInstantiated()
    {
        // Arrange & Act
        BankDetails bankDetails = new();

        // Assert
        using (new AssertionScope())
        {
            bankDetails.Should().NotBeNull();
            bankDetails.UserId.Should().Be(Guid.Empty);
            bankDetails.AccountName.Should().BeNull();
            bankDetails.SortCode.Should().BeNull();
            bankDetails.AccountNumber.Should().BeNull();
            bankDetails.BuildingSocietyRollNumber.Should().BeNull();
            bankDetails.User.Should().BeNull();
        }
    }

    [Fact]
    public void BankDetails_Properties_CanBeSet()
    {
        // Arrange
        BankDetails bankDetails = new();
        Guid userId = Guid.NewGuid();
        string accountName = "Mr John Smith";
        string sortCode = "12-34-56";
        string accountNumber = "12345678";
        string buildingSocietyRollNumber = "ABC123/456";

        // Act
        bankDetails.UserId = userId;
        bankDetails.AccountName = accountName;
        bankDetails.SortCode = sortCode;
        bankDetails.AccountNumber = accountNumber;
        bankDetails.BuildingSocietyRollNumber = buildingSocietyRollNumber;

        // Assert
        using (new AssertionScope())
        {
            bankDetails.UserId.Should().Be(userId);
            bankDetails.AccountName.Should().Be(accountName);
            bankDetails.SortCode.Should().Be(sortCode);
            bankDetails.AccountNumber.Should().Be(accountNumber);
            bankDetails.BuildingSocietyRollNumber.Should().Be(buildingSocietyRollNumber);
        }
    }

    [Fact]
    public void BankDetails_WithNullBuildingSocietyRollNumber_IsValid()
    {
        // Arrange
        BankDetails bankDetails = new();
        Guid userId = Guid.NewGuid();
        string accountName = "Ms Jane Doe";
        string sortCode = "20-00-00";
        string accountNumber = "87654321";

        // Act
        bankDetails.UserId = userId;
        bankDetails.AccountName = accountName;
        bankDetails.SortCode = sortCode;
        bankDetails.AccountNumber = accountNumber;
        bankDetails.BuildingSocietyRollNumber = null;

        // Assert
        using (new AssertionScope())
        {
            bankDetails.BuildingSocietyRollNumber.Should().BeNull();
            bankDetails.AccountName.Should().Be(accountName);
            bankDetails.SortCode.Should().Be(sortCode);
            bankDetails.AccountNumber.Should().Be(accountNumber);
        }
    }

    [Fact]
    public void BankDetails_User_CanBeSet()
    {
        // Arrange
        BankDetails bankDetails = new();
        User user = new()
        {
            Id = Guid.NewGuid(),
            FullName = "Test User"
        };

        // Act
        bankDetails.User = user;
        bankDetails.UserId = user.Id;

        // Assert
        using (new AssertionScope())
        {
            bankDetails.User.Should().NotBeNull();
            bankDetails.User.Should().Be(user);
            bankDetails.UserId.Should().Be(user.Id);
        }
    }

    [Fact]
    public void BankDetails_InheritsFromBaseEntity()
    {
        // Arrange & Act
        BankDetails bankDetails = new();

        // Assert
        bankDetails.Should().BeAssignableTo<BaseEntity>();
    }

    [Theory]
    [InlineData("12-34-56")]
    [InlineData("20-00-00")]
    [InlineData("40-47-84")]
    [InlineData("08-92-99")]
    public void BankDetails_SortCode_AcceptsVariousFormats(string sortCode)
    {
        // Arrange
        BankDetails bankDetails = new()
        {
            // Act
            SortCode = sortCode
        };

        // Assert
        bankDetails.SortCode.Should().Be(sortCode);
    }

    [Theory]
    [InlineData("12345678")]
    [InlineData("87654321")]
    [InlineData("00000000")]
    [InlineData("99999999")]
    public void BankDetails_AccountNumber_AcceptsVariousNumbers(string accountNumber)
    {
        // Arrange
        BankDetails bankDetails = new()
        {
            // Act
            AccountNumber = accountNumber
        };

        // Assert
        bankDetails.AccountNumber.Should().Be(accountNumber);
    }

    [Fact]
    public void BankDetails_WithAllProperties_RetainsValues()
    {
        // Arrange & Act
        BankDetails bankDetails = new()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            AccountName = "Dr Test Person",
            SortCode = "11-22-33",
            AccountNumber = "11223344",
            BuildingSocietyRollNumber = "BS-123/456",
            Created = DateTime.UtcNow,
            CreatedBy = "TestUser"
        };

        // Assert
        using (new AssertionScope())
        {
            bankDetails.Id.Should().NotBe(Guid.Empty);
            bankDetails.UserId.Should().NotBe(Guid.Empty);
            bankDetails.AccountName.Should().Be("Dr Test Person");
            bankDetails.SortCode.Should().Be("11-22-33");
            bankDetails.AccountNumber.Should().Be("11223344");
            bankDetails.BuildingSocietyRollNumber.Should().Be("BS-123/456");
            bankDetails.Created.Should().NotBeNull();
            bankDetails.CreatedBy.Should().Be("TestUser");
        }
    }

    [Fact]
    public void BankDetails_Navigation_MaintainsRelationship()
    {
        // Arrange
        User user = new()
        {
            Id = Guid.NewGuid(),
            FullName = "Test User"
        };

        BankDetails bankDetails = new()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            AccountName = "Test User",
            SortCode = "12-34-56",
            AccountNumber = "12345678",
            User = user
        };

        // Act
        user.BankDetails.Add(bankDetails);

        // Assert
        using (new AssertionScope())
        {
            bankDetails.User.Should().Be(user);
            bankDetails.UserId.Should().Be(user.Id);
            user.BankDetails.Should().Contain(bankDetails);
        }
    }

    [Fact]
    public void BankDetails_MultipleBankDetailsForSameUser_MaintainsSeparateInstances()
    {
        // Arrange
        User user = new() { Id = Guid.NewGuid() };
        BankDetails bank1 = new()
        {
            UserId = user.Id,
            AccountName = "Current Account",
            SortCode = "11-11-11",
            AccountNumber = "11111111"
        };
        BankDetails bank2 = new()
        {
            UserId = user.Id,
            AccountName = "Savings Account",
            SortCode = "22-22-22",
            AccountNumber = "22222222"
        };

        // Act
        user.BankDetails.Add(bank1);
        user.BankDetails.Add(bank2);

        // Assert
        using (new AssertionScope())
        {
            user.BankDetails.Should().HaveCount(2);
            bank1.Should().NotBeSameAs(bank2);
            bank1.AccountName.Should().Be("Current Account");
            bank2.AccountName.Should().Be("Savings Account");
        }
    }
}