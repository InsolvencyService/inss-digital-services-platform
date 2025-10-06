CREATE PROCEDURE [dbo].[EnsureUserIsRegistered]
(
	@userId UNIQUEIDENTIFIER
,	@identityProviderUserId UNIQUEIDENTIFIER 
,	@userIdentityId UNIQUEIDENTIFIER
,	@identityProviderId UNIQUEIDENTIFIER
,	@userName NVARCHAR(255)
)
AS

BEGIN
	-- Ensure the user exists
	IF NOT EXISTS (
		SELECT	1
		FROM [dbo].[User]
		WHERE Id = @UserId
	)
	BEGIN
		RAISERROR('User does not exist.', 16, 1);
		RETURN 1;
	END

	-- Ensure the user does not exist with a different @identityProviderUserId
	IF EXISTS (
		SELECT	1
		FROM	[dbo].[User] U LEFT JOIN [dbo].[UserIdentity] UI
					ON U.UserIdentityId = UI.Id
		WHERE	U.Id = @UserId
				AND UI.IdentityProviderUserId IS NOT NULL
				AND UI.IdentityProviderUserId <> @identityProviderUserId
	)
	BEGIN
		RAISERROR('User exists with a different Identity Provider User Id.', 16, 1);
		RETURN 1;
	END
				
	-- Check if the provider user identity has been linked to the user.
	IF NOT EXISTS (
		SELECT	1
		FROM	[dbo].[User] U LEFT JOIN [dbo].[UserIdentity] UI
					ON U.UserIdentityId = UI.Id
		WHERE	U.Id = @UserId
				AND UI.Id IS NOT NULL
	)
	BEGIN

		-- Create the UserIdentity record and link it to the user.
		DECLARE @TimeNow DATETIME2 = GETUTCDATE()

		INSERT	[dbo].[UserIdentity] ([Id], [IdentityProviderUserId], [IdentityProviderId], [Created], [CreatedBy], [Modified], [ModifiedBy])
		VALUES	(@userIdentityId, @identityProviderUserId, @identityProviderId, @TimeNow, @userName, @TimeNow, @userName)

		UPDATE	[dbo].[User]
		SET		UserIdentityId = @userIdentityId
		WHERE	Id = @UserId

		PRINT 'User Identity Has Been Created'
	END
	ELSE
		PRINT 'The User Identity Already Exists'

END

RETURN 0

