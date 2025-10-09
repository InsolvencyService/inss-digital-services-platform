CREATE TABLE [dbo].[Organisation]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[Name] NVARCHAR(255) NOT NULL,
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

ALTER TABLE [dbo].[Organisation]
ADD CONSTRAINT [DF_Organisation_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO

ALTER TABLE [dbo].[Organisation]
ADD CONSTRAINT [UQ_Organisation_Name] UNIQUE ([Name]);
GO
