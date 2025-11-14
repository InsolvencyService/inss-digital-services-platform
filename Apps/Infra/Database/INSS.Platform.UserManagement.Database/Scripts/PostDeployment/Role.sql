-- =========================================================================
-- 1. Assign the Roles
-- =========================================================================
DECLARE @RoleId_InsolvencyAdministrator UNIQUEIDENTIFIER = NEWID();
DECLARE @RoleId_OfficialReceiver UNIQUEIDENTIFIER = NEWID();
DECLARE @RoleId_InsolvencyPractitioner UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[Role] ([Id], [Name], [Created], [Description], [CreatedBy], [Modified], [ModifiedBy])
VALUES
(@RoleId_InsolvencyAdministrator, 'Insolvency Administrator', '', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@RoleId_OfficialReceiver, 'Official Receiver', '', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@RoleId_InsolvencyPractitioner, 'Insolvency Practitioner', '', GETDATE(), SUSER_SNAME(), NULL, NULL);


-- =========================================================================
-- 2. Assign the Role Metadata
-- =========================================================================
INSERT INTO [dbo].[RoleMetadata] ([Id], [RoleId], [Name], [Value], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES
(NEWID(), @RoleId_InsolvencyAdministrator, 'Can Impersonate', 'True', GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @RoleId_OfficialReceiver, 'Approved Intermediary ID', 'AI001', GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @RoleId_InsolvencyPractitioner, 'Insolvency Practitioner ID', 'IP001', GETDATE(), SUSER_SNAME(), NULL, NULL);

