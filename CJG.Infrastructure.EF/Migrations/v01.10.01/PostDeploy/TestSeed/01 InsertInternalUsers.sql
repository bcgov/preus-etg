PRINT 'Inserting [InternalUsers]'

INSERT [dbo].[InternalUsers]
 ([FirstName],	[LastName],		[IDIR],			[Email],					[DateAdded]) VALUES
-- Business Users
 (N'Ian',		N'Campbell',    N'IMCAMPBE',	N'ian.campbell@gov.bc.ca',	GETUTCDATE())

DECLARE @UserId INT = @@IDENTITY

PRINT 'Inserting [ApplicationUsers]'

INSERT [dbo].[AspNetUsers]
 ([InternalUserId], [ApplicationUserId],						[Email],					[SecurityStamp],							[UserName],		[Active]) VALUES

-- Internal UAT Users
 (@UserId,			N'17349028-6ed0-47b4-b0c2-d9353930f598',	N'ian.campbell@gov.bc.ca',	N'14159367-9a50-405c-9a30-e7f857cd9ee9',	N'IMCAMPBE',	1)


PRINT 'Inserting [ApplicationUserRoles]'

INSERT [dbo].[AspNetUserRoles]
 ([UserId],									[RoleId],																				[Discriminator]) VALUES

-- Internal UAT Users
 (N'17349028-6ed0-47b4-b0c2-d9353930f598', (SELECT TOP 1 [Id] FROM [dbo].[AspNetRoles] WHERE [Name] = N'System Administrator'), N'IdentityUserRole')
