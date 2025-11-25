CREATE TABLE [dbo].[ProductAuthenticationPolicyProvider]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[ProductId] UNIQUEIDENTIFIER NOT NULL,
	[AuthenticationPolicyProviderId] UNIQUEIDENTIFIER NOT NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_ProductAuthenticationPolicyProvider] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[ProductAuthenticationPolicyProvider] ADD CONSTRAINT [FK_ProductAuthenticationPolicyProvider_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO

ALTER TABLE [dbo].[ProductAuthenticationPolicyProvider] CHECK CONSTRAINT [FK_ProductAuthenticationPolicyProvider_Product]
GO

ALTER TABLE [dbo].[ProductAuthenticationPolicyProvider] ADD CONSTRAINT [FK_ProductAuthenticationPolicyProvider_AuthenticationPolicyProvider] FOREIGN KEY([AuthenticationPolicyProviderId])
REFERENCES [dbo].[AuthenticationPolicyProvider] ([Id])
GO

ALTER TABLE [dbo].[ProductAuthenticationPolicyProvider] CHECK CONSTRAINT [FK_ProductAuthenticationPolicyProvider_AuthenticationPolicyProvider]
GO

ALTER TABLE [dbo].[ProductAuthenticationPolicyProvider]
ADD CONSTRAINT [DF_ProductAuthenticationPolicyProvider_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO
