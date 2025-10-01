CREATE TABLE [dbo].[ApplicationRole]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[ApplicationId] UNIQUEIDENTIFIER NOT NULL,
	[RoleId] UNIQUEIDENTIFIER NOT NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_ApplicationRole] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[ApplicationRole]
ADD CONSTRAINT [DF_ApplicationRole_Id] DEFAULT (NEWSEQUENTIALID()) FOR [Id];
GO

ALTER TABLE [dbo].[ApplicationRole]
ADD CONSTRAINT [DF_ApplicationRole_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO

ALTER TABLE [dbo].[ApplicationRole] ADD CONSTRAINT [FK_ApplicationRole_Application] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Application] ([Id])
GO

ALTER TABLE [dbo].[ApplicationRole] CHECK CONSTRAINT [FK_ApplicationRole_Application]
GO

ALTER TABLE [dbo].[ApplicationRole] ADD CONSTRAINT [FK_ApplicationRole_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO

ALTER TABLE [dbo].[ApplicationRole] CHECK CONSTRAINT [FK_ApplicationRole_Role]
GO

