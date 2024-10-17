PRINT 'Inserting [InternalUsers]'

DECLARE @ScopeIdentity INT

IF (NOT EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = 'bohastin'))
BEGIN
	INSERT [dbo].[InternalUsers]
	 ([FirstName], [LastName],	[IDIR],			[Email]) VALUES
	 (N'Bonny',    N'Hastings', N'bohastin', N'bonny.hastings@gov.bc.ca')
	 
	SET @ScopeIdentity = SCOPE_IDENTITY()
	INSERT [dbo].[ApplicationUsers]
	 ([InternalUserId], [ApplicationUserId], [Email], [SecurityStamp], [UserName], [Active]) VALUES
	 (@ScopeIdentity, N'0a4c81af-f619-4a0f-8c68-0b6c37ccdf0a', (SELECT TOP 1 [Email] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), NEWID(), (SELECT TOP 1 [IDIR] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), 1)

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 ((SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [InternalUserId] = @ScopeIdentity), (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (NOT EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = 'kmoyer'))
BEGIN
	INSERT [dbo].[InternalUsers]
	 ([FirstName],	[LastName],		[IDIR],		[Email]) VALUES
	 (N'Kate',		N'Moyer',		N'kmoyer',	N'kate.moyer@gov.bc.ca')
	 
	SET @ScopeIdentity = SCOPE_IDENTITY()
	INSERT [dbo].[ApplicationUsers]
	 ([InternalUserId], [ApplicationUserId], [Email], [SecurityStamp], [UserName], [Active]) VALUES
	 (@ScopeIdentity, N'437d4b4a-32fa-46a7-a818-a48d08b9a355', (SELECT TOP 1 [Email] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), NEWID(), (SELECT TOP 1 [IDIR] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), 1)

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 ((SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [InternalUserId] = @ScopeIdentity), (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (NOT EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = 'jefoster'))
BEGIN
	INSERT [dbo].[InternalUsers]
	 ([FirstName],	[LastName],		[IDIR],			[Email]) VALUES
	 (N'Jeremy',	N'Foster',		N'jefoster',	N'jeremy.foster@avocette.com')
	 
	SET @ScopeIdentity = SCOPE_IDENTITY()
	INSERT [dbo].[ApplicationUsers]
	 ([InternalUserId], [ApplicationUserId], [Email], [SecurityStamp], [UserName], [Active]) VALUES
	 (@ScopeIdentity, N'5cc22b21-d0a2-498c-a78d-b9e1bcc91967', (SELECT TOP 1 [Email] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), NEWID(), (SELECT TOP 1 [IDIR] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), 1)

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 ((SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [InternalUserId] = @ScopeIdentity), (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END