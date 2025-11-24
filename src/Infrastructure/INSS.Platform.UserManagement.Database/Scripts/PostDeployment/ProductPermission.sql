-- =========================================================================
-- 1. Assign the Product Roles
-- =========================================================================
DECLARE @ProductRoleId_UM_InsolvencyAdministrator UNIQUEIDENTIFIER = NEWID();
DECLARE @ProductRoleId_DCRS_OfficialReceiver UNIQUEIDENTIFIER = NEWID();
DECLARE @ProductRoleId_DCRS_InsolvencyPractitioner UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[ProductRole] ([Id], [ProductId], [RoleId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES
(@ProductRoleId_UM_InsolvencyAdministrator, @ProductId_UM, @RoleId_InsolvencyAdministrator, GETDATE(), SUSER_SNAME(), NULL, NULL),
(@ProductRoleId_DCRS_OfficialReceiver, @ProductId_DCRS, @RoleId_OfficialReceiver, GETDATE(), SUSER_SNAME(), NULL, NULL),
(@ProductRoleId_DCRS_InsolvencyPractitioner, @ProductId_DCRS, @RoleId_InsolvencyPractitioner, GETDATE(), SUSER_SNAME(), NULL, NULL);


-- =========================================================================
-- 2. Assign the Resources
-- =========================================================================
DECLARE @ResourceId_All UNIQUEIDENTIFIER = NEWID();
DECLARE @ResourceId_Asset UNIQUEIDENTIFIER = NEWID();
DECLARE @ResourceId_Income UNIQUEIDENTIFIER = NEWID();
DECLARE @ResourceId_Creditor UNIQUEIDENTIFIER = NEWID();
DECLARE @ResourceId_Director UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[Resource] ([Id], [Name], [Description], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES
(@ResourceId_All, 'All', 'All Resources', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@ResourceId_Asset, 'Asset', 'Asset Resource', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@ResourceId_Income, 'Income', 'Income Resource', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@ResourceId_Creditor, 'Creditor', 'Creditor Resource', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@ResourceId_Director, 'Director', 'Director Resource', GETDATE(), SUSER_SNAME(), NULL, NULL);


-- =========================================================================
-- 2. Assign the Permissions
-- =========================================================================
DECLARE @PermissionId_Manage UNIQUEIDENTIFIER = NEWID();
DECLARE @PermissionId_Create UNIQUEIDENTIFIER = NEWID();
DECLARE @PermissionId_Read UNIQUEIDENTIFIER = NEWID();
DECLARE @PermissionId_Update UNIQUEIDENTIFIER = NEWID();
DECLARE @PermissionId_Delete UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[Permission] ([Id], [Name], [Description], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES
(@PermissionId_Manage, 'Manage', 'Can create, read, update and delete', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@PermissionId_Create, 'Create', 'Can create', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@PermissionId_Read, 'Read', 'Can read and print', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@PermissionId_Update, 'Update', 'Can update', GETDATE(), SUSER_SNAME(), NULL, NULL),
(@PermissionId_Delete, 'Delete', 'Can delete', GETDATE(), SUSER_SNAME(), NULL, NULL);


-- =========================================================================
-- 3. Assign the Resource Permissions
-- =========================================================================
DECLARE @ResourceId_UM_InsolvencyAdministrator_All_Manage UNIQUEIDENTIFIER = NEWID();
DECLARE @ResourceId_DCRS_OfficialReceiver_Director_Read UNIQUEIDENTIFIER = NEWID();
DECLARE @ResourceId_DCRS_InsolvencyPractitioner_All_Manage UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[ResourcePermission] ([Id], [ResourceId], [PermissionId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES
(@ResourceId_UM_InsolvencyAdministrator_All_Manage, @ResourceId_All, @PermissionId_Manage, GETDATE(), SUSER_SNAME(), NULL, NULL),
(@ResourceId_DCRS_OfficialReceiver_Director_Read, @ResourceId_Director, @PermissionId_Read, GETDATE(), SUSER_SNAME(), NULL, NULL),
(@ResourceId_DCRS_InsolvencyPractitioner_All_Manage, @ResourceId_All, @PermissionId_Manage, GETDATE(), SUSER_SNAME(), NULL, NULL);


-- =========================================================================
-- 4. Assign the Product Role Resource Permissions
-- =========================================================================
INSERT INTO [dbo].[ProductRoleResourcePermission] ([Id], [ProductRoleId], [ResourcePermissionId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES
(NEWID(), @ProductRoleId_UM_InsolvencyAdministrator, @ResourceId_UM_InsolvencyAdministrator_All_Manage, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @ProductRoleId_DCRS_OfficialReceiver, @ResourceId_DCRS_OfficialReceiver_Director_Read, GETDATE(), SUSER_SNAME(), NULL, NULL),
(NEWID(), @ProductRoleId_DCRS_InsolvencyPractitioner, @ResourceId_DCRS_InsolvencyPractitioner_All_Manage, GETDATE(), SUSER_SNAME(), NULL, NULL);
