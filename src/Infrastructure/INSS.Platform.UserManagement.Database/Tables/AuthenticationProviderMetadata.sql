CREATE TABLE [dbo].[AuthenticationProviderMetadata]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[AuthenticationProviderId] UNIQUEIDENTIFIER NOT NULL,
	[ClientId] NVARCHAR(100) NOT NULL,
	[Secret] NVARCHAR(100) NULL,
	[AuthorizeEndPoint] NVARCHAR(2083) NULL,
	[TokenEndPoint] NVARCHAR(2083) NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_AuthenticationProviderMetadata] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[AuthenticationProviderMetadata] ADD CONSTRAINT [FK_AuthenticationProviderMetadata_AuthenticationProvider] FOREIGN KEY([AuthenticationProviderId])
REFERENCES [dbo].[AuthenticationProvider] ([Id])
GO

ALTER TABLE [dbo].[AuthenticationProviderMetadata] CHECK CONSTRAINT [FK_AuthenticationProviderMetadata_AuthenticationProvider]
GO

ALTER TABLE [dbo].[AuthenticationProviderMetadata]
ADD CONSTRAINT [UQ_AuthenticationProviderMetadata_ClientId] UNIQUE ([ClientId]);
GO

ALTER TABLE [dbo].[AuthenticationProviderMetadata]
ADD CONSTRAINT [DF_AuthenticationProviderMetadata_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO
