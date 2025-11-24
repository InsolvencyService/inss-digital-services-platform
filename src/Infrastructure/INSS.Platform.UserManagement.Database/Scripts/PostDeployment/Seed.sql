-- =========================================================================
-- 1. Assign the Party Relationship Types
-- =========================================================================
DECLARE @RelationshipTypeId_EmployedBy UNIQUEIDENTIFIER = NEWID();
DECLARE @RelationshipTypeId_Employs UNIQUEIDENTIFIER = NEWID();
DECLARE @RelationshipTypeId_IsMemberOf UNIQUEIDENTIFIER = NEWID();
DECLARE @RelationshipTypeId_HasMember UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[RelationshipType] ([Id], [Name], [Description], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES
(@RelationshipTypeId_EmployedBy, 'EmployedBy', 'The party is employed by another party.', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@RelationshipTypeId_Employs, 'Employs', 'The party employs another party.', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@RelationshipTypeId_IsMemberOf, 'MemberOf', 'The party is a member of another party.', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@RelationshipTypeId_HasMember, 'HasMember', 'The party has a member that is another party.', GETDATE(), SUSER_SNAME(), NULL, NULL);


-- =========================================================================
-- 2. Assign the Party Types
-- =========================================================================
DECLARE @PartyTypeId_Individual UNIQUEIDENTIFIER = NEWID();
DECLARE @PartyTypeId_Organization UNIQUEIDENTIFIER = NEWID();
DECLARE @PartyTypeId_Group UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[PartyType] ([Id], [Name], [Description], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES
(@PartyTypeId_Individual, 'Individual', 'An individual person.', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@PartyTypeId_Organization, 'Organization', 'A legal entity such as a company or non-profit.', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@PartyTypeId_Group, 'Group', 'A collection of individuals or organizations.', GETDATE(), SUSER_SNAME(), NULL, NULL);


-- =========================================================================
-- 3. Assign the Address Types
-- =========================================================================
DECLARE @AddressTypeId_Primary UNIQUEIDENTIFIER = NEWID();
DECLARE @AddressTypeId_Billing UNIQUEIDENTIFIER = NEWID();
DECLARE @AddressTypeId_Postal UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[AddressType] ([Id], [Name], [Description], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES
(@AddressTypeId_Primary, 'Primary', 'Primary Address', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@AddressTypeId_Billing, 'Billing', 'Billing Address', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@AddressTypeId_Postal, 'Postal', 'Postal Address', GETDATE(), SUSER_SNAME(), NULL, NULL);

