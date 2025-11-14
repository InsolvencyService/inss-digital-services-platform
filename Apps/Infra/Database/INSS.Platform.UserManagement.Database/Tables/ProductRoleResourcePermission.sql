CREATE TABLE [dbo].[ProductRoleResourcePermission]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[ProductRoleId] UNIQUEIDENTIFIER NOT NULL,
	[ResourcePermissionId] UNIQUEIDENTIFIER NOT NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_ProductRoleResourcePermission] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[ProductRoleResourcePermission] ADD CONSTRAINT [FK_ProductRoleResourcePermission_ProductRole] FOREIGN KEY([ProductRoleId])
REFERENCES [dbo].[ProductRole] ([Id])
GO

ALTER TABLE [dbo].[ProductRoleResourcePermission] CHECK CONSTRAINT [FK_ProductRoleResourcePermission_ProductRole]
GO

ALTER TABLE [dbo].[ProductRoleResourcePermission] ADD CONSTRAINT [FK_ProductRoleResourcePermission_ResourcePermission] FOREIGN KEY([ResourcePermissionId])
REFERENCES [dbo].[ResourcePermission] ([Id])
GO

ALTER TABLE [dbo].[ProductRoleResourcePermission] CHECK CONSTRAINT [FK_ProductRoleResourcePermission_ResourcePermission]
GO

ALTER TABLE [dbo].[ProductRoleResourcePermission]
ADD CONSTRAINT [DF_ProductRoleResourcePermission_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO
