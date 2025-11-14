CREATE TABLE [dbo].[Address]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[PartyId] UNIQUEIDENTIFIER NOT NULL,
	[AddressTypeId] UNIQUEIDENTIFIER NOT NULL,
	[AddressLine1] NVARCHAR(255) NOT NULL,
	[AddressLine2] NVARCHAR(255) NOT NULL,
	[AddressLine3] NVARCHAR(255) NOT NULL,
	[Postcode] NVARCHAR(20) NOT NULL,
	[UPRN] BIGINT NULL,
	[Longitude] DECIMAL(9,6) NULL,
	[Latitude] DECIMAL(9,6) NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[Address] ADD CONSTRAINT [FK_Address_Party] FOREIGN KEY([PartyId])
REFERENCES [dbo].[Party] ([Id])
GO

ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_Party]
GO

ALTER TABLE [dbo].[Address] ADD CONSTRAINT [FK_Address_AddressType] FOREIGN KEY([AddressTypeId])
REFERENCES [dbo].[AddressType] ([Id])
GO

ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_AddressType]
GO

ALTER TABLE [dbo].[Address]
ADD CONSTRAINT [DF_Address_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO
