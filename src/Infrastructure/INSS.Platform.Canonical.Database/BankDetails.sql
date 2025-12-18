CREATE TABLE [dbo].[BankDetails]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[InstanceId] UNIQUEIDENTIFIER NOT NULL,
	[UserId] UNIQUEIDENTIFIER NOT NULL,
	[AccountName] NVARCHAR(255) NOT NULL,
	[SortCode] CHAR(6) NOT NULL,
	[AccountNumber] VARCHAR(8) NOT NULL,
	[BuildingSocietyRollNumber] VARCHAR(20) NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_BankDetails] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[BankDetails] ADD CONSTRAINT [FK_BankDetails_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[BankDetails] CHECK CONSTRAINT [FK_BankDetails_User]
GO


ALTER TABLE [dbo].[BankDetails]
ADD CONSTRAINT [DF_BankDetails_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO

