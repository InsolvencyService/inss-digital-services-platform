
-- =========================================================================
-- 1. Assign the Identity Providers
-- =========================================================================
DECLARE @IdentityProviderId UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[IdentityProvider] ([Id], [Name], [IssuerURL], [ClientId], [Created], [CreatedBy], [Modified], [ModifiedBy], [Secret])
VALUES (@IdentityProviderId, 'One Login', 'https://oidc.integration.account.gov.uk', '80GquKbr00-Z3VzhFNoCVi-sZeo', GETDATE(), SUSER_SNAME(), NULL, NULL, NULL);


-- =========================================================================
-- 2. Assign the Applications
-- =========================================================================
DECLARE @ApplicationId_DCRS UNIQUEIDENTIFIER = NEWID();
DECLARE @ApplicationId_IPUS UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[Application] ([Id], [Name], [IdentityProviderId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (@ApplicationId_DCRS, 'Director Conduct Reporting Service', @IdentityProviderId, GETDATE(), SUSER_SNAME(), NULL, NULL);

INSERT INTO [dbo].[Application] ([Id], [Name], [IdentityProviderId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (@ApplicationId_IPUS, 'IP Upload Service', @IdentityProviderId, GETDATE(), SUSER_SNAME(), NULL, NULL);


-- =========================================================================
-- 3. Assign the Roles
-- =========================================================================
DECLARE @RoleId_OfficialReceiver UNIQUEIDENTIFIER = NEWID();
DECLARE @RoleId_InsolvencyPractitioner UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[Role] ([Id], [Name], [Description], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (@RoleId_OfficialReceiver, 'Official Receiver', 'Official Receiver role with full access.', GETDATE(), SUSER_SNAME(), NULL, NULL);

INSERT INTO [dbo].[Role] ([Id], [Name], [Description], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (@RoleId_InsolvencyPractitioner, 'Insolvency Practitioner', 'Insolvency Practitioner role with full access.', GETDATE(), SUSER_SNAME(), NULL, NULL);


-- =========================================================================
-- 4. Link the Applications to the Roles
-- =========================================================================
INSERT INTO [dbo].[ApplicationRole] ([Id], [ApplicationId], [RoleId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (NEWID(), @ApplicationId_DCRS, @RoleId_InsolvencyPractitioner, GETDATE(), SUSER_SNAME(), NULL, NULL);
-- DCRS has an Insolvency Practitioner role.

INSERT INTO [dbo].[ApplicationRole] ([Id], [ApplicationId], [RoleId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (NEWID(), @ApplicationId_DCRS, @RoleId_OfficialReceiver, GETDATE(), SUSER_SNAME(), NULL, NULL);
-- DCRS has an Official Receiver role.

INSERT INTO [dbo].[ApplicationRole] ([Id], [ApplicationId], [RoleId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (NEWID(), @ApplicationId_IPUS, @RoleId_InsolvencyPractitioner, GETDATE(), SUSER_SNAME(), NULL, NULL);
-- IPUS has an Insolvency Practitioner role.

-- =========================================================================
-- 5. Assign the Organisations
-- =========================================================================
DECLARE @OrganisationId_AcmeFinancialGroup UNIQUEIDENTIFIER = NEWID();
DECLARE @OrganisationId_UnityCreditServices UNIQUEIDENTIFIER = NEWID();
DECLARE @OrganisationId_BrightFutureTrust UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[Organisation] ([Id], [Name], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (@OrganisationId_AcmeFinancialGroup, 'Acme Financial Group', GETDATE(), SUSER_SNAME(), NULL, NULL);

INSERT INTO [dbo].[Organisation] ([Id], [Name], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (@OrganisationId_UnityCreditServices, 'Unity Credit Services', GETDATE(), SUSER_SNAME(), NULL, NULL);

INSERT INTO [dbo].[Organisation] ([Id], [Name], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (@OrganisationId_BrightFutureTrust, 'Bright Future Trust', GETDATE(), SUSER_SNAME(), NULL, NULL);


-- =========================================================================
-- 6. Assign the Users
-- =========================================================================
-- NOTE: When the User is created, the UserIdentityId is NULL. The UserIdentity record is created after the User signs in for the first time.
-- User Creation Process:
-- 1. User is created here with UserIdentityId = NULL
-- 2. The user is sent an email with a link to our portal, the link will contain a hashed UserId parameter.
-- 3. The user clicks the link, they are redirected to our portal and asked to sign in / register.
-- 3. We pass the hashed UserId to the Identity Provider as part of the CallBack Url (or can we pass it in the state parameter ?).
-- 4. The user signs in / registers with the Identity Provider.
-- 5. The Identity Provider redirects the user back to our portal with an authorization code and the UserId and the Identity Provider User Id (in the subject claim?).
-- 6. We exchange the authorization code for an access token and ID token.
-- 7. We create a UserIdentity record with the Identity Provider User Id and link it to the User record using the UserId.

DECLARE @UserId_DaveC UNIQUEIDENTIFIER = NEWID();
DECLARE @UserId_GlenS UNIQUEIDENTIFIER = NEWID();
DECLARE @UserId_GaryG UNIQUEIDENTIFIER = NEWID();
DECLARE @UserId_WayneB UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[User] ([Id], [UserIdentityId], [FirstName], [LastName], [Email], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (@UserId_DaveC, NULL, 'David', 'Cook', 'dave.cook@insolvency.gov.uk', GETDATE(), SUSER_SNAME(), NULL, NULL);

INSERT INTO [dbo].[User] ([Id], [UserIdentityId], [FirstName], [LastName], [Email], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (@UserId_GlenS, NULL, 'Glen', 'Stone', 'glen.stone@insolvency.gov.uk', GETDATE(), SUSER_SNAME(), NULL, NULL);

INSERT INTO [dbo].[User] ([Id], [UserIdentityId], [FirstName], [LastName], [Email], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (@UserId_GaryG, NULL, 'Gary', 'Griffiths', 'gary.griffiths@insolvency.gov.uk', GETDATE(), SUSER_SNAME(), NULL, NULL);

INSERT INTO [dbo].[User] ([Id], [UserIdentityId], [FirstName], [LastName], [Email], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (@UserId_WayneB, NULL, 'Wayne', 'Busby', 'wayne.busby@insolvency.gov.uk', GETDATE(), SUSER_SNAME(), NULL, NULL);


-- =========================================================================
-- 7. Link the Users to the Organisations
-- =========================================================================
INSERT INTO [dbo].[OrganisationUser] ([Id], [UserId], [OrganisationId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (NEWID(), @UserId_DaveC, @OrganisationId_AcmeFinancialGroup, GETDATE(), SUSER_SNAME(), NULL, NULL);
--- Dave Cook works at Acme Financial Group

INSERT INTO [dbo].[OrganisationUser] ([Id], [UserId], [OrganisationId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (NEWID(), @UserId_DaveC, @OrganisationId_BrightFutureTrust, GETDATE(), SUSER_SNAME(), NULL, NULL);
--- Dave Cook also works at Bright Future Trust

INSERT INTO [dbo].[OrganisationUser] ([Id], [UserId], [OrganisationId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (NEWID(), @UserId_GlenS, @OrganisationId_AcmeFinancialGroup, GETDATE(), SUSER_SNAME(), NULL, NULL);
--- Glen Stone works at Acme Financial Group

INSERT INTO [dbo].[OrganisationUser] ([Id], [UserId], [OrganisationId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (NEWID(), @UserId_GaryG, @OrganisationId_UnityCreditServices, GETDATE(), SUSER_SNAME(), NULL, NULL);
-- Gary Griffiths works at Unity Credit Services

INSERT INTO [dbo].[OrganisationUser] ([Id], [UserId], [OrganisationId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (NEWID(), @UserId_WayneB, @OrganisationId_UnityCreditServices, GETDATE(), SUSER_SNAME(), NULL, NULL);
-- Wayne Busby works at Unity Credit Services


-- =========================================================================
-- 8. Link the Users & Organisation to the Application & Roles
-- =========================================================================
INSERT INTO [dbo].[OrganisationUserApplicationRole] ([Id], [OrganisationUserId], [ApplicationRoleId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (NEWID(), 
		(SELECT TOP 1 [Id] FROM [dbo].[OrganisationUser] WHERE [UserId] = @UserId_DaveC AND [OrganisationId] = @OrganisationId_AcmeFinancialGroup), 
		(SELECT TOP 1 [Id] FROM [dbo].[ApplicationRole] WHERE [ApplicationId] = @ApplicationId_DCRS AND [RoleId] = @RoleId_OfficialReceiver), 
		GETDATE(), SUSER_SNAME(), NULL, NULL);
		-- Dave Cook works at Acme Financial Group and has the Official Receiver Role within the Director Conduct Reporting Service

INSERT INTO [dbo].[OrganisationUserApplicationRole] ([Id], [OrganisationUserId], [ApplicationRoleId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (NEWID(), 
		(SELECT TOP 1 [Id] FROM [dbo].[OrganisationUser] WHERE [UserId] = @UserId_DaveC AND [OrganisationId] = @OrganisationId_AcmeFinancialGroup), 
		(SELECT TOP 1 [Id] FROM [dbo].[ApplicationRole] WHERE [ApplicationId] = @ApplicationId_IPUS AND [RoleId] = @RoleId_InsolvencyPractitioner), 
		GETDATE(), SUSER_SNAME(), NULL, NULL);
		-- Dave Cook works at Acme Financial Group and has the Insolvency Practitioner Role within the IP Upload Service

INSERT INTO [dbo].[OrganisationUserApplicationRole] ([Id], [OrganisationUserId], [ApplicationRoleId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (NEWID(), 
		(SELECT TOP 1 [Id] FROM [dbo].[OrganisationUser] WHERE [UserId] = @UserId_DaveC AND [OrganisationId] = @OrganisationId_BrightFutureTrust), 
		(SELECT TOP 1 [Id] FROM [dbo].[ApplicationRole] WHERE [ApplicationId] = @ApplicationId_DCRS AND [RoleId] = @RoleId_InsolvencyPractitioner), 
		GETDATE(), SUSER_SNAME(), NULL, NULL);
		-- Dave Cook works at Bright Future Trust and has the Insolvency Practitioner Role within the Director Conduct Reporting Service

INSERT INTO [dbo].[OrganisationUserApplicationRole] ([Id], [OrganisationUserId], [ApplicationRoleId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (NEWID(), 
		(SELECT TOP 1 [Id] FROM [dbo].[OrganisationUser] WHERE [UserId] = @UserId_GlenS AND [OrganisationId] = @OrganisationId_AcmeFinancialGroup), 
		(SELECT TOP 1 [Id] FROM [dbo].[ApplicationRole] WHERE [ApplicationId] = @ApplicationId_DCRS AND [RoleId] = @RoleId_OfficialReceiver), 
		GETDATE(), SUSER_SNAME(), NULL, NULL);
		-- Glen Stone works at Acme Financial Group and has the Official Receiver Role within the Director Conduct Reporting Service

INSERT INTO [dbo].[OrganisationUserApplicationRole] ([Id], [OrganisationUserId], [ApplicationRoleId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (NEWID(), 
		(SELECT TOP 1 [Id] FROM [dbo].[OrganisationUser] WHERE [UserId] = @UserId_GaryG AND [OrganisationId] = @OrganisationId_UnityCreditServices), 
		(SELECT TOP 1 [Id] FROM [dbo].[ApplicationRole] WHERE [ApplicationId] = @ApplicationId_DCRS AND [RoleId] = @RoleId_InsolvencyPractitioner), 
		GETDATE(), SUSER_SNAME(), NULL, NULL);
		-- Gary Griffiths works at Unity Credit Services and has the Insolvency Practitioner Role within the Director Conduct Reporting Service

INSERT INTO [dbo].[OrganisationUserApplicationRole] ([Id], [OrganisationUserId], [ApplicationRoleId], [Created], [CreatedBy], [Modified], [ModifiedBy])
VALUES (NEWID(), 
		(SELECT TOP 1 [Id] FROM [dbo].[OrganisationUser] WHERE [UserId] = @UserId_WayneB AND [OrganisationId] = @OrganisationId_UnityCreditServices), 
		(SELECT TOP 1 [Id] FROM [dbo].[ApplicationRole] WHERE [ApplicationId] = @ApplicationId_DCRS AND [RoleId] = @RoleId_InsolvencyPractitioner), 
		GETDATE(), SUSER_SNAME(), NULL, NULL);
		-- Wayne Busby works at Unity Credit Services and has the Insolvency Practitioner Role within the Director Conduct Reporting Service
