CREATE TABLE [dbo].[ProductRole]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[ProductId] UNIQUEIDENTIFIER NOT NULL,
	[RoleId] UNIQUEIDENTIFIER NOT NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_ProductRole] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[ProductRole] ADD CONSTRAINT [FK_ProductRole_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO

ALTER TABLE [dbo].[ProductRole] CHECK CONSTRAINT [FK_ProductRole_Product]
GO

ALTER TABLE [dbo].[ProductRole] ADD CONSTRAINT [FK_ProductRole_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO

ALTER TABLE [dbo].[ProductRole] CHECK CONSTRAINT [FK_ProductRole_Role]
GO

ALTER TABLE [dbo].[ProductRole]
ADD CONSTRAINT [DF_ProductRole_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO
