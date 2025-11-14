CREATE TABLE [dbo].[RelationshipType]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[Name] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(512) NOT NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_RelationshipType] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[RelationshipType]
ADD CONSTRAINT [UQ_RelationshipType] UNIQUE ([Name]);
GO

ALTER TABLE [dbo].[RelationshipType]
ADD CONSTRAINT [DF_RelationshipType_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO
