CREATE TABLE [dbo].[OrganisationUserApplicationRole]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[OrganisationUserId] UNIQUEIDENTIFIER NOT NULL,
	[ApplicationRoleId] UNIQUEIDENTIFIER NOT NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_OrganisationUserApplicationRole] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[OrganisationUserApplicationRole]
ADD CONSTRAINT [DF_OrganisationUserApplicationRole_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO

ALTER TABLE [dbo].[OrganisationUserApplicationRole] ADD CONSTRAINT [FK_OrganisationUserApplicationRole_OrganisationUser] FOREIGN KEY([OrganisationUserId])
REFERENCES [dbo].[OrganisationUser] ([Id])
GO

ALTER TABLE [dbo].[OrganisationUserApplicationRole] CHECK CONSTRAINT [FK_OrganisationUserApplicationRole_OrganisationUser]
GO

ALTER TABLE [dbo].[OrganisationUserApplicationRole] ADD CONSTRAINT [FK_OrganisationUserApplicationRole_ApplicationRole] FOREIGN KEY([ApplicationRoleId])
REFERENCES [dbo].[ApplicationRole] ([Id])
GO

ALTER TABLE [dbo].[OrganisationUserApplicationRole] CHECK CONSTRAINT [FK_OrganisationUserApplicationRole_ApplicationRole]
GO

ALTER TABLE [dbo].[OrganisationUserApplicationRole]
ADD CONSTRAINT [UQ_OrganisationUserApplicationRole_OrganisationUserId_ApplicationRoleId] UNIQUE ([OrganisationUserId], [ApplicationRoleId]);
GO


