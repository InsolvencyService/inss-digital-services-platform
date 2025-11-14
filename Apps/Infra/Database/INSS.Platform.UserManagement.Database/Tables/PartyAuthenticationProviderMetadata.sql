CREATE TABLE [dbo].[PartyAuthenticationProviderMetadata]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[PartyId] UNIQUEIDENTIFIER NOT NULL,
	[AuthenticationPolicyProviderId] UNIQUEIDENTIFIER NOT NULL,
	[AuthenticationProviderUserId] NVARCHAR(255) NULL,
	[AuthenticationProviderSessionData] NVARCHAR(MAX) NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_PartyAuthenticationProviderMetadata] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[PartyAuthenticationProviderMetadata] ADD CONSTRAINT [FK_PartyAuthenticationProviderMetadata_Party] FOREIGN KEY([PartyId])
REFERENCES [dbo].[Party] ([Id])
GO

ALTER TABLE [dbo].[PartyAuthenticationProviderMetadata] CHECK CONSTRAINT [FK_PartyAuthenticationProviderMetadata_Party]
GO

ALTER TABLE [dbo].[PartyAuthenticationProviderMetadata] ADD CONSTRAINT [FK_PartyAuthenticationProviderMetadata_AuthenticationPolicyProvider] FOREIGN KEY([AuthenticationPolicyProviderId])
REFERENCES [dbo].[AuthenticationPolicyProvider] ([Id])
GO

ALTER TABLE [dbo].[PartyAuthenticationProviderMetadata] CHECK CONSTRAINT [FK_PartyAuthenticationProviderMetadata_AuthenticationPolicyProvider]
GO

ALTER TABLE [dbo].[PartyAuthenticationProviderMetadata]
ADD CONSTRAINT [DF_PartyAuthenticationProviderMetadata_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO
