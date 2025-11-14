CREATE TABLE [dbo].[PartyProductRole]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[PartyId] UNIQUEIDENTIFIER NOT NULL,
	[ProductRoleId] UNIQUEIDENTIFIER NOT NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_PartyProductRole] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[PartyProductRole] ADD CONSTRAINT [FK_PartyProductRole_Party] FOREIGN KEY([PartyId])
REFERENCES [dbo].[Party] ([Id])
GO

ALTER TABLE [dbo].[PartyProductRole] CHECK CONSTRAINT [FK_PartyProductRole_Party]
GO

ALTER TABLE [dbo].[PartyProductRole] ADD CONSTRAINT [FK_PartyProductRole_ProductRole] FOREIGN KEY([ProductRoleId])
REFERENCES [dbo].[ProductRole] ([Id])
GO

ALTER TABLE [dbo].[PartyProductRole] CHECK CONSTRAINT [FK_PartyProductRole_ProductRole]
GO

ALTER TABLE [dbo].[PartyProductRole]
ADD CONSTRAINT [DF_PartyProductRole_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO
