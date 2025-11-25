-- =========================================================================
-- 1. Pull in the Secrets
-- =========================================================================
:r .\SecretKeys.sql
:r .\SecretValues.sql


-- =========================================================================
-- 1. Assign the Authentication Providers
-- =========================================================================
DECLARE @OneLoginProviderId UNIQUEIDENTIFIER = NEWID();
DECLARE @EntraProviderId UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[AuthenticationProvider] ([Id], [Name], [Description], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES 
(@OneLoginProviderId, 'OneLogin', 'One Login Identity Provider', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@EntraProviderId, 'Entra', 'Microsoft Entra Authentication Provider', GETDATE(), SUSER_SNAME(), NULL, NULL);


-- =========================================================================
-- 2. Assign the Authentication Provider Metadata
-- =========================================================================
INSERT INTO [dbo].[AuthenticationProviderMetadata] ([Id], [AuthenticationProviderId], [ClientId], [Secret], [AuthorizeEndPoint], [TokenEndPoint], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES 
(NEWID(), @OneLoginProviderId, @OneLoginClientId, NULL, 'https://oidc.integration.account.gov.uk', 'https://oidc.integration.account.gov.uk/token', GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @EntraProviderId, @EntraClientId, @EntraSecret, 'https://login.microsoftonline.com/' + @EntraTenant + '/v2.0', NULL, GETDATE(), SUSER_SNAME(), NULL, NULL);


-- =========================================================================
-- 3. Assign the Authentication Policies
-- =========================================================================
DECLARE @DefaultPolicyId UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[AuthenticationPolicy] ([Id], [Name], [Description], [RequireMultiFactorAuthentication], [RequireIdentityVerification], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (@DefaultPolicyId, 'Default Policy', 'The default authentication policy.', 1, 0, GETDATE(), SUSER_SNAME(), NULL, NULL);


-- =========================================================================
-- 4. Assign the Authentication Policy Providers
-- =========================================================================
DECLARE @OneLoginPolicyProviderId UNIQUEIDENTIFIER = NEWID();
DECLARE @EntraPolicyProviderId UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[AuthenticationPolicyProvider] ([Id], [AuthenticationPolicyId], [AuthenticationProviderId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES 
(@OneLoginPolicyProviderId, @DefaultPolicyId, @OneLoginProviderId, GETDATE(), SUSER_SNAME(), NULL, NULL),
(@EntraPolicyProviderId, @DefaultPolicyId, @EntraProviderId, GETDATE(), SUSER_SNAME(), NULL, NULL);


