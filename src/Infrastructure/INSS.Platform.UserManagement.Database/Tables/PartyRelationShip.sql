CREATE TABLE [dbo].[PartyRelationShip]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[FromPartyId] UNIQUEIDENTIFIER NOT NULL,
	[ToPartyId] UNIQUEIDENTIFIER NOT NULL,
	[RelationshipTypeId] UNIQUEIDENTIFIER NOT NULL,
	[StartDate] DATETIME2 NOT NULL,
	[EndDate] DATETIME2 NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_PartyRelationShip] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[PartyRelationShip] ADD CONSTRAINT [FK_PartyRelationShip_FromParty] FOREIGN KEY([FromPartyId])
REFERENCES [dbo].[Party] ([Id])
GO

ALTER TABLE [dbo].[PartyRelationShip] CHECK CONSTRAINT [FK_PartyRelationShip_FromParty]
GO

ALTER TABLE [dbo].[PartyRelationShip] ADD CONSTRAINT [FK_PartyRelationShip_ToParty] FOREIGN KEY([ToPartyId])
REFERENCES [dbo].[Party] ([Id])
GO

ALTER TABLE [dbo].[PartyRelationShip] CHECK CONSTRAINT [FK_PartyRelationShip_ToParty]
GO

ALTER TABLE [dbo].[PartyRelationShip] ADD CONSTRAINT [FK_PartyRelationShip_RelationshipType] FOREIGN KEY([RelationshipTypeId])
REFERENCES [dbo].[RelationshipType] ([Id])
GO

ALTER TABLE [dbo].[PartyRelationShip] CHECK CONSTRAINT [FK_PartyRelationShip_RelationshipType]
GO

ALTER TABLE [dbo].[PartyRelationShip]
ADD CONSTRAINT [DF_PartyRelationShip_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO
