CREATE TABLE [dbo].[RoleMetadata]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[RoleId] UNIQUEIDENTIFIER NOT NULL,
	[Name] NVARCHAR(255) NOT NULL,
	[Value] NVARCHAR(255) NOT NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_RoleMetadata] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[RoleMetadata] ADD CONSTRAINT [FK_RoleMetadata_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO

ALTER TABLE [dbo].[RoleMetadata] CHECK CONSTRAINT [FK_RoleMetadata_Role]
GO

ALTER TABLE [dbo].[RoleMetadata]
ADD CONSTRAINT [UQ_RoleMetadata_Name] UNIQUE ([Name]);
GO

ALTER TABLE [dbo].[RoleMetadata]
ADD CONSTRAINT [DF_RoleMetadata_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO
