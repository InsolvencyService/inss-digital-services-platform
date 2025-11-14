CREATE TABLE [dbo].[AuthenticationProvider]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[Name] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(512) NOT NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_AuthenticationProvider] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[AuthenticationProvider]
ADD CONSTRAINT [UQ_AuthenticationProvider_Name] UNIQUE ([Name]);
GO

ALTER TABLE [dbo].[AuthenticationProvider]
ADD CONSTRAINT [DF_AuthenticationProvider_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO
