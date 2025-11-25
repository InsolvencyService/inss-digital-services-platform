CREATE TABLE [dbo].[AuthenticationPolicyProvider]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[AuthenticationPolicyId] UNIQUEIDENTIFIER NOT NULL,
	[AuthenticationProviderId] UNIQUEIDENTIFIER NOT NULL,
	[Created] DATETIME2 NOT NULL,
	[CreatedBy] NVARCHAR(255) NOT NULL,
	[Modified] DATETIME2 NULL,
	[ModifiedBy] NVARCHAR(255) NULL,
	CONSTRAINT [PK_AuthenticationPolicyProvider] PRIMARY KEY CLUSTERED 
	(
		[Id]
	) 
)
GO

ALTER TABLE [dbo].[AuthenticationPolicyProvider] ADD CONSTRAINT [FK_AuthenticationPolicyProvider_AuthenticationPolicy] FOREIGN KEY([AuthenticationPolicyId])
REFERENCES [dbo].[AuthenticationPolicy] ([Id])
GO

ALTER TABLE [dbo].[AuthenticationPolicyProvider] CHECK CONSTRAINT [FK_AuthenticationPolicyProvider_AuthenticationPolicy]
GO

ALTER TABLE [dbo].[AuthenticationPolicyProvider] ADD CONSTRAINT [FK_AuthenticationPolicyProvider_AuthenticationProvider] FOREIGN KEY([AuthenticationProviderId])
REFERENCES [dbo].[AuthenticationProvider] ([Id])
GO

ALTER TABLE [dbo].[AuthenticationPolicyProvider] CHECK CONSTRAINT [FK_AuthenticationPolicyProvider_AuthenticationProvider]
GO

ALTER TABLE [dbo].[AuthenticationPolicyProvider]
ADD CONSTRAINT [DF_AuthenticationPolicyProvider_Created] DEFAULT (GETUTCDATE()) FOR [Created];
GO
