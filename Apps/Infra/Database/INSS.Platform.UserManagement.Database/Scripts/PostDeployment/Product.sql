-- =========================================================================
-- 1. Assign the Products
-- =========================================================================

DECLARE @ProductId_UM UNIQUEIDENTIFIER = NEWID();
DECLARE @ProductId_DCRS UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[Product] ([Id], [Name], [Description], [Url], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES
(@ProductId_UM, 'User Management', 'User Management Application', NULL, GETDATE(), 'System', GETDATE(), 'System'),
(@ProductId_DCRS, 'DCRS', 'Director Conduct Reporting System', NULL, GETDATE(), 'System', GETDATE(), 'System');

-- =========================================================================
-- 2. Assign the ProductAuthentication Policy Providers
-- =========================================================================
INSERT INTO [dbo].[ProductAuthenticationPolicyProvider] ([Id], [ProductId], [AuthenticationPolicyProviderId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES
(NEWID(), @ProductId_UM, @OneLoginPolicyProviderId, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @ProductId_DCRS, @OneLoginPolicyProviderId, GETDATE(), SUSER_SNAME(), NULL, NULL);


