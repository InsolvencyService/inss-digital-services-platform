CREATE PROCEDURE [dbo].[GetUserOrganisationsApplicationsAndRoles]
(
	@userId UNIQUEIDENTIFIER
,	@identityProviderId UNIQUEIDENTIFIER
)
AS

BEGIN

	SELECT	U.[Id] AS [UserId]
	,		U.[FirstName]
	,		U.[LastName]
	,		U.[Email] AS [User]
	,		O.[Id] AS [OrganisationId]
	,		O.[Name] AS [Organisation]
	,		A.[Id] AS [ApplicationId]
	,		A.[Name] AS [Application]
	,		R.[Id] AS [RoleId]
	,		R.[Name] AS [Role]
	FROM	[dbo].[User] U 
			INNER JOIN [dbo].[UserIdentity] UI
				ON U.[UserIdentityId] = UI.[Id]
			INNER JOIN [dbo].[OrganisationUser] OU
				ON U.[Id] = OU.[UserId]
			INNER JOIN [dbo].[Organisation] O 
				ON O.[Id] = OU.[OrganisationId]
			INNER JOIN [dbo].[OrganisationUserApplicationRole] OUAR
				ON OU.[Id] = OUAR.[OrganisationUserId]
			INNER JOIN [dbo].[ApplicationRole] AR
				ON AR.[Id] = OUAR.[ApplicationRoleId]
			INNER JOIN [dbo].[Application] A
				ON A.[Id] = AR.[ApplicationId]
			INNER JOIN [dbo].[Role] R
				ON R.Id = AR.[RoleId]
	WHERE	OU.[UserId] = @UserId
			AND UI.[IdentityProviderId] = @identityProviderId
	ORDER BY 
			O.[Id], U.[Id], A.[Id], R.[Id]

END

RETURN 0
