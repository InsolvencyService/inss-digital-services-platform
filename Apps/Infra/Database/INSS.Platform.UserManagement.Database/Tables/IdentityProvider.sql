CREATE TABLE [dbo].[IdentityProvider]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
	[IssuerURL] NVARCHAR(2048) NULL,
	[ClientId] NVARCHAR(255) NULL,
	[Secret] NVARCHAR(255) NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_IdentityProvider] PRIMARY KEY CLUSTERED 
	(
		[Id]
	)
)
GO

ALTER TABLE [dbo].[IdentityProvider]
ADD CONSTRAINT [DF_IdentityProvider_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO

ALTER TABLE [dbo].[IdentityProvider]
ADD CONSTRAINT [UQ_IdentityProvider_Name] UNIQUE ([Name]);
GO
