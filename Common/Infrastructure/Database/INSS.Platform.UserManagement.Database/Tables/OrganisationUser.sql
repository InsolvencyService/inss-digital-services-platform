CREATE TABLE [dbo].[OrganisationUser]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[OrganisationId] UNIQUEIDENTIFIER NOT NULL,
	[UserId] UNIQUEIDENTIFIER NOT NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_OrganisationUser] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[OrganisationUser]
ADD CONSTRAINT [DF_OrganisationUser_Id] DEFAULT (NEWSEQUENTIALID()) FOR [Id];
GO

ALTER TABLE [dbo].[OrganisationUser]
ADD CONSTRAINT [DF_OrganisationUser_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO

ALTER TABLE [dbo].[OrganisationUser] ADD CONSTRAINT [FK_OrganisationUser_Organisation] FOREIGN KEY([OrganisationId])
REFERENCES [dbo].[Organisation] ([Id])
GO

ALTER TABLE [dbo].[OrganisationUser] CHECK CONSTRAINT [FK_OrganisationUser_Organisation]
GO

ALTER TABLE [dbo].[OrganisationUser] ADD CONSTRAINT [FK_OrganisationUser_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO

ALTER TABLE [dbo].[OrganisationUser] CHECK CONSTRAINT [FK_OrganisationUser_User]
GO


