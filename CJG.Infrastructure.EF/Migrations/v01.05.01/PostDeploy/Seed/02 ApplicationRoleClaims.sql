PRINT 'Updating [ApplicationRoleClaims]'

-- assessor
IF (EXISTS (SELECT TOP 1 * FROM [dbo].[ApplicationRoleClaims] WHERE [RoleId] = (SELECT TOP 1 Id FROM dbo.ApplicationRoles ar WHERE ar.Name = 'Assessor') AND [ClaimId] = (SELECT TOP 1 Id FROM dbo.ApplicationClaims WHERE ClaimType = 'Privilege' AND ClaimValue = 'AM5')))
BEGIN
	DELETE FROM [dbo].[ApplicationRoleClaims] WHERE [RoleId] = (SELECT TOP 1 Id FROM dbo.ApplicationRoles ar WHERE ar.Name = 'Assessor') AND [ClaimId] = (SELECT TOP 1 Id FROM dbo.ApplicationClaims WHERE ClaimType = 'Privilege' AND ClaimValue = 'AM5')
END

-- financial clerk
IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[ApplicationRoleClaims] WHERE [RoleId] = (SELECT TOP 1 Id FROM dbo.ApplicationRoles ar WHERE ar.Name = 'Financial Clerk')))
BEGIN
	INSERT [dbo].[ApplicationRoleClaims]
		(ClaimId, RoleId)
			SELECT ac.ID, (SELECT TOP 1 Id FROM dbo.ApplicationRoles ar WHERE ar.Name = 'Financial Clerk')                FROM dbo.ApplicationClaims ac WHERE ac.ClaimType = 'Privilege' AND ac.ClaimValue IN ('IA1', 'IA2', 'IA4', 'AM1', 'AM5', 'PM1', 'TP1')
END