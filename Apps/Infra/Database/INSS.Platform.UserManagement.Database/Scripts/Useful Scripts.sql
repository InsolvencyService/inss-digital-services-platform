DECLARE @UserId UNIQUEIDENTIFIER = (SELECT Id FROM [dbo].[User] WHERE Email = 'dave.cook@insolvency.gov.uk')
,		@identityProviderId UNIQUEIDENTIFIER = (SELECT Id FROM [dbo].[IdentityProvider] WHERE Name = 'One Login')


SELECT	U.[Id] AS [UserId]
,		U.[FirstName]
,		U.[LastName]
,		U.[Email]
,		O.[Id] AS [OrganisationId]
,		O.[Name] AS [Organisation]
FROM	[dbo].[User] U INNER JOIN [dbo].[OrganisationUser] OU
			ON U.[Id] = OU.[UserId]
		INNER JOIN [dbo].[Organisation] O
			ON O.[Id] = OU.[OrganisationId]
WHERE	OU.[UserId] = @UserId
ORDER BY O.[Name]

SELECT	A.[Id] AS [ApplicationId] 
,		A.[Name] AS [Application]
FROM	[dbo].[Application] A 
WHERE	A.[IdentityProviderId] = @identityProviderId
ORDER BY A.[Name]

SELECT	A.[Id] AS [ApplicationId] 
,		A.[Name] AS [Application]
,		R.[Id] AS [RoleId]
,		R.[Name] AS [Role]
,		R.[Description] AS [RoleDescription]
FROM	[dbo].[Application] A INNER JOIN [dbo].[ApplicationRole] AR
			ON A.[Id] = AR.[ApplicationId]
		INNER JOIN [dbo].[Role] R
			ON R.[Id] = AR.[RoleId]
WHERE	A.[IdentityProviderId] = @identityProviderId
ORDER BY A.[Name]


