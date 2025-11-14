CREATE TABLE [dbo].[AuthenticationPolicy]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[Name] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(512) NOT NULL,
	[RequireMultiFactorAuthentication] BIT NOT NULL,
	[RequireIdentityVerification] BIT NOT NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_AuthenticationPolicy] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[AuthenticationPolicy]
ADD CONSTRAINT [UQ_AuthenticationPolicy_Name] UNIQUE ([Name]);
GO

ALTER TABLE [dbo].[AuthenticationPolicy]
ADD CONSTRAINT [DF_AuthenticationPolicy_RequireMultiFactorAuthentication] DEFAULT (0) FOR [RequireMultiFactorAuthentication];
GO

ALTER TABLE [dbo].[AuthenticationPolicy]
ADD CONSTRAINT [DF_AuthenticationPolicy_RequireIdentityVerification] DEFAULT (0) FOR [RequireIdentityVerification];
GO

ALTER TABLE [dbo].[AuthenticationPolicy]
ADD CONSTRAINT [DF_AuthenticationPolicy_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO
