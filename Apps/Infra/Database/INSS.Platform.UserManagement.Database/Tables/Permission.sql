CREATE TABLE [dbo].[Permission]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[Name] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(512) NOT NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[Permission]
ADD CONSTRAINT [UQ_Permission_Name] UNIQUE ([Name]);
GO

ALTER TABLE [dbo].[Permission]
ADD CONSTRAINT [DF_Permission_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO
