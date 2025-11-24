CREATE TABLE [dbo].[Organisation]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[PartyId] UNIQUEIDENTIFIER NOT NULL,
	[Name] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(512) NOT NULL,
	[CompanyIdentifier] NVARCHAR(100) NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_Organisation] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[Organisation] ADD CONSTRAINT [FK_Organisation_Party] FOREIGN KEY([PartyId])
REFERENCES [dbo].[Party] ([Id])
GO

ALTER TABLE [dbo].[Organisation] CHECK CONSTRAINT [FK_Organisation_Party]
GO

ALTER TABLE [dbo].[Organisation]
ADD CONSTRAINT [UQ_Organisation_Name] UNIQUE ([Name]);
GO

ALTER TABLE [dbo].[Organisation]
ADD CONSTRAINT [DF_Organisation_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO
