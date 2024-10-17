PRINT 'Updating [InternalUsers]'

-- Deactivate internal users accounts no longer in use.
UPDATE u
SET
	u.Active = 0
FROM (
	SELECT
		au.[ApplicationUserId]
		, au.[Active]
	FROM dbo.[InternalUsers] iu
	INNER JOIN dbo.[AspNetUsers] au ON iu.Id = au.InternalUserId
	WHERE au.[Active] = 1
		AND iu.[IDIR] IN ('bohastin', 'IMCAMPBE', 'kmoyer', 'rssamra', 'vanmccol', 'icaesar', 'amantel', 'sbhojwan', 'gcasault', 'cclarke')
) u


IF (NOT EXISTS (SELECT 1 FROM dbo.[InternalUsers] WHERE [IDIR] = 'TYWOOLLE'))
BEGIN
	-- Add Tyler Woolley to the default internal users.
	INSERT dbo.[InternalUsers] (
		[IDIR]
		, [FirstName]
		, [LastName]
		, [Salutation]
		, [Email]
		, [PhoneNumber]
		, [DateAdded]
	) VALUES (
		N'TYWOOLLE'
		, N'Tyler'
		, N'Woolley'
		, N'Mr.'
		, N'tyler.woolley@gov.bc.ca'
		, N'123-123-1234'
		, GETUTCDATE()
	)

	-- Create a user account for Tyler.
	INSERT dbo.[AspNetUsers] (
		[ApplicationUserId]
		, [InternalUserId]
		, [Active]
		, [UserName]
		, [Email]
		, [SecurityStamp]
	) VALUES (
		N'61622774-9d95-4859-aa78-13b12bc84707'
		, @@IDENTITY
		, 1
		, N'TYWOOLLE'
		, N'tyler.woolley@gov.bc.ca'
		, N'61622774-9d95-4859-aa78-13b12bc84707'
	)

	-- Add Tyler to System Administrators Role
	INSERT dbo.[AspNetUserRoles] (
		[UserId]
		, [RoleId]
		, [Discriminator]
	) VALUES (
		N'61622774-9d95-4859-aa78-13b12bc84707'
		, N'5310a4ac-5eed-4dfa-8603-f7c616bce47a'
		, N'IdentityUserRole'
	)
END

IF (NOT EXISTS (SELECT 1 FROM dbo.[InternalUsers] WHERE [IDIR] = 'REGEORGE'))
BEGIN
	-- Add Reeja George to the default internal users.
	INSERT dbo.[InternalUsers] (
		[IDIR]
		, [FirstName]
		, [LastName]
		, [Salutation]
		, [Email]
		, [PhoneNumber]
		, [DateAdded]
	) VALUES (
		N'REGEORGE'
		, N'Reeja'
		, N'George'
		, N'Mrs.'
		, N'reeja.george@gov.bc.ca'
		, N'123-123-1234'
		, GETUTCDATE()
	)

	-- Create a user account for Tyler.
	INSERT dbo.[AspNetUsers] (
		[ApplicationUserId]
		, [InternalUserId]
		, [Active]
		, [UserName]
		, [Email]
		, [SecurityStamp]
	) VALUES (
		N'f811bbb3-e66b-4dd7-80f4-f6c4ecad5440'
		, @@IDENTITY
		, 1
		, N'REGEORGE'
		, N'reeja.george@gov.bc.ca'
		, N'f811bbb3-e66b-4dd7-80f4-f6c4ecad5440'
	)

	-- Add Tyler to System Administrators Role
	INSERT dbo.[AspNetUserRoles] (
		[UserId]
		, [RoleId]
		, [Discriminator]
	) VALUES (
		N'f811bbb3-e66b-4dd7-80f4-f6c4ecad5440'
		, N'5310a4ac-5eed-4dfa-8603-f7c616bce47a'
		, N'IdentityUserRole'
	)
END

CHECKPOINT