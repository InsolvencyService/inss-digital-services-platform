namespace INSS.Platform.UserManagement.Domain.Tests;

public class DomainEntityTests
{
    // AuditableEntity Tests
    [Fact]
    public void AuditableEntity_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        TestAuditableEntity entity = new();

        // Assert
        Assert.Equal(Guid.Empty, entity.Id);
        Assert.Null(entity.Created);
        Assert.Equal(string.Empty, entity.CreatedBy);
        Assert.Null(entity.Modified);
        Assert.Null(entity.ModifiedBy);
    }

    [Fact]
    public void AuditableEntity_AllProperties_CanBeSet()
    {
        // Arrange
        TestAuditableEntity entity = new();
        Guid id = Guid.NewGuid();
        DateTime created = DateTime.UtcNow;
        string createdBy = "TestUser";
        DateTime modified = DateTime.UtcNow;
        string modifiedBy = "ModifiedUser";

        // Act
        entity.Id = id;
        entity.Created = created;
        entity.CreatedBy = createdBy;
        entity.Modified = modified;
        entity.ModifiedBy = modifiedBy;

        // Assert
        Assert.Equal(id, entity.Id);
        Assert.Equal(created, entity.Created);
        Assert.Equal(createdBy, entity.CreatedBy);
        Assert.Equal(modified, entity.Modified);
        Assert.Equal(modifiedBy, entity.ModifiedBy);
    }

    // Party Tests
    [Fact]
    public void Party_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        Party party = new();

        // Assert
        Assert.Equal(Guid.Empty, party.PartyTypeId);
        Assert.Null(party.SourceOfIntroduction);
        Assert.Null(party.PartyType);
        Assert.NotNull(party.Addresses);
        Assert.Empty(party.Addresses);
        Assert.NotNull(party.RelationshipsFrom);
        Assert.Empty(party.RelationshipsFrom);
        Assert.NotNull(party.RelationshipsTo);
        Assert.Empty(party.RelationshipsTo);
    }

    [Fact]
    public void Party_Properties_CanBeSet()
    {
        // Arrange
        Party party = new();
        Guid partyTypeId = Guid.NewGuid();
        string source = "TestSource";

        // Act
        party.PartyTypeId = partyTypeId;
        party.SourceOfIntroduction = source;

        // Assert
        Assert.Equal(partyTypeId, party.PartyTypeId);
        Assert.Equal(source, party.SourceOfIntroduction);
    }

    [Fact]
    public void Party_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        Party party = new();

        // Assert
        Assert.IsType<AuditableEntity>(party, exactMatch: false);
    }

    // PartyType Tests
    [Fact]
    public void PartyType_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        PartyType partyType = new();

        // Assert
        Assert.Null(partyType.Name);
        Assert.Null(partyType.Description);
    }

    [Fact]
    public void PartyType_Properties_CanBeSet()
    {
        // Arrange
        PartyType partyType = new();
        string name = "Individual";
        string description = "Individual party type";

        // Act
        partyType.Name = name;
        partyType.Description = description;

        // Assert
        Assert.Equal(name, partyType.Name);
        Assert.Equal(description, partyType.Description);
    }

    [Fact]
    public void PartyType_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        PartyType partyType = new();

        // Assert
        Assert.IsType<AuditableEntity>(partyType, exactMatch: false);
    }

    // Address Tests
    [Fact]
    public void Address_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        Address address = new();

        // Assert
        Assert.Equal(Guid.Empty, address.PartyId);
        Assert.Equal(Guid.Empty, address.AddressTypeId);
        Assert.Null(address.AddressLine1);
        Assert.Null(address.AddressLine2);
        Assert.Null(address.AddressLine3);
        Assert.Null(address.Postcode);
        Assert.Null(address.UPRN);
        Assert.Null(address.Longitude);
        Assert.Null(address.Latitude);
        Assert.Null(address.Party);
    }

    [Fact]
    public void Address_RequiredProperties_CanBeSet()
    {
        // Arrange
        Address address = new();
        Guid partyId = Guid.NewGuid();
        Guid addressTypeId = Guid.NewGuid();
        string line1 = "123 Main Street";
        string postcode = "SW1A 1AA";

        // Act
        address.PartyId = partyId;
        address.AddressTypeId = addressTypeId;
        address.AddressLine1 = line1;
        address.Postcode = postcode;

        // Assert
        Assert.Equal(partyId, address.PartyId);
        Assert.Equal(addressTypeId, address.AddressTypeId);
        Assert.Equal(line1, address.AddressLine1);
        Assert.Equal(postcode, address.Postcode);
    }

    [Fact]
    public void Address_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        Address address = new();

        // Assert
        Assert.IsType<AuditableEntity>(address, exactMatch: false);
    }

    // AddressType Tests
    [Fact]
    public void AddressType_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        AddressType addressType = new();

        // Assert
        Assert.Null(addressType.Name);
        Assert.Null(addressType.Description);
    }

    [Fact]
    public void AddressType_Properties_CanBeSet()
    {
        // Arrange
        AddressType addressType = new();
        string name = "Home";
        string description = "Primary residence address";

        // Act
        addressType.Name = name;
        addressType.Description = description;

        // Assert
        Assert.Equal(name, addressType.Name);
        Assert.Equal(description, addressType.Description);
    }

    [Fact]
    public void AddressType_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        AddressType addressType = new();

        // Assert
        Assert.IsType<AuditableEntity>(addressType, exactMatch: false);
    }

    // RelationshipType Tests
    [Fact]
    public void RelationshipType_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        RelationshipType relationshipType = new();

        // Assert
        Assert.Null(relationshipType.Name);
        Assert.Null(relationshipType.Description);
    }

    [Fact]
    public void RelationshipType_Properties_CanBeSet()
    {
        // Arrange
        RelationshipType relationshipType = new();
        string name = "Parent-Child";
        string description = "Parent to child relationship";

        // Act
        relationshipType.Name = name;
        relationshipType.Description = description;

        // Assert
        Assert.Equal(name, relationshipType.Name);
        Assert.Equal(description, relationshipType.Description);
    }

    [Fact]
    public void RelationshipType_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        RelationshipType relationshipType = new();

        // Assert
        Assert.IsType<AuditableEntity>(relationshipType, exactMatch: false);
    }

    // PartyRelationship Tests
    [Fact]
    public void PartyRelationship_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        PartyRelationship relationship = new();

        // Assert
        Assert.Equal(Guid.Empty, relationship.FromPartyId);
        Assert.Equal(Guid.Empty, relationship.ToPartyId);
        Assert.Equal(Guid.Empty, relationship.RelationshipTypeId);
        Assert.Equal(default, relationship.StartDate);
        Assert.Null(relationship.EndDate);
        Assert.Null(relationship.FromParty);
        Assert.Null(relationship.ToParty);
        Assert.Null(relationship.RelationshipType);
    }

    [Fact]
    public void PartyRelationship_Properties_CanBeSet()
    {
        // Arrange
        PartyRelationship relationship = new();
        Guid fromPartyId = Guid.NewGuid();
        Guid toPartyId = Guid.NewGuid();
        Guid relationshipTypeId = Guid.NewGuid();
        DateTime startDate = DateTime.UtcNow;

        // Act
        relationship.FromPartyId = fromPartyId;
        relationship.ToPartyId = toPartyId;
        relationship.RelationshipTypeId = relationshipTypeId;
        relationship.StartDate = startDate;

        // Assert
        Assert.Equal(fromPartyId, relationship.FromPartyId);
        Assert.Equal(toPartyId, relationship.ToPartyId);
        Assert.Equal(relationshipTypeId, relationship.RelationshipTypeId);
        Assert.Equal(startDate, relationship.StartDate);
    }

    [Fact]
    public void PartyRelationship_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        PartyRelationship relationship = new();

        // Assert
        Assert.IsType<AuditableEntity>(relationship, exactMatch: false);
    }

    // AuthenticationPolicy Tests
    [Fact]
    public void AuthenticationPolicy_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        AuthenticationPolicy policy = new();

        // Assert
        Assert.Null(policy.Name);
        Assert.Null(policy.Description);
        Assert.False(policy.RequireMultiFactorAuthentication);
        Assert.False(policy.RequireIdentityVerification);
    }

    [Fact]
    public void AuthenticationPolicy_Properties_CanBeSet()
    {
        // Arrange
        AuthenticationPolicy policy = new();
        string name = "HighSecurity";
        string description = "High security authentication policy";
        bool requireMfa = true;
        bool requireIdVerification = true;

        // Act
        policy.Name = name;
        policy.Description = description;
        policy.RequireMultiFactorAuthentication = requireMfa;
        policy.RequireIdentityVerification = requireIdVerification;

        // Assert
        Assert.Equal(name, policy.Name);
        Assert.Equal(description, policy.Description);
        Assert.Equal(requireMfa, policy.RequireMultiFactorAuthentication);
        Assert.Equal(requireIdVerification, policy.RequireIdentityVerification);
    }

    [Fact]
    public void AuthenticationPolicy_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        AuthenticationPolicy policy = new();

        // Assert
        Assert.IsType<AuditableEntity>(policy, exactMatch: false);
    }

    [Fact]
    public void AuthenticationPolicyProvider_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        AuthenticationPolicyProvider policyProvider = new();

        // Assert
        Assert.Equal(Guid.Empty, policyProvider.AuthenticationPolicyId);
        Assert.Equal(Guid.Empty, policyProvider.AuthenticationProviderId);
    }

    [Fact]
    public void AuthenticationPolicyProvider_Properties_CanBeSet()
    {
        // Arrange
        AuthenticationPolicyProvider policyProvider = new();
        Guid policyId = Guid.NewGuid();
        Guid providerId = Guid.NewGuid();

        // Act
        policyProvider.AuthenticationPolicyId = policyId;
        policyProvider.AuthenticationProviderId = providerId;

        // Assert
        Assert.Equal(policyId, policyProvider.AuthenticationPolicyId);
        Assert.Equal(providerId, policyProvider.AuthenticationProviderId);
    }

    [Fact]
    public void AuthenticationPolicyProvider_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        AuthenticationPolicyProvider policyProvider = new();

        // Assert
        Assert.IsType<AuditableEntity>(policyProvider, exactMatch: false);
    }

    [Fact]
    public void AuthenticationProvider_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        AuthenticationProvider provider = new();

        // Assert
        Assert.Null(provider.Name);
        Assert.Null(provider.Description);
    }

    [Fact]
    public void AuthenticationProvider_Properties_CanBeSet()
    {
        // Arrange
        AuthenticationProvider provider = new();
        string name = "OneLogin";
        string description = "GOV.UK One Login provider";

        // Act
        provider.Name = name;
        provider.Description = description;

        // Assert
        Assert.Equal(name, provider.Name);
        Assert.Equal(description, provider.Description);
    }

    [Fact]
    public void AuthenticationProvider_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        AuthenticationProvider provider = new();

        // Assert
        Assert.IsType<AuditableEntity>(provider, exactMatch: false);
    }

    [Fact]
    public void AuthenticationProviderMetadata_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        AuthenticationProviderMetadata metadata = new();

        // Assert
        Assert.Equal(Guid.Empty, metadata.AuthenticationProviderId);
        Assert.Null(metadata.ClientId);
        Assert.Null(metadata.Secret);
        Assert.Null(metadata.AuthorizeEndpoint);
        Assert.Null(metadata.TokenEndpoint);
    }

    [Fact]
    public void AuthenticationProviderMetadata_Properties_CanBeSet()
    {
        // Arrange
        AuthenticationProviderMetadata metadata = new();
        Guid providerId = Guid.NewGuid();
        string clientId = "test-client-id";
        string secret = "test-secret";
        string authorizeEndpoint = "https://auth.example.com/authorize";
        string tokenEndpoint = "https://auth.example.com/token";

        // Act
        metadata.AuthenticationProviderId = providerId;
        metadata.ClientId = clientId;
        metadata.Secret = secret;
        metadata.AuthorizeEndpoint = authorizeEndpoint;
        metadata.TokenEndpoint = tokenEndpoint;

        // Assert
        Assert.Equal(providerId, metadata.AuthenticationProviderId);
        Assert.Equal(clientId, metadata.ClientId);
        Assert.Equal(secret, metadata.Secret);
        Assert.Equal(authorizeEndpoint, metadata.AuthorizeEndpoint);
        Assert.Equal(tokenEndpoint, metadata.TokenEndpoint);
    }

    [Fact]
    public void AuthenticationProviderMetadata_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        AuthenticationProviderMetadata metadata = new();

        // Assert
        Assert.IsType<AuditableEntity>(metadata, exactMatch: false);
    }

    [Fact]
    public void Group_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        Group group = new();

        // Assert
        Assert.Equal(Guid.Empty, group.PartyId);
        Assert.Null(group.Name);
        Assert.Null(group.Description);
        Assert.Null(group.Party);
    }

    [Fact]
    public void Group_Properties_CanBeSet()
    {
        // Arrange
        Group group = new();
        Guid partyId = Guid.NewGuid();
        string name = "Administrators";
        string description = "Administrator group";

        // Act
        group.PartyId = partyId;
        group.Name = name;
        group.Description = description;

        // Assert
        Assert.Equal(partyId, group.PartyId);
        Assert.Equal(name, group.Name);
        Assert.Equal(description, group.Description);
    }

    [Fact]
    public void Group_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        Group group = new();

        // Assert
        Assert.IsType<AuditableEntity>(group, exactMatch: false);
    }

    [Fact]
    public void Individual_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        Individual individual = new();

        // Assert
        Assert.Equal(Guid.Empty, individual.PartyId);
        Assert.Null(individual.FirstName);
        Assert.Null(individual.LastName);
        Assert.Null(individual.DateOfBirth);
        Assert.Null(individual.NationalInsuranceNumber);
        Assert.False(individual.IsIdentityVerified);
        Assert.Null(individual.VerificationSource);
        Assert.Null(individual.Party);
    }

    [Fact]
    public void Individual_Properties_CanBeSet()
    {
        // Arrange
        Individual individual = new();
        Guid partyId = Guid.NewGuid();
        string firstName = "John";
        string lastName = "Doe";
        DateOnly dateOfBirth = new (1990, 1, 1);
        string nino = "AB123456C";
        bool isVerified = true;
        string verificationSource = "GOV.UK Verify";

        // Act
        individual.PartyId = partyId;
        individual.FirstName = firstName;
        individual.LastName = lastName;
        individual.DateOfBirth = dateOfBirth;
        individual.NationalInsuranceNumber = nino;
        individual.IsIdentityVerified = isVerified;
        individual.VerificationSource = verificationSource;

        // Assert
        Assert.Equal(partyId, individual.PartyId);
        Assert.Equal(firstName, individual.FirstName);
        Assert.Equal(lastName, individual.LastName);
        Assert.Equal(dateOfBirth, individual.DateOfBirth);
        Assert.Equal(nino, individual.NationalInsuranceNumber);
        Assert.Equal(isVerified, individual.IsIdentityVerified);
        Assert.Equal(verificationSource, individual.VerificationSource);
    }

    [Fact]
    public void Individual_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        Individual individual = new();

        // Assert
        Assert.IsType<AuditableEntity>(individual, exactMatch: false);
    }

    [Fact]
    public void Organisation_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        Organisation organisation = new();

        // Assert
        Assert.Equal(Guid.Empty, organisation.PartyId);
        Assert.Null(organisation.Name);
        Assert.Null(organisation.Description);
        Assert.Null(organisation.CompanyIdentifier);
        Assert.Null(organisation.Party);
    }

    [Fact]
    public void Organisation_Properties_CanBeSet()
    {
        // Arrange
        Organisation organisation = new();
        Guid partyId = Guid.NewGuid();
        string name = "Test Organisation";
        string description = "Test organisation description";
        string companyId = "12345678";

        // Act
        organisation.PartyId = partyId;
        organisation.Name = name;
        organisation.Description = description;
        organisation.CompanyIdentifier = companyId;

        // Assert
        Assert.Equal(partyId, organisation.PartyId);
        Assert.Equal(name, organisation.Name);
        Assert.Equal(description, organisation.Description);
        Assert.Equal(companyId, organisation.CompanyIdentifier);
    }

    [Fact]
    public void Organisation_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        Organisation organisation = new();

        // Assert
        Assert.IsType<AuditableEntity>(organisation, exactMatch: false);
    }

    [Fact]
    public void PartyAuthenticationProviderMetadata_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        PartyAuthenticationProviderMetadata metadata = new();

        // Assert
        Assert.Equal(Guid.Empty, metadata.PartyId);
        Assert.Equal(Guid.Empty, metadata.AuthenticationPolicyProviderId);
        Assert.Null(metadata.AuthenticationProviderUserId);
        Assert.Null(metadata.AuthenticationProviderSessionData);
    }

    [Fact]
    public void PartyAuthenticationProviderMetadata_Properties_CanBeSet()
    {
        // Arrange
        PartyAuthenticationProviderMetadata metadata = new();
        Guid partyId = Guid.NewGuid();
        Guid policyProviderId = Guid.NewGuid();
        string userId = "external-user-123";
        string sessionData = "session-token-xyz";

        // Act
        metadata.PartyId = partyId;
        metadata.AuthenticationPolicyProviderId = policyProviderId;
        metadata.AuthenticationProviderUserId = userId;
        metadata.AuthenticationProviderSessionData = sessionData;

        // Assert
        Assert.Equal(partyId, metadata.PartyId);
        Assert.Equal(policyProviderId, metadata.AuthenticationPolicyProviderId);
        Assert.Equal(userId, metadata.AuthenticationProviderUserId);
        Assert.Equal(sessionData, metadata.AuthenticationProviderSessionData);
    }

    [Fact]
    public void PartyAuthenticationProviderMetadata_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        PartyAuthenticationProviderMetadata metadata = new();

        // Assert
        Assert.IsType<AuditableEntity>(metadata, exactMatch: false);
    }

    [Fact]
    public void PartyProductRole_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        PartyProductRole partyProductRole = new();

        // Assert
        Assert.Equal(Guid.Empty, partyProductRole.PartyId);
        Assert.Equal(Guid.Empty, partyProductRole.ProductRoleId);
    }

    [Fact]
    public void PartyProductRole_Properties_CanBeSet()
    {
        // Arrange
        PartyProductRole partyProductRole = new();
        Guid partyId = Guid.NewGuid();
        Guid productRoleId = Guid.NewGuid();

        // Act
        partyProductRole.PartyId = partyId;
        partyProductRole.ProductRoleId = productRoleId;

        // Assert
        Assert.Equal(partyId, partyProductRole.PartyId);
        Assert.Equal(productRoleId, partyProductRole.ProductRoleId);
    }

    [Fact]
    public void PartyProductRole_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        PartyProductRole partyProductRole = new();

        // Assert
        Assert.IsType<AuditableEntity>(partyProductRole, exactMatch: false);
    }

    [Fact]
    public void Permission_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        Permission permission = new();

        // Assert
        Assert.Null(permission.Name);
        Assert.Null(permission.Description);
    }

    [Fact]
    public void Permission_Properties_CanBeSet()
    {
        // Arrange
        Permission permission = new();
        string name = "Read";
        string description = "Read permission";

        // Act
        permission.Name = name;
        permission.Description = description;

        // Assert
        Assert.Equal(name, permission.Name);
        Assert.Equal(description, permission.Description);
    }

    [Fact]
    public void Permission_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        Permission permission = new();

        // Assert
        Assert.IsType<AuditableEntity>(permission, exactMatch: false);
    }

    // Product Tests
    [Fact]
    public void Product_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        Product product = new();

        // Assert
        Assert.Null(product.Name);
        Assert.Null(product.Description);
        Assert.Null(product.Url);
    }

    [Fact]
    public void Product_Properties_CanBeSet()
    {
        // Arrange
        Product product = new();
        string name = "TestProduct";
        string description = "Test product description";
        string url = "https://example.com";

        // Act
        product.Name = name;
        product.Description = description;
        product.Url = url;

        // Assert
        Assert.Equal(name, product.Name);
        Assert.Equal(description, product.Description);
        Assert.Equal(url, product.Url);
    }

    [Fact]
    public void Product_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        Product product = new();

        // Assert
        Assert.IsType<AuditableEntity>(product, exactMatch: false);
    }

    [Fact]
    public void ProductAuthenticationPolicyProvider_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        ProductAuthenticationPolicyProvider productPolicyProvider = new();

        // Assert
        Assert.Equal(Guid.Empty, productPolicyProvider.ProductId);
        Assert.Equal(Guid.Empty, productPolicyProvider.AuthenticationPolicyProviderId);
    }

    [Fact]
    public void ProductAuthenticationPolicyProvider_Properties_CanBeSet()
    {
        // Arrange
        ProductAuthenticationPolicyProvider productPolicyProvider = new();
        Guid productId = Guid.NewGuid();
        Guid policyProviderId = Guid.NewGuid();

        // Act
        productPolicyProvider.ProductId = productId;
        productPolicyProvider.AuthenticationPolicyProviderId = policyProviderId;

        // Assert
        Assert.Equal(productId, productPolicyProvider.ProductId);
        Assert.Equal(policyProviderId, productPolicyProvider.AuthenticationPolicyProviderId);
    }

    [Fact]
    public void ProductAuthenticationPolicyProvider_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        ProductAuthenticationPolicyProvider productPolicyProvider = new();

        // Assert
        Assert.IsType<AuditableEntity>(productPolicyProvider, exactMatch: false);
    }

    // ProductRole Tests
    [Fact]
    public void ProductRole_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        ProductRole productRole = new();

        // Assert
        Assert.Equal(Guid.Empty, productRole.ProductId);
        Assert.Equal(Guid.Empty, productRole.RoleId);
    }

    [Fact]
    public void ProductRole_Properties_CanBeSet()
    {
        // Arrange
        ProductRole productRole = new();
        Guid productId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();

        // Act
        productRole.ProductId = productId;
        productRole.RoleId = roleId;

        // Assert
        Assert.Equal(productId, productRole.ProductId);
        Assert.Equal(roleId, productRole.RoleId);
    }

    [Fact]
    public void ProductRole_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        ProductRole productRole = new();

        // Assert
        Assert.IsType<AuditableEntity>(productRole, exactMatch: false);
    }

    [Fact]
    public void ProductRoleResourcePermission_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        ProductRoleResourcePermission prrp = new();

        // Assert
        Assert.Equal(Guid.Empty, prrp.ProductRoleId);
        Assert.Equal(Guid.Empty, prrp.ResourcePermissionId);
    }

    [Fact]
    public void ProductRoleResourcePermission_Properties_CanBeSet()
    {
        // Arrange
        ProductRoleResourcePermission prrp = new();
        Guid productRoleId = Guid.NewGuid();
        Guid resourcePermissionId = Guid.NewGuid();

        // Act
        prrp.ProductRoleId = productRoleId;
        prrp.ResourcePermissionId = resourcePermissionId;

        // Assert
        Assert.Equal(productRoleId, prrp.ProductRoleId);
        Assert.Equal(resourcePermissionId, prrp.ResourcePermissionId);
    }

    [Fact]
    public void ProductRoleResourcePermission_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        ProductRoleResourcePermission prrp = new();

        // Assert
        Assert.IsType<AuditableEntity>(prrp, exactMatch: false);
    }

    [Fact]
    public void Resource_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        Resource resource = new();

        // Assert
        Assert.Null(resource.Name);
        Assert.Null(resource.Description);
    }

    [Fact]
    public void Resource_Properties_CanBeSet()
    {
        // Arrange
        Resource resource = new();
        string name = "UserManagement";
        string description = "User management resource";

        // Act
        resource.Name = name;
        resource.Description = description;

        // Assert
        Assert.Equal(name, resource.Name);
        Assert.Equal(description, resource.Description);
    }

    [Fact]
    public void Resource_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        Resource resource = new();

        // Assert
        Assert.IsType<AuditableEntity>(resource, exactMatch: false);
    }

    [Fact]
    public void ResourcePermission_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        ResourcePermission resourcePermission = new();

        // Assert
        Assert.Equal(Guid.Empty, resourcePermission.ResourceId);
        Assert.Equal(Guid.Empty, resourcePermission.PermissionId);
    }

    [Fact]
    public void ResourcePermission_Properties_CanBeSet()
    {
        // Arrange
        ResourcePermission resourcePermission = new();
        Guid resourceId = Guid.NewGuid();
        Guid permissionId = Guid.NewGuid();

        // Act
        resourcePermission.ResourceId = resourceId;
        resourcePermission.PermissionId = permissionId;

        // Assert
        Assert.Equal(resourceId, resourcePermission.ResourceId);
        Assert.Equal(permissionId, resourcePermission.PermissionId);
    }

    [Fact]
    public void ResourcePermission_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        ResourcePermission resourcePermission = new();

        // Assert
        Assert.IsType<AuditableEntity>(resourcePermission, exactMatch: false);
    }

    [Fact]
    public void Role_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        Role role = new();

        // Assert
        Assert.Equal(string.Empty, role.Name);
        Assert.Equal(string.Empty, role.Description);
    }

    [Fact]
    public void Role_Properties_CanBeSet()
    {
        // Arrange
        Role role = new();
        string name = "Administrator";
        string description = "Administrator role";

        // Act
        role.Name = name;
        role.Description = description;

        // Assert
        Assert.Equal(name, role.Name);
        Assert.Equal(description, role.Description);
    }

    [Fact]
    public void Role_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        Role role = new();

        // Assert
        Assert.IsType<AuditableEntity>(role, exactMatch: false);
    }

    [Fact]
    public void RoleMetadata_CanBeInstantiated_WithDefaultValues()
    {
        // Arrange & Act
        RoleMetadata roleMetadata = new();

        // Assert
        Assert.Equal(Guid.Empty, roleMetadata.RoleId);
        Assert.Null(roleMetadata.Name);
        Assert.Null(roleMetadata.Value);
    }

    [Fact]
    public void RoleMetadata_Properties_CanBeSet()
    {
        // Arrange
        RoleMetadata roleMetadata = new();
        Guid roleId = Guid.NewGuid();
        string name = "DisplayName";
        string value = "Super Administrator";

        // Act
        roleMetadata.RoleId = roleId;
        roleMetadata.Name = name;
        roleMetadata.Value = value;

        // Assert
        Assert.Equal(roleId, roleMetadata.RoleId);
        Assert.Equal(name, roleMetadata.Name);
        Assert.Equal(value, roleMetadata.Value);
    }

    [Fact]
    public void RoleMetadata_InheritsFromAuditableEntity()
    {
        // Arrange & Act
        RoleMetadata roleMetadata = new();

        // Assert
        Assert.IsType<AuditableEntity>(roleMetadata, exactMatch: false);
    }
}
