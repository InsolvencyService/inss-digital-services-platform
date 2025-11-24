CREATE TABLE [dbo].[Individual]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[PartyId] UNIQUEIDENTIFIER NULL,
	[FirstName] NVARCHAR(255) NOT NULL,
	[LastName] NVARCHAR(255) NOT NULL,
	[DateOfBirth] DATE NULL,
	[NationalInsuranceNumber] CHAR(9) NULL,
	[IsIdentityVerified] BIT NOT NULL,
	[VerificationSource] NVARCHAR(255) NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_Individual] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[Individual] ADD CONSTRAINT [FK_Individual_Party] FOREIGN KEY([PartyId])
REFERENCES [dbo].[Party] ([Id])
GO

ALTER TABLE [dbo].[Individual] CHECK CONSTRAINT [FK_Individual_Party]
GO

ALTER TABLE [dbo].[Individual]
ADD CONSTRAINT [DF_Individual_IsIdentityVerified] DEFAULT (0) FOR [IsIdentityVerified];
GO

ALTER TABLE [dbo].[Individual]
ADD CONSTRAINT [DF_Individual_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO
