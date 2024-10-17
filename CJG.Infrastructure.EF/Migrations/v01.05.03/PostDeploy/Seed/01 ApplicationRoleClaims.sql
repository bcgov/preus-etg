PRINT 'Updating [ApplicationRoleClaims]'
-- Correcting privileges assigned to roles

-- Assessor Role, add IA2
INSERT INTO ApplicationRoleClaims (RoleId, ClaimId)  
SELECT 
	(SELECT TOP 1 Id FROM ApplicationRoles ar WHERE ar.[Name] = 'Assessor') AS RoleId, 
	(SELECT TOP 1 Id FROM ApplicationClaims ac WHERE ac.[ClaimValue] = 'IA2') As ClaimId

-- Director Role, remove AM2
DELETE FROM ApplicationRoleClaims
WHERE 
	RoleId = (SELECT TOP 1 Id FROM ApplicationRoles ar WHERE ar.[Name] = 'Director') AND
	ClaimId = (SELECT TOP 1 Id FROM ApplicationClaims ac WHERE ac.[ClaimValue] = 'AM2')
-- Director Role, remove AM2
DELETE FROM ApplicationRoleClaims
WHERE 
	RoleId = (SELECT TOP 1 Id FROM ApplicationRoles ar WHERE ar.[Name] = 'Director') AND
	ClaimId = (SELECT TOP 1 Id FROM ApplicationClaims ac WHERE ac.[ClaimValue] = 'AM5')

-- Operation Manager Role, remove AM2
DELETE FROM ApplicationRoleClaims
WHERE 
	RoleId = (SELECT TOP 1 Id FROM ApplicationRoles ar WHERE ar.[Name] = 'Operations Manager') AND
	ClaimId = (SELECT TOP 1 Id FROM ApplicationClaims ac WHERE ac.[ClaimValue] = 'PM2')

-- Assessor Role, add IA2
INSERT INTO ApplicationRoleClaims (RoleId, ClaimId)  
SELECT 
	(SELECT TOP 1 Id FROM ApplicationRoles ar WHERE ar.[Name] = 'Operations Manager') AS RoleId, 
	(SELECT TOP 1 Id FROM ApplicationClaims ac WHERE ac.[ClaimValue] = 'TP2') As ClaimId
