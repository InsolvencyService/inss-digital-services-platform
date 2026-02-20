using FluentAssertions;
using FluentAssertions.Execution;

namespace INSS.Platform.Canonical.Domain.Tests;

public class AddressTests
{
    [Fact]
    public void Address_CanBeInstantiated()
    {
        // Arrange & Act
        Address address = new ();

        // Assert
        using (new AssertionScope())
        {
            address.Should().NotBeNull();
            address.UserId.Should().Be(Guid.Empty);
            address.AddressLine1.Should().BeNull();
            address.AddressLine2.Should().BeNull();
            address.TownCity.Should().BeNull();
            address.County.Should().BeNull();
            address.Postcode.Should().BeNull();
            address.User.Should().BeNull();
        }
    }

    [Fact]
    public void Address_Properties_CanBeSet()
    {
        // Arrange
        Address address = new ();
        Guid userId = Guid.NewGuid();
        string addressLine1 = "123 Main Street";
        string addressLine2 = "Apartment 4B";
        string townCity = "London";
        string county = "Greater London";
        string postcode = "SW1A 1AA";

        // Act
        address.UserId = userId;
        address.AddressLine1 = addressLine1;
        address.AddressLine2 = addressLine2;
        address.TownCity = townCity;
        address.County = county;
        address.Postcode = postcode;

        // Assert
        using (new AssertionScope())
        {
            address.UserId.Should().Be(userId);
            address.AddressLine1.Should().Be(addressLine1);
            address.AddressLine2.Should().Be(addressLine2);
            address.TownCity.Should().Be(townCity);
            address.County.Should().Be(county);
            address.Postcode.Should().Be(postcode);
        }
    }

    [Fact]
    public void Address_WithNullOptionalFields_IsValid()
    {
        // Arrange
        Address address = new ();
        Guid userId = Guid.NewGuid();
        string addressLine1 = "456 High Street";
        string townCity = "Manchester";
        string postcode = "M1 1AA";

        // Act
        address.UserId = userId;
        address.AddressLine1 = addressLine1;
        address.AddressLine2 = null;
        address.TownCity = townCity;
        address.County = null;
        address.Postcode = postcode;

        // Assert
        using (new AssertionScope())
        {
            address.AddressLine2.Should().BeNull();
            address.County.Should().BeNull();
            address.AddressLine1.Should().Be(addressLine1);
            address.TownCity.Should().Be(townCity);
            address.Postcode.Should().Be(postcode);
        }
    }

    [Fact]
    public void Address_User_CanBeSet()
    {
        // Arrange
        Address address = new ();
        User user = new ()
        {
            Id = Guid.NewGuid(),
            FullName = "John Doe"
        };

        // Act
        address.User = user;
        address.UserId = user.Id;

        // Assert
        using (new AssertionScope())
        {
            address.User.Should().NotBeNull();
            address.User.Should().Be(user);
            address.UserId.Should().Be(user.Id);
        }
    }

    [Fact]
    public void Address_InheritsFromBaseEntity()
    {
        // Arrange & Act
        Address address = new ();

        // Assert
        address.Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void Address_WithAllProperties_RetainsValues()
    {
        // Arrange & Act
        Address address = new ()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            AddressLine1 = "789 Park Road",
            AddressLine2 = "Suite 10",
            TownCity = "Birmingham",
            County = "West Midlands",
            Postcode = "B1 1AA",
            Created = DateTime.UtcNow,
            CreatedBy = "TestUser"
        };

        // Assert
        using (new AssertionScope())
        {
            address.Id.Should().NotBe(Guid.Empty);
            address.UserId.Should().NotBe(Guid.Empty);
            address.AddressLine1.Should().Be("789 Park Road");
            address.AddressLine2.Should().Be("Suite 10");
            address.TownCity.Should().Be("Birmingham");
            address.County.Should().Be("West Midlands");
            address.Postcode.Should().Be("B1 1AA");
            address.Created.Should().NotBeNull();
            address.CreatedBy.Should().Be("TestUser");
        }
    }

    [Theory]
    [InlineData("SW1A 1AA")]
    [InlineData("M1 1AA")]
    [InlineData("B1 1AA")]
    [InlineData("EC1A 1BB")]
    public void Address_Postcode_AcceptsVariousFormats(string postcode)
    {
        // Arrange
        Address address = new()
        {
            // Act
            Postcode = postcode
        };

        // Assert
        address.Postcode.Should().Be(postcode);
    }

    [Fact]
    public void Address_Navigation_MaintainsRelationship()
    {
        // Arrange
        User user = new ()
        {
            Id = Guid.NewGuid(),
            FullName = "Test User"
        };

        Address address = new ()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            AddressLine1 = "Test Street",
            TownCity = "Test City",
            Postcode = "TE1 1ST",
            User = user
        };

        // Act
        user.Addresses.Add(address);

        // Assert
        using (new AssertionScope())
        {
            address.User.Should().Be(user);
            address.UserId.Should().Be(user.Id);
            user.Addresses.Should().Contain(address);
        }
    }
}