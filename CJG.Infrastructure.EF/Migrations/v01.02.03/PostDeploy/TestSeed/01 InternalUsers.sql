PRINT 'Inserting [InternalUsers]'

DECLARE @ScopeIdentity INT

IF (NOT EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = 'lelliott'))
BEGIN
	INSERT [dbo].[InternalUsers]
	 ([FirstName], [LastName],	[IDIR],			[Email]) VALUES
	 (N'Laura',    N'Elliot', N'lelliott', N'laura.elliott@gov.bc.ca')
	 
	SET @ScopeIdentity = SCOPE_IDENTITY()
	INSERT [dbo].[ApplicationUsers]
	 ([InternalUserId], [ApplicationUserId], [Email], [SecurityStamp], [UserName], [Active]) VALUES
	 (@ScopeIdentity, N'3cfc3f41-93c2-4dda-ae0d-bbf7177ff6e8', (SELECT TOP 1 [Email] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), NEWID(), (SELECT TOP 1 [IDIR] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), 1)

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 ((SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [InternalUserId] = @ScopeIdentity), (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (NOT EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = 'machen'))
BEGIN
	INSERT [dbo].[InternalUsers]
	 ([FirstName],	[LastName],		[IDIR],		[Email]) VALUES
	 (N'Maria',		N'Chen',		N'machen',	N'maria.chen@gov.bc.ca')
	 
	SET @ScopeIdentity = SCOPE_IDENTITY()
	INSERT [dbo].[ApplicationUsers]
	 ([InternalUserId], [ApplicationUserId], [Email], [SecurityStamp], [UserName], [Active]) VALUES
	 (@ScopeIdentity, N'd42359d1-b297-46b8-946c-30e52df8c575', (SELECT TOP 1 [Email] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), NEWID(), (SELECT TOP 1 [IDIR] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), 1)

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 ((SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [InternalUserId] = @ScopeIdentity), (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (NOT EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = 'nhowells'))
BEGIN
	INSERT [dbo].[InternalUsers]
	 ([FirstName],	[LastName],		[IDIR],			[Email]) VALUES
	 (N'Nadaya',	N'Howells',		N'nhowells',	N'nadaya.howells@gov.bc.ca')
	 
	SET @ScopeIdentity = SCOPE_IDENTITY()
	INSERT [dbo].[ApplicationUsers]
	 ([InternalUserId], [ApplicationUserId], [Email], [SecurityStamp], [UserName], [Active]) VALUES
	 (@ScopeIdentity, N'72235ded-3045-4a00-8e08-bfdb47957617', (SELECT TOP 1 [Email] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), NEWID(), (SELECT TOP 1 [IDIR] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), 1)

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 ((SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [InternalUserId] = @ScopeIdentity), (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (NOT EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = 'ltabachu'))
BEGIN
	INSERT [dbo].[InternalUsers]
	 ([FirstName],	[LastName],		[IDIR],			[Email]) VALUES
	 (N'Lisa',		N'Tabachuk',		N'ltabachu',	N'lisa.tabachuk@gov.bc.ca')
	 
	SET @ScopeIdentity = SCOPE_IDENTITY()
	INSERT [dbo].[ApplicationUsers]
	 ([InternalUserId], [ApplicationUserId], [Email], [SecurityStamp], [UserName], [Active]) VALUES
	 (@ScopeIdentity, N'51c90c85-3a04-4f6c-84d6-a23701d089c5', (SELECT TOP 1 [Email] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), NEWID(), (SELECT TOP 1 [IDIR] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), 1)

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 ((SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [InternalUserId] = @ScopeIdentity), (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (NOT EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = 'couwilso'))
BEGIN
	INSERT [dbo].[InternalUsers]
	 ([FirstName],	[LastName],		[IDIR],			[Email]) VALUES
	 (N'Courtenay',	N'Wilson',		N'couwilso',	N'courtenay.wilson@gov.bc.ca')
	 
	SET @ScopeIdentity = SCOPE_IDENTITY()
	INSERT [dbo].[ApplicationUsers]
	 ([InternalUserId], [ApplicationUserId], [Email], [SecurityStamp], [UserName], [Active]) VALUES
	 (@ScopeIdentity, N'b7cefa1a-e0fa-4155-a8f6-a6331252740c', (SELECT TOP 1 [Email] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), NEWID(), (SELECT TOP 1 [IDIR] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), 1)

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 ((SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [InternalUserId] = @ScopeIdentity), (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (NOT EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = 'gcasault'))
BEGIN
	INSERT [dbo].[InternalUsers]
	 ([FirstName],	[LastName],		[IDIR],			[Email]) VALUES
	 (N'Genevieve',	N'Casault',		N'gcasault',	N'genevieve.casault@gov.bc.ca')
	 
	SET @ScopeIdentity = SCOPE_IDENTITY()
	INSERT [dbo].[ApplicationUsers]
	 ([InternalUserId], [ApplicationUserId], [Email], [SecurityStamp], [UserName], [Active]) VALUES
	 (@ScopeIdentity, N'ce7e601e-80b2-4436-a46f-52e3711c1987', (SELECT TOP 1 [Email] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), NEWID(), (SELECT TOP 1 [IDIR] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), 1)

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 ((SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [InternalUserId] = @ScopeIdentity), (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (NOT EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = 'rmwood'))
BEGIN
	INSERT [dbo].[InternalUsers]
	 ([FirstName],	[LastName],		[IDIR],			[Email]) VALUES
	 (N'Robyn',		N'Wood',		N'rmwood',	N'robyn.wood@gov.bc.ca')
	 
	SET @ScopeIdentity = SCOPE_IDENTITY()
	INSERT [dbo].[ApplicationUsers]
	 ([InternalUserId], [ApplicationUserId], [Email], [SecurityStamp], [UserName], [Active]) VALUES
	 (@ScopeIdentity, N'74edd439-defc-463d-96b1-3e5b797217df', (SELECT TOP 1 [Email] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), NEWID(), (SELECT TOP 1 [IDIR] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), 1)

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 ((SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [InternalUserId] = @ScopeIdentity), (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (NOT EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = 'cclarke'))
BEGIN
	INSERT [dbo].[InternalUsers]
	 ([FirstName],	[LastName],		[IDIR],			[Email]) VALUES
	 (N'Chris',		N'Clarke',		N'cclarke',	N'chris.clarke@gov.bc.ca')
	 
	SET @ScopeIdentity = SCOPE_IDENTITY()
	INSERT [dbo].[ApplicationUsers]
	 ([InternalUserId], [ApplicationUserId], [Email], [SecurityStamp], [UserName], [Active]) VALUES
	 (@ScopeIdentity, N'f5d167fc-119e-4734-81b5-e3e85358db0d', (SELECT TOP 1 [Email] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), NEWID(), (SELECT TOP 1 [IDIR] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), 1)

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 ((SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [InternalUserId] = @ScopeIdentity), (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (NOT EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = 'nturture'))
BEGIN
	INSERT [dbo].[InternalUsers]
	 ([FirstName],	[LastName],		[IDIR],			[Email]) VALUES
	 (N'Nicoleta',	N'Turtureanu',		N'nturture',	N'nicoleta.turtureanu@gov.bc.ca')
	 
	SET @ScopeIdentity = SCOPE_IDENTITY()
	INSERT [dbo].[ApplicationUsers]
	 ([InternalUserId], [ApplicationUserId], [Email], [SecurityStamp], [UserName], [Active]) VALUES
	 (@ScopeIdentity, N'19086edf-737c-4966-b7f2-d4ed4970d37b', (SELECT TOP 1 [Email] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), NEWID(), (SELECT TOP 1 [IDIR] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), 1)

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 ((SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [InternalUserId] = @ScopeIdentity), (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (NOT EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = 'ebaydar'))
BEGIN
	INSERT [dbo].[InternalUsers]
	 ([FirstName],	[LastName],		[IDIR],			[Email]) VALUES
	 (N'Erhan',		N'Baydar',		N'ebaydar',	N'erhan.baydar@gov.bc.ca')
	 
	SET @ScopeIdentity = SCOPE_IDENTITY()
	INSERT [dbo].[ApplicationUsers]
	 ([InternalUserId], [ApplicationUserId], [Email], [SecurityStamp], [UserName], [Active]) VALUES
	 (@ScopeIdentity, N'ead65bc3-86ca-4f35-a83b-e6fad9b42566', (SELECT TOP 1 [Email] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), NEWID(), (SELECT TOP 1 [IDIR] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), 1)

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 ((SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [InternalUserId] = @ScopeIdentity), (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (NOT EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = 'mbeaubie'))
BEGIN
	INSERT [dbo].[InternalUsers]
	 ([FirstName],	[LastName],		[IDIR],			[Email]) VALUES
	 (N'Michelle',	N'Beaubien',		N'mbeaubie',	N'michelle.beaubien@gov.bc.ca')
	 
	SET @ScopeIdentity = SCOPE_IDENTITY()
	INSERT [dbo].[ApplicationUsers]
	 ([InternalUserId], [ApplicationUserId], [Email], [SecurityStamp], [UserName], [Active]) VALUES
	 (@ScopeIdentity, N'2c103774-39a3-49e5-a1de-a0540a5b7f70', (SELECT TOP 1 [Email] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), NEWID(), (SELECT TOP 1 [IDIR] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), 1)

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 ((SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [InternalUserId] = @ScopeIdentity), (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (NOT EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = 'mamason'))
BEGIN
	INSERT [dbo].[InternalUsers]
	 ([FirstName],	[LastName],		[IDIR],			[Email]) VALUES
	 (N'Matthew',	N'Mason',		N'mamason',	N'matthew.mason@fcvinteractive.com')
	 
	SET @ScopeIdentity = SCOPE_IDENTITY()
	INSERT [dbo].[ApplicationUsers]
	 ([InternalUserId], [ApplicationUserId], [Email], [SecurityStamp], [UserName], [Active]) VALUES
	 (@ScopeIdentity, N'a2ede42f-e4df-48a7-8e06-acdc174a60af', (SELECT TOP 1 [Email] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), NEWID(), (SELECT TOP 1 [IDIR] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), 1)

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 ((SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [InternalUserId] = @ScopeIdentity), (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (NOT EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = 'rssamra'))
BEGIN
	INSERT [dbo].[InternalUsers]
	 ([FirstName],	[LastName],		[IDIR],			[Email]) VALUES
	 (N'Raman',		N'Samra',		N'rssamra',	N'raman.samra@avocette.com')
	 
	SET @ScopeIdentity = SCOPE_IDENTITY()
	INSERT [dbo].[ApplicationUsers]
	 ([InternalUserId], [ApplicationUserId], [Email], [SecurityStamp], [UserName], [Active]) VALUES
	 (@ScopeIdentity, N'43a4dcd7-f69b-407b-8667-1a681c24d570', (SELECT TOP 1 [Email] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), NEWID(), (SELECT TOP 1 [IDIR] FROM [dbo].[InternalUsers] WHERE [Id] = @ScopeIdentity), 1)

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 ((SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [InternalUserId] = @ScopeIdentity), (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

PRINT 'Updating [InternalUsers]'

IF (EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = N'lelliott'))
BEGIN
	DELETE FROM [dbo].[ApplicationUserRoles] WHERE [UserId] = (SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [UserName] = N'lelliott')

	UPDATE [dbo].[ApplicationUsers] 
	SET [ApplicationUserId] = N'3cfc3f41-93c2-4dda-ae0d-bbf7177ff6e8'
	WHERE [UserName] = N'lelliott'

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 (N'3cfc3f41-93c2-4dda-ae0d-bbf7177ff6e8', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = N'machen'))
BEGIN
	DELETE FROM [dbo].[ApplicationUserRoles] WHERE [UserId] = (SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [UserName] = N'machen')

	UPDATE [dbo].[ApplicationUsers] 
	SET [ApplicationUserId] = N'd42359d1-b297-46b8-946c-30e52df8c575'
	WHERE [UserName] = N'machen'

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 (N'd42359d1-b297-46b8-946c-30e52df8c575', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END


IF (EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = N'nhowells'))
BEGIN
	DELETE FROM [dbo].[ApplicationUserRoles] WHERE [UserId] = (SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [UserName] = N'nhowells')

	UPDATE [dbo].[ApplicationUsers] 
	SET [ApplicationUserId] = N'72235ded-3045-4a00-8e08-bfdb47957617'
	WHERE [UserName] = N'nhowells'

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 (N'72235ded-3045-4a00-8e08-bfdb47957617', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END


IF (EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = N'ltabachu'))
BEGIN
	DELETE FROM [dbo].[ApplicationUserRoles] WHERE [UserId] = (SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [UserName] = N'ltabachu')

	UPDATE [dbo].[ApplicationUsers] 
	SET [ApplicationUserId] = N'51c90c85-3a04-4f6c-84d6-a23701d089c5'
	WHERE [UserName] = N'ltabachu'

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 (N'51c90c85-3a04-4f6c-84d6-a23701d089c5', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = N'couwilso'))
BEGIN
	DELETE FROM [dbo].[ApplicationUserRoles] WHERE [UserId] = (SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [UserName] = N'couwilso')

	UPDATE [dbo].[ApplicationUsers] 
	SET [ApplicationUserId] = N'b7cefa1a-e0fa-4155-a8f6-a6331252740c'
	WHERE [UserName] = N'couwilso'

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 (N'b7cefa1a-e0fa-4155-a8f6-a6331252740c', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = N'gcasault'))
BEGIN
	DELETE FROM [dbo].[ApplicationUserRoles] WHERE [UserId] = (SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [UserName] = N'gcasault')

	UPDATE [dbo].[ApplicationUsers] 
	SET [ApplicationUserId] = N'ce7e601e-80b2-4436-a46f-52e3711c1987'
	WHERE [UserName] = N'gcasault'

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 (N'ce7e601e-80b2-4436-a46f-52e3711c1987', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = N'rmwood'))
BEGIN
	DELETE FROM [dbo].[ApplicationUserRoles] WHERE [UserId] = (SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [UserName] = N'rmwood')

	UPDATE [dbo].[ApplicationUsers] 
	SET [ApplicationUserId] = N'74edd439-defc-463d-96b1-3e5b797217df'
	WHERE [UserName] = N'rmwood'

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 (N'74edd439-defc-463d-96b1-3e5b797217df', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = N'cclarke'))
BEGIN
	DELETE FROM [dbo].[ApplicationUserRoles] WHERE [UserId] = (SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [UserName] = N'cclarke')

	UPDATE [dbo].[ApplicationUsers] 
	SET [ApplicationUserId] = N'f5d167fc-119e-4734-81b5-e3e85358db0d'
	WHERE [UserName] = N'cclarke'

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 (N'f5d167fc-119e-4734-81b5-e3e85358db0d', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = N'nturture'))
BEGIN
	DELETE FROM [dbo].[ApplicationUserRoles] WHERE [UserId] = (SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [UserName] = N'nturture')

	UPDATE [dbo].[ApplicationUsers] 
	SET [ApplicationUserId] = N'19086edf-737c-4966-b7f2-d4ed4970d37b'
	WHERE [UserName] = N'nturture'

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 (N'19086edf-737c-4966-b7f2-d4ed4970d37b', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = N'ebaydar'))
BEGIN
	DELETE FROM [dbo].[ApplicationUserRoles] WHERE [UserId] = (SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [UserName] = N'ebaydar')

	UPDATE [dbo].[ApplicationUsers] 
	SET [ApplicationUserId] = N'ead65bc3-86ca-4f35-a83b-e6fad9b42566'
	WHERE [UserName] = N'ebaydar'

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 (N'ead65bc3-86ca-4f35-a83b-e6fad9b42566', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = N'mbeaubie'))
BEGIN
	DELETE FROM [dbo].[ApplicationUserRoles] WHERE [UserId] = (SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [UserName] = N'mbeaubie')

	UPDATE [dbo].[ApplicationUsers] 
	SET [ApplicationUserId] = N'2c103774-39a3-49e5-a1de-a0540a5b7f70'
	WHERE [UserName] = N'mbeaubie'

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 (N'2c103774-39a3-49e5-a1de-a0540a5b7f70', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = N'mamason'))
BEGIN
	DELETE FROM [dbo].[ApplicationUserRoles] WHERE [UserId] = (SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [UserName] = N'mamason')

	UPDATE [dbo].[ApplicationUsers] 
	SET [ApplicationUserId] = N'a2ede42f-e4df-48a7-8e06-acdc174a60af'
	WHERE [UserName] = N'mamason'

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 (N'a2ede42f-e4df-48a7-8e06-acdc174a60af', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = N'rssamra'))
BEGIN
	DELETE FROM [dbo].[ApplicationUserRoles] WHERE [UserId] = (SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [UserName] = N'rssamra')

	UPDATE [dbo].[ApplicationUsers] 
	SET [ApplicationUserId] = N'43a4dcd7-f69b-407b-8667-1a681c24d570'
	WHERE [UserName] = N'rssamra'

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 (N'43a4dcd7-f69b-407b-8667-1a681c24d570', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = N'bohastin'))
BEGIN
	DELETE FROM [dbo].[ApplicationUserRoles] WHERE [UserId] = (SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [UserName] = N'bohastin')

	UPDATE [dbo].[ApplicationUsers] 
	SET [ApplicationUserId] = N'0a4c81af-f619-4a0f-8c68-0b6c37ccdf0a'
	WHERE [UserName] = N'bohastin'

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 (N'0a4c81af-f619-4a0f-8c68-0b6c37ccdf0a', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END

IF (EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = N'kmoyer'))
BEGIN
	DELETE FROM [dbo].[ApplicationUserRoles] WHERE [UserId] = (SELECT TOP 1 [ApplicationUserId] FROM [dbo].[ApplicationUsers] WHERE [UserName] = N'kmoyer')

	UPDATE [dbo].[ApplicationUsers] 
	SET [ApplicationUserId] = N'437d4b4a-32fa-46a7-a818-a48d08b9a355'
	WHERE [UserName] = N'kmoyer'

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 (N'437d4b4a-32fa-46a7-a818-a48d08b9a355', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'), N'IdentityUserRole')
END