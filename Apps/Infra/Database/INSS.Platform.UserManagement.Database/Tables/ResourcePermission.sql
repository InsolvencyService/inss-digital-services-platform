CREATE TABLE [dbo].[ResourcePermission]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[ResourceId] UNIQUEIDENTIFIER NOT NULL,
	[PermissionId] UNIQUEIDENTIFIER NOT NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_ResourcePermission] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[ResourcePermission] ADD CONSTRAINT [FK_ResourcePermission_Resource] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[Resource] ([Id])
GO

ALTER TABLE [dbo].[ResourcePermission] CHECK CONSTRAINT [FK_ResourcePermission_Resource]
GO

ALTER TABLE [dbo].[ResourcePermission] ADD CONSTRAINT [FK_ResourcePermission_Permission] FOREIGN KEY([PermissionId])
REFERENCES [dbo].[Permission] ([Id])
GO

ALTER TABLE [dbo].[ResourcePermission] CHECK CONSTRAINT [FK_ResourcePermission_Permission]
GO

ALTER TABLE [dbo].[ResourcePermission]
ADD CONSTRAINT [DF_ResourcePermission_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO
