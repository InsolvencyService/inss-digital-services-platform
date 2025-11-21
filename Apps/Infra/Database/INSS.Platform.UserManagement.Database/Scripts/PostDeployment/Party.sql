-- =========================================================================
-- 1. Assign the Parties
-- =========================================================================
DECLARE @PartyId_DaveCook UNIQUEIDENTIFIER = NEWID();
DECLARE @PartyId_GlenStone UNIQUEIDENTIFIER = NEWID();
DECLARE @PartyId_SteveSaunders UNIQUEIDENTIFIER = NEWID();
DECLARE @PartyId_WayneBusby UNIQUEIDENTIFIER = NEWID();
DECLARE @PartyId_ButchCassidy UNIQUEIDENTIFIER = NEWID();
DECLARE @PartyId_InsolvencyService UNIQUEIDENTIFIER = NEWID();
DECLARE @PartyId_UnityCreditServices UNIQUEIDENTIFIER = NEWID();
DECLARE @PartyId_BrightFutureTrust UNIQUEIDENTIFIER = NEWID();
DECLARE @PartyId_DevTeam UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[Party] ([Id], [PartyTypeId], [SourceOfIntroduction], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES
(@PartyId_DaveCook, @PartyTypeId_Individual, '', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@PartyId_GlenStone, @PartyTypeId_Individual, '', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@PartyId_SteveSaunders, @PartyTypeId_Individual, '', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@PartyId_WayneBusby, @PartyTypeId_Individual, '', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@PartyId_ButchCassidy, @PartyTypeId_Individual, '', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@PartyId_InsolvencyService, @PartyTypeId_Organization, '', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@PartyId_UnityCreditServices, @PartyTypeId_Organization, '', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@PartyId_BrightFutureTrust, @PartyTypeId_Organization, '', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@PartyId_DevTeam, @PartyTypeId_Group, '', GETDATE(), SUSER_SNAME(), NULL, NULL);

INSERT INTO [dbo].[PartyRelationship] ([Id], [FromPartyId], [ToPartyId], [RelationshipTypeId],[StartDate], [EndDate], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES
(NEWID(), @PartyId_DaveCook, @PartyId_InsolvencyService, @RelationshipTypeId_EmployedBy, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_GlenStone, @PartyId_InsolvencyService, @RelationshipTypeId_EmployedBy, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_SteveSaunders, @PartyId_InsolvencyService, @RelationshipTypeId_EmployedBy, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_WayneBusby, @PartyId_InsolvencyService, @RelationshipTypeId_EmployedBy, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_ButchCassidy, @PartyId_InsolvencyService, @RelationshipTypeId_EmployedBy, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),

(NEWID(), @PartyId_InsolvencyService, @PartyId_DaveCook, @RelationshipTypeId_Employs, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_InsolvencyService, @PartyId_GlenStone,  @RelationshipTypeId_Employs, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_InsolvencyService, @PartyId_SteveSaunders, @RelationshipTypeId_Employs, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_InsolvencyService, @PartyId_WayneBusby, @RelationshipTypeId_Employs, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_InsolvencyService, @PartyId_ButchCassidy, @RelationshipTypeId_Employs, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),
-------------------------------------------------------------------------------------------------------------------------------------------------------
(NEWID(), @PartyId_DaveCook, @PartyId_DevTeam, @RelationshipTypeId_IsMemberOf, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_GlenStone, @PartyId_DevTeam, @RelationshipTypeId_IsMemberOf, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_SteveSaunders, @PartyId_DevTeam, @RelationshipTypeId_IsMemberOf, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),

(NEWID(), @PartyId_DevTeam, @PartyId_DaveCook, @RelationshipTypeId_HasMember, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_DevTeam, @PartyId_GlenStone, @RelationshipTypeId_HasMember, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_DevTeam, @PartyId_SteveSaunders, @RelationshipTypeId_HasMember, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),
-------------------------------------------------------------------------------------------------------------------------------------------------------
(NEWID(), @PartyId_DaveCook, @PartyId_BrightFutureTrust, @RelationshipTypeId_EmployedBy, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_GlenStone, @PartyId_BrightFutureTrust, @RelationshipTypeId_EmployedBy, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_SteveSaunders, @PartyId_BrightFutureTrust, @RelationshipTypeId_EmployedBy, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),

(NEWID(), @PartyId_BrightFutureTrust, @PartyId_DaveCook, @RelationshipTypeId_Employs, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_BrightFutureTrust, @PartyId_GlenStone, @RelationshipTypeId_Employs, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_BrightFutureTrust, @PartyId_SteveSaunders, @RelationshipTypeId_Employs, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),
-------------------------------------------------------------------------------------------------------------------------------------------------------
(NEWID(), @PartyId_ButchCassidy, @PartyId_UnityCreditServices, @RelationshipTypeId_EmployedBy, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_UnityCreditServices, @PartyId_ButchCassidy, @RelationshipTypeId_Employs, GETDATE(), NULL, GETDATE(), SUSER_SNAME(), NULL, NULL);
-------------------------------------------------------------------------------------------------------------------------------------------------------


-- =========================================================================
-- 2. Assign the Individuals
-- =========================================================================
INSERT INTO [dbo].[Individual] ([Id], [PartyId], [FirstName], [LastName], [DateOfBirth], [NationalInsuranceNumber], [IsIdentityVerified], [VerificationSource], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES
(@PartyId_DaveCook, @PartyId_DaveCook, 'Dave', 'Cook', '1980-01-15', 'AB123456C', 1, 'GovID', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@PartyId_GlenStone, @PartyId_GlenStone, 'Glen', 'Stone', '1975-05-30', 'CD789012E', 1, 'GovID', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@PartyId_SteveSaunders, @PartyId_SteveSaunders, 'Steve', 'Saunders', '1990-09-20', 'EF345678G', 1, 'GovID', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@PartyId_WayneBusby, @PartyId_WayneBusby, 'Wayne', 'Busby', '1985-12-10', 'GH901234I', 1, 'GovID', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@PartyId_ButchCassidy, @PartyId_ButchCassidy, 'Butch', 'Cassidy', '1970-03-25', 'IJ567890K', 1, 'GovID', GETDATE(), SUSER_SNAME(), NULL, NULL);


-- =========================================================================
-- 3. Assign the Organisations
-- =========================================================================
INSERT INTO [dbo].[Organisation] ([Id], [PartyId], [Name], [Description], [CompanyIdentifier], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES
(@PartyId_InsolvencyService, @PartyId_InsolvencyService, 'Insolvency Service', 'Insolvency Service Description', 'IS123456', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@PartyId_BrightFutureTrust, @PartyId_BrightFutureTrust, 'Bright Future Trust', 'Bright Future Trust Description', 'BFT123456', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@PartyId_UnityCreditServices, @PartyId_UnityCreditServices, 'Unity Credit Services', 'Unity Credit Services Description', 'UCS123456', GETDATE(), SUSER_SNAME(), NULL, NULL);


-- =========================================================================
-- 4. Assign the Groups
-- =========================================================================
INSERT INTO [dbo].[Group] ([Id], [PartyId], [Name], [Description], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES
(@PartyId_DevTeam, @PartyId_DevTeam, 'Development Team', 'Group for development activities', GETDATE(), SUSER_SNAME(), NULL, NULL);


-- =========================================================================
-- 4. Assign the Addresses
-- =========================================================================
INSERT INTO [dbo].[Address] ([Id], [PartyId], [AddressTypeId], [AddressLine1], [AddressLine2], [AddressLine3], [Postcode], [UPRN], [Longitude], [Latitude], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES
(NEWID(), @PartyId_DaveCook, @AddressTypeId_Primary, '123 Main St', 'Apt 4B', 'Springfield', 'SP1 2AB', '10001234567', '-1.234567', '51.234567', GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_InsolvencyService, @AddressTypeId_Primary, '456 Corporate Blvd', 'Suite 800', 'Metropolis', 'MT3 4CD', '20007654321', '-2.345678', '52.345678', GETDATE(), SUSER_SNAME(), NULL, NULL);


-- =========================================================================
-- 5. Assign the Party Product Roles
-- =========================================================================
INSERT INTO [dbo].[PartyProductRole] ([Id], [PartyId], [ProductRoleId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES
(NEWID(), @PartyId_DaveCook, @ProductRoleId_UM_InsolvencyAdministrator, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_GlenStone, @ProductRoleId_UM_InsolvencyAdministrator, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_SteveSaunders, @ProductRoleId_UM_InsolvencyAdministrator, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_WayneBusby, @ProductRoleId_UM_InsolvencyAdministrator, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_ButchCassidy, @ProductRoleId_UM_InsolvencyAdministrator, GETDATE(), SUSER_SNAME(), NULL, NULL),

(NEWID(), @PartyId_DaveCook, @ProductRoleId_DCRS_InsolvencyPractitioner, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_GlenStone, @ProductRoleId_DCRS_InsolvencyPractitioner, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_SteveSaunders, @ProductRoleId_DCRS_InsolvencyPractitioner, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @PartyId_WayneBusby, @ProductRoleId_DCRS_InsolvencyPractitioner, GETDATE(), SUSER_SNAME(), NULL, NULL);


-- =========================================================================
-- 6. Assign the Party Authentication Provider Metadata
-- =========================================================================
INSERT INTO [dbo].[PartyAuthenticationProviderMetadata] ([Id], [PartyId], [AuthenticationPolicyProviderId], [AuthenticationProviderUserId], [AuthenticationProviderSessionData], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES
(NEWID(), @PartyId_DaveCook, @OneLoginPolicyProviderId, @OneLoginId_DaveCook, NULL, GETDATE(), SUSER_SNAME(), NULL, NULL);

