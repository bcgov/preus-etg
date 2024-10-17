PRINT 'Inserting [InternalUsers]'
	 
DECLARE @v1_3_0_administratorRole nvarchar(100) 
SELECT @v1_3_0_administratorRole = Id FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'

IF (NOT EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = 'amantel'))
BEGIN
	INSERT [dbo].[InternalUsers]
	 ([FirstName],	[LastName],		[IDIR],			[Email]) VALUES
	 (N'Anna',		N'Mantel',      N'amantel',		N'anna.mantel@avocette.com')
	 
	INSERT [dbo].[ApplicationUsers]
	 ([InternalUserId], [ApplicationUserId],						[Email],						[SecurityStamp],							[UserName],		[Active]) VALUES
	 (SCOPE_IDENTITY(), N'7c7a13b4-8273-4a6d-a6f5-136f06688299',	N'anna.mantel@avocette.com',	NEWID(),	N'amantel',		1)

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId],									[RoleId],					[Discriminator]) VALUES
	 (N'7c7a13b4-8273-4a6d-a6f5-136f06688299',	@v1_3_0_administratorRole,	N'IdentityUserRole')
END
  
IF (NOT EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = 'gpovoroz'))
BEGIN
	INSERT [dbo].[InternalUsers]
	 ([FirstName],	[LastName],				[IDIR],			[Email]) VALUES
	 (N'George',    N'Povorozniouk',		N'gpovoroz',	N'george.povorozniouk@avocette.com')
	 
	INSERT [dbo].[ApplicationUsers]
	 ([InternalUserId], [ApplicationUserId],						[Email],								[SecurityStamp],							[UserName],		[Active]) VALUES
	 (SCOPE_IDENTITY(), N'9e7c4d71-2c46-40dd-8089-b5d31b591842',	N'george.povorozniouk@avocette.com',	NEWID(),	N'gpovoroz',	1)

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId],									[RoleId],					[Discriminator]) VALUES
	 (N'9e7c4d71-2c46-40dd-8089-b5d31b591842',	@v1_3_0_administratorRole,	N'IdentityUserRole')
END