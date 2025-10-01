CREATE TABLE [dbo].[UserIdentity]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[IdentityProviderUserId] NVARCHAR(100) NOT NULL,
	[IdentityProviderId] UNIQUEIDENTIFIER NOT NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_UserIdentity] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[UserIdentity]
ADD CONSTRAINT [DF_UserIdentity] DEFAULT (NEWSEQUENTIALID()) FOR [Id];
GO

ALTER TABLE [dbo].[UserIdentity]
ADD CONSTRAINT [DF_UserIdentity_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO

ALTER TABLE [dbo].[UserIdentity] ADD CONSTRAINT [FK_UserIdentity_IdentityProvider] FOREIGN KEY([IdentityProviderId])
REFERENCES [dbo].[IdentityProvider] ([Id])
GO

ALTER TABLE [dbo].[UserIdentity] CHECK CONSTRAINT [FK_UserIdentity_IdentityProvider]
GO
