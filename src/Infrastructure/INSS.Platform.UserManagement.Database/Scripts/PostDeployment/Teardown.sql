------------------------------------------------------------------
--	Removes all test and seed data from the database.
------------------------------------------------------------------

DELETE FROM [dbo].[Individual]
DELETE FROM [dbo].[Organisation]
DELETE FROM [dbo].[Group]
DELETE FROM [dbo].[Address]
DELETE FROM [dbo].[AddressType]
DELETE FROM [dbo].[PartyAuthenticationProviderMetadata]
DELETE FROM [dbo].[PartyProductRole]
DELETE FROM [dbo].[PartyRelationship]

DELETE FROM [dbo].[ProductRoleResourcePermission]
DELETE FROM [dbo].[ResourcePermission]
DELETE FROM [dbo].[Permission]
DELETE FROM [dbo].[Resource]

DELETE FROM [dbo].[ProductAuthenticationPolicyProvider]
DELETE FROM [dbo].[ProductRole]
DELETE FROM [dbo].[Product]

DELETE FROM [dbo].[RoleMetadata]
DELETE FROM [dbo].[Role]

DELETE FROM [dbo].[AuthenticationPolicyProvider]
DELETE FROM [dbo].[AuthenticationPolicy]
DELETE FROM [dbo].[AuthenticationProviderMetadata]
DELETE FROM [dbo].[AuthenticationProvider]

DELETE FROM [dbo].[Party]
DELETE FROM [dbo].[PartyType]
DELETE FROM [dbo].[RelationshipType]
