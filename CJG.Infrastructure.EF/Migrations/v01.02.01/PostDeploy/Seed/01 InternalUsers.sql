PRINT 'Inserting [InternalUsers]'

IF (NOT EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = 'jefoster'))
BEGIN
	 
	DECLARE @v1_2_1_administratorRole nvarchar(100) 
	SELECT @v1_2_1_administratorRole = Id FROM [dbo].[ApplicationRoles] WHERE Name='System Administrator'

	INSERT [dbo].[InternalUsers]
	 ([FirstName], [LastName], [IDIR], [Email]) VALUES
	 (N'Jeremy',    N'Foster',      N'jefoster', N'jfoster@fosol.ca')
	 
	INSERT [dbo].[ApplicationUsers]
	 ([InternalUserId], [ApplicationUserId], [Email], [SecurityStamp], [UserName], [Active]) VALUES
	 (SCOPE_IDENTITY(), N'5cc22b21-d0a2-498c-a78d-b9e1bcc91967', N'jfoster@fosol.ca',                   N'6aead52d-287d-425b-b155-2e48c829e220', N'jefoster', 1)

	INSERT [dbo].[ApplicationUserRoles]
	 ([UserId], [RoleId], [Discriminator]) VALUES
	 (N'5cc22b21-d0a2-498c-a78d-b9e1bcc91967', @v1_2_1_administratorRole, N'IdentityUserRole')
 END