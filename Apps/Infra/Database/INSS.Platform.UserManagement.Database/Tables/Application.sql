CREATE TABLE [dbo].[Application]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[Name] NVARCHAR(255) NOT NULL,
	[IdentityProviderId] UNIQUEIDENTIFIER NOT NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_Application] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[Application]
ADD CONSTRAINT [DF_Application_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO

ALTER TABLE [dbo].[Application] ADD CONSTRAINT [FK_Application_IdentityProvider] FOREIGN KEY([IdentityProviderId])
REFERENCES [dbo].[IdentityProvider] ([Id])
GO

ALTER TABLE [dbo].[Application] CHECK CONSTRAINT [FK_Application_IdentityProvider]
GO

ALTER TABLE [dbo].[Application]
ADD CONSTRAINT [UQ_Application_Name_IdentityProviderId] UNIQUE ([Name], [IdentityProviderId]);
GO
