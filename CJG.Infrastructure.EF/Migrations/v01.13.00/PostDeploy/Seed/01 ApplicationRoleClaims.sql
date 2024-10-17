PRINT 'Updating [ApplicationRoleClaims] - Removing AM3, AM4, and AM5 from System Administrator role'

-- Removing AM3, AM4, and AM5 from System Administrator role
DELETE FROM ApplicationRoleClaims
WHERE 
	RoleId = (SELECT TOP 1 Id FROM AspNetRoles ar WHERE ar.[Name] = 'System Administrator') AND
	ClaimId = (SELECT TOP 1 Id FROM ApplicationClaims ac WHERE ac.[ClaimValue] = 'AM3')

DELETE FROM ApplicationRoleClaims
WHERE 
	RoleId = (SELECT TOP 1 Id FROM AspNetRoles ar WHERE ar.[Name] = 'System Administrator') AND
	ClaimId = (SELECT TOP 1 Id FROM ApplicationClaims ac WHERE ac.[ClaimValue] = 'AM4')

DELETE FROM ApplicationRoleClaims
WHERE 
	RoleId = (SELECT TOP 1 Id FROM AspNetRoles ar WHERE ar.[Name] = 'System Administrator') AND
	ClaimId = (SELECT TOP 1 Id FROM ApplicationClaims ac WHERE ac.[ClaimValue] = 'AM5')